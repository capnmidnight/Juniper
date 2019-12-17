using System;

using Accord.Math;
using Accord.Statistics.Distributions.Univariate;

using static System.Math;

namespace Juniper.Mathematics
{
    /// <summary>
    /// A Kalman filter is a recursive solution to the general dynamic estimation problem for the
    /// important special case of linear system models and Gaussian noise.
    /// <para>The Kalman Filter uses a predictor-corrector structure, in which
    /// if a measurement of the system is available at time <italic>t</italic>,
    /// We first call the Predict function, to estimate the state of the system
    /// at time <italic>t</italic>. We then call the Correct function to
    /// correct the estimate of state, based on the noisy measurement.</para>
    ///
    /// <para>
    /// The discrete Kalman filter can process linear models which have Gaussian noise.
    /// If the model is not linear then estimate transition matrix (and other parameters if necessary) in each step and update Kalman filter.
    /// This "dynamic" version of an Discrete Kalman filter is called Extended Kalman filter and it is used for non-linear models.
    /// If the model is highly non-linear an Unscented Kalman filter or particle filtering is used.
    /// See: <a href="http://en.wikipedia.org/wiki/Kalman_filter"/> for details.
    /// </para>
    /// </summary>
    public class DiscreteKalmanFilter<TState, TMeasurement>
    {
        /// <summary>
        /// Upper limit which includes valid measurements (gating) with 99% probability
        /// </summary>
        private static readonly double gateThreshold = new ChiSquareDistribution(2).InverseDistributionFunction(0.99);

        private readonly Func<double[], TState> stateConvertBackFunc;
        private readonly Func<TMeasurement, double[]> measurementConvertFunc;
        private double[] state;

        /// <summary>
        /// Creates Discrete Kalman filter.
        /// </summary>
        /// <param name="initialState">The best estimate of the initial state. [n x 1] vector. It's dimension is - n.</param>
        /// <param name="initialStateError">Initial error for a state: (assumed values – actual values)^2 + the variance of the values.
        /// <para>e.g. if using ConstantAccelerationModel it can be specified as: Matrix.Diagonal(StateVectorDimension, [x, y, vX, vY, aX, aY]);</para>
        /// </param>
        /// <param name="measurementVectorDimension">Dimensionality of the measurement vector - p.</param>
        /// <param name="controlVectorDimension">Dimensionality of the control vector - k. If there is no external control put 0.</param>
        /// <param name="stateConvertFunc">State conversion function: TState => double[]</param>
        /// <param name="stateConvertBackFunc">State conversion function: double[] => TState</param>
        /// <param name="measurementConvertFunc">Measurement conversion function: TMeasurement => double[]</param>
        public DiscreteKalmanFilter(TState initialState, double[,] initialStateError,
                                    int measurementVectorDimension, int controlVectorDimension,
                                    Func<TState, double[]> stateConvertFunc, Func<double[], TState> stateConvertBackFunc, Func<TMeasurement, double[]> measurementConvertFunc)
        {
            var _state = stateConvertFunc(initialState);
            StateVectorDimension = _state.Length;
            MeasurementVectorDimension = measurementVectorDimension;
            ControlVectorDimension = controlVectorDimension;
            state = _state;
            EstimateCovariance = initialStateError;
            this.stateConvertBackFunc = stateConvertBackFunc;
            this.measurementConvertFunc = measurementConvertFunc;
        }

        /// <summary>
        /// Gets state (x(k)). [1 x n] vector.
        /// After obtaining a measurement z(k) predicted state will be corrected.
        /// This value is used as an ultimate result.
        /// </summary>
        public TState State
        {
            get { return stateConvertBackFunc(state); }
        }

        /// <summary>
        /// Gets Kalman covariance matrix (S). [p x p] matrix.
        /// This matrix servers for Kalman gain calculation.
        /// <para>The matrix along with innovation vector can be used to achieve gating in JPDAF. See: <see cref="Accord.Extensions.Statistics.Filters.JPDAF"/> filter.</para>
        /// </summary>
        public double[,] ResidualCovariance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the inverse of covariance matrix. See: <see cref="ResidualCovariance"/>.
        /// </summary>
        public double[,] ResidualCovarianceInv
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets Kalman gain matrix (K). [n x p] matrix.
        /// </summary>
        public double[,] KalmanGain
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets error estimate covariance matrix (P(k)). [n x n] matrix.
        /// </summary>
        public double[,] EstimateCovariance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets state transition matrix (A). [n x n] matrix.
        /// </summary>
        public double[,] TransitionMatrix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets control matrix (B). [n x k] vector.
        /// It is not used if there is no control.
        /// </summary>
        public double[,] ControlMatrix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets measurement matrix (H). [p x n] matrix, where p is a dimension of measurement vector. <br/>
        /// <para>Selects components from a state vector that are obtained by measurement.</para>
        /// </summary>
        public double[,] MeasurementMatrix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets process noise covariance matrix (Q). [n x n] matrix.
        /// <para>Deviation of selected and actual model.
        /// e.g. for constant acceleration model it can be defined as: Matrix.Diagonal(StateVectorDimension, [x, y, vX, vY, aX, aY]); where usually (position error) &lt; (velocity error) &lt; (acceleration error).</para>
        /// </summary>
        public double[,] ProcessNoise
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets measurement noise covariance matrix (R). [p x p] matrix.
        /// <para>Variance inaccuracy of detected location.
        /// It is usually defined as scalar, therefore it can be set as: Matrix.Diagonal(MeasurementVectorDimension, [value]);</para>
        /// </summary>
        public double[,] MeasurementNoise
        {
            get;
            set;
        }

        /// <summary>
        /// Gets state dimensionality.
        /// </summary>
        public int StateVectorDimension
        {
            get;
        }

        /// <summary>
        /// Gets measurement dimensionality.
        /// </summary>
        public int MeasurementVectorDimension
        {
            get;
        }

        /// <summary>
        /// Gets control vector dimensionality.
        /// </summary>
        public int ControlVectorDimension
        {
            get;
        }

        /// <summary>
        /// Checks pre-conditions: matrix sizes.
        /// </summary>
        private void CheckPrerequisites()
        {
            /************************** TRANSITION MATRIX ***************************/
            if (TransitionMatrix == null)
            {
                throw new Exception("Transition matrix cannot be null!");
            }

            if (TransitionMatrix.GetLength(0) != StateVectorDimension || TransitionMatrix.GetLength(1) != StateVectorDimension)
            {
                throw new Exception("Transition matrix dimensions are not valid!");
            }
            /************************** TRANSITION MATRIX ***************************/

            /************************** CONTROL MATRIX ***************************/
            if (ControlMatrix == null && ControlVectorDimension != 0)
            {
                throw new Exception("Control matrix can be null only if control vector dimension is set to 0!");
            }

            if (ControlMatrix != null && (ControlMatrix.GetLength(0) != StateVectorDimension || ControlMatrix.GetLength(1) != ControlVectorDimension))
            {
                throw new Exception("Control matrix dimensions are not valid!");
            }
            /************************** CONTROL MATRIX ***************************/

            /************************** MEASUREMENT MATRIX ***************************/
            if (MeasurementMatrix == null)
            {
                throw new Exception("Measurement matrix cannot be null!");
            }

            if (MeasurementMatrix.GetLength(0) != MeasurementVectorDimension || MeasurementMatrix.GetLength(1) != StateVectorDimension)
            {
                throw new Exception("Measurement matrix dimesnions are not valid!");
            }
            /************************** MEASUREMENT MATRIX ***************************/

            /************************** PROCES NOISE COV. MATRIX ***************************/
            if (ProcessNoise == null)
            {
                throw new Exception("Process noise covariance matrix cannot be null!");
            }

            if (ProcessNoise.GetLength(0) != StateVectorDimension || ProcessNoise.GetLength(1) != StateVectorDimension)
            {
                throw new Exception("Process noise covariance matrix dimensions are not valid!");
            }
            /************************** PROCES NOISE COV. MATRIX ***************************/

            /************************** MEASUREMENT NOISE COV. MATRIX ***************************/
            if (MeasurementNoise == null)
            {
                throw new Exception("Measurement noise covariance matrix cannot be null!");
            }

            if (MeasurementNoise.GetLength(0) != MeasurementVectorDimension || MeasurementNoise.GetLength(1) != MeasurementVectorDimension)
            {
                throw new Exception("Measurement noise covariance matrix dimensions are not valid!");
            }
            /************************** MEASUREMENT NOISE COV. MATRIX ***************************/
        }

        /// <summary>
        /// Estimates the subsequent model state.
        /// This function is implementation-dependent.
        /// </summary>
        public void Predict()
        {
            Predict(null);
        }

        /// <summary>
        /// Predicts the next state using the current state and <paramref name="controlVector"/>.
        /// </summary>
        /// <param name="controlVector">Set of data for external system control.</param>
        /// <summary>
        /// <para>Estimates the subsequent model state.</para>
        /// <para>
        /// x'(k) = A * x(k-1) + B * u(k).
        /// P'(k) = A * P(k-1) * At + Q
        /// K(k) = P'(k) * Ht * (H * P'(k) * Ht + R)^(-1)
        /// </para>
        /// </summary>
        public void Predict(double[] controlVector)
        {
            CheckPrerequisites();
            //x'(k) = A * x(k-1)
            //this.state = this.state.Multiply(this.TransitionMatrix);
            state = TransitionMatrix.Dot(state);

            //x'(k) =  x'(k) + B * u(k)
            if (controlVector != null)
            {
                state = state.Add(ControlMatrix.Dot(controlVector));
            }

            //P'(k) = A * P(k-1) * At + Q
            EstimateCovariance = TransitionMatrix.Multiply(EstimateCovariance).Multiply(TransitionMatrix.Transpose()).Add(ProcessNoise);

            /******* calculate Kalman gain **********/
            var measurementMatrixTransponsed = MeasurementMatrix.Transpose();

            //S(k) = H * P'(k) * Ht + R
            ResidualCovariance = MeasurementMatrix.Multiply(EstimateCovariance).Multiply(measurementMatrixTransponsed).Add(MeasurementNoise);
            ResidualCovarianceInv = ResidualCovariance.Inverse();

            //K(k) = P'(k) * Ht * S(k)^(-1)
            KalmanGain = EstimateCovariance.Multiply(measurementMatrixTransponsed).Multiply(ResidualCovarianceInv);
            /******* calculate Kalman gain **********/
        }

        /// <summary>
        /// Calculates the residual from the measurement and the current state.
        /// </summary>
        /// <param name="measurement">Measurement.</param>
        /// <returns>Residual, error or innovation vector.</returns>
        public double[] CalculateDelta(TMeasurement measurement)
        {
            CheckPrerequisites();

            var m = measurementConvertFunc(measurement);
            return CalculateDelta(m);
        }

        private double[] CalculateDelta(double[] measurement)
        {
            //innovation vector (measurement error)
            var stateMeasurement = MeasurementMatrix.Dot(state);
            var measurementError = measurement.Subtract(stateMeasurement);
            return measurementError;
        }

        /// <summary>
        /// Calculates Mahalanobis distance between the provided measurement and the predicted state.
        /// <para>Covariance matrix is the <see cref="ResidualCovariance"/>.</para>
        /// </summary>
        /// <param name="measurement">Measurement.</param>
        /// <param name="delta">The residual from the measurement and the current state.</param>
        /// <returns>Mahalanobis distance.</returns>
        public double CalculateMahalanobisDistance(TMeasurement measurement, out double[] delta)
        {
            CheckPrerequisites();

            var m = measurementConvertFunc(measurement);
            var stateMeasurement = MeasurementMatrix.Dot(state);
            delta = m.Subtract(stateMeasurement);

            var distance = Distance.Mahalanobis(m, stateMeasurement, ResidualCovarianceInv);
            return distance;
        }

        /// <summary>
        /// Calculates Mahalanobis distance and compares distance and gate threshold.
        /// <para>Gate threshold is obtained by calculating the 99 percent interval for the residual covariance.</para>
        /// </summary>
        /// <param name="measurement">Measurement.</param>
        /// <param name="delta">The residual from the measurement and the current state.</param>
        /// <param name="mahalanobisDistance">Mahalanobis distance.</param>
        /// <returns>True if the measurement is inside the gating, false otherwise.</returns>
        public bool IsMeasurementInsideGate(TMeasurement measurement, out double[] delta, out double mahalanobisDistance)
        {
            mahalanobisDistance = CalculateMahalanobisDistance(measurement, out delta);
            return mahalanobisDistance <= gateThreshold;
        }

        /// <summary>
        /// Calculates entropy from the error covariance matrix.
        /// </summary>
        public double CalculateEntropy()
        {
            if (EstimateCovariance == null || EstimateCovariance.GetLength(0) != EstimateCovariance.GetLength(1))
            {
                throw new ArgumentException("Error covariance matrix (P) must have the same number of rows and columns.");
            }

            var stateVectorDim = EstimateCovariance.GetLength(0);

            var errorCovDet = EstimateCovariance.Determinant();
            return 0.5f * ((stateVectorDim * Log(4 * PI)) + Log(errorCovDet));
        }

        /// <summary>
        /// <para>The function adjusts the stochastic model state on the basis of the given measurement of the model state.</para>
        /// <para>
        /// x(k) = x'(k) + K(k) * (z(k) - H * x'(k))
        /// P(k) =(I - K(k) * H) * P'(k)
        /// </para>
        /// </summary>
        /// <param name="measurement">Obtained measurement vector.</param>
        public void Correct(TMeasurement measurement)
        {
            CheckPrerequisites();
            var m = measurementConvertFunc(measurement);
            //innovation vector (measurement error)
            var delta = CalculateDelta(m);
            Correct(delta);
        }

        /// <summary>
        /// Corrects the state error covariance based on innovation vector and Kalman update.
        /// </summary>
        /// <param name="innovationVector">The difference between predicted state and measurement.</param>
        public void Correct(double[] innovationVector)
        {
            CheckPrerequisites();

            if (innovationVector.Length != MeasurementVectorDimension)
            {
                throw new Exception("PredicitionError error vector (innovation vector) must have the same length as measurement.");
            }

            //correct state using Kalman gain
            state = state.Add(KalmanGain.Dot(innovationVector));

            var identity = Matrix.Identity(StateVectorDimension);
            EstimateCovariance = (identity.Subtract(KalmanGain.Multiply(MeasurementMatrix))).Multiply(EstimateCovariance.Transpose());
        }
    }
}