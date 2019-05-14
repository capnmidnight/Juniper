using System;

using Accord.Extensions.Statistics.Filters;
using Accord.Math;

using UnityEngine;

using Vector3 = UnityEngine.Vector3;

namespace Juniper.Statistics
{
    [Serializable]
    [CreateAssetMenu(fileName = "kalmanMotionFilter", menuName = "Motion Filters/Kalman")]
    public class KalmanMotionFilter : AbstractMotionFilter
    {
        public uint componentCount = 2;

        private uint ComponentCount
        {
            get; set;
        }

        public float measurementNoise = 10;
        private float mNoise;

        public uint processNoiseScale = 100;
        private uint procNoiseK;

        [Range(0, 10)]
        public float processNoiseExponent = 1;

        private float procNoiseE;

        private const uint VectorSize = 3;

        private uint Dimension
        {
            get; set;
        }

        private DiscreteKalmanFilter<Vector3[], Vector3> Kalman
        {
            get; set;
        }

        private uint X(uint i)
        {
            return i;
        }

        private uint Y(uint i)
        {
            return i + ComponentCount;
        }

        private uint Z(uint i)
        {
            return i + (2 * ComponentCount);
        }

        public override Vector3 PredictedPosition
        {
            get
            {
                Kalman?.Predict();
                return Position;
            }
        }

        public override Vector3 Position
        {
            get
            {
                return Kalman?.State[0] ?? Vector3.zero;
            }
        }

#if UNITY_EDITOR

        public override void Copy(AbstractMotionFilter filter)
        {
            if (filter is KalmanMotionFilter f)
            {
                componentCount = f.componentCount;
                measurementNoise = f.measurementNoise;
                processNoiseScale = f.processNoiseScale;
            }
        }

#endif

        private Vector3? lastPoint;

        public override void UpdateState(Vector3 point)
        {
            componentCount = Math.Max(1, componentCount);

            if (Kalman != null
                && componentCount == ComponentCount
                && Mathf.Approximately(measurementNoise, mNoise)
                && processNoiseScale == procNoiseK
                && Mathf.Approximately(processNoiseExponent, procNoiseE))
            {
                Kalman.Correct(point);
            }
            else
            {
                ComponentCount = componentCount;
                Dimension = VectorSize * ComponentCount;
                mNoise = measurementNoise;
                procNoiseK = processNoiseScale;
                procNoiseE = processNoiseExponent;

                var initialState = new Vector3[ComponentCount];
                initialState[0] = lastPoint ?? point;

                Kalman = new DiscreteKalmanFilter<Vector3[], Vector3>(
                    initialState,
                    GetProcessNoise(_ => processNoiseScale),
                    (int)VectorSize,
                    0,
                    ToArray,
                    FromArray,
                    x => new double[] { x.x, x.y, x.z })
                {
                    ProcessNoise = GetProcessNoise(i =>
                        Mathf.Pow(processNoiseScale, processNoiseExponent * (i + 1))),
                    MeasurementMatrix = MakePositionMeasurementMatrix(),
                    TransitionMatrix = MakeTransitionMatrix(),
                    MeasurementNoise = Matrix.Diagonal((int)VectorSize, (double)measurementNoise)
                };

                if (lastPoint != null)
                {
                    Kalman.Correct(point);
                }
                Kalman.Predict();
                lastPoint = point;
            }
        }

        private Vector3[] FromArray(double[] arr)
        {
            var vecs = new Vector3[ComponentCount];
            for (uint i = 0; i < ComponentCount; ++i)
            {
                vecs[i].x = (float)arr[X(i)];
                vecs[i].y = (float)arr[Y(i)];
                vecs[i].z = (float)arr[Z(i)];
            }
            return vecs;
        }

        private double[] ToArray(Vector3[] vecs)
        {
            var arr = new double[Dimension];
            for (uint i = 0; i < vecs.Length; ++i)
            {
                arr[X(i)] = vecs[i].x;
                arr[Y(i)] = vecs[i].y;
                arr[Z(i)] = vecs[i].z;
            }
            return arr;
        }

        private double[,] GetProcessNoise(Func<uint, double> getProcNoiseForVec)
        {
            var vec = new double[Dimension];
            for (uint i = 0; i < ComponentCount; ++i)
            {
                for (uint x = 0; x < VectorSize; ++x)
                {
                    vec[i * VectorSize + x] = getProcNoiseForVec(i);
                }
            }
            return Matrix.Diagonal((int)Dimension, vec);
        }

        private double[,] MakePositionMeasurementMatrix()
        {
            var m = new double[VectorSize, ComponentCount * VectorSize];
            for (uint i = 0; i < VectorSize; ++i)
            {
                m[i, i * ComponentCount] = 1;
            }
            return m;
        }

        private double[,] MakeTransitionMatrix()
        {
            var t = Time.fixedDeltaTime;
            var parts = new double[ComponentCount];
            double accum = 1;
            for (uint i = 0; i < ComponentCount; ++i)
            {
                parts[i] = accum * Mathf.Pow(t, i);
                accum /= i + 1;
            }

            var m = new double[Dimension, Dimension];
            for (uint y = 0; y < Dimension; ++y)
            {
                var chunkRow = y % ComponentCount;
                for (uint x = 0; x < ComponentCount - chunkRow; ++x)
                {
                    m[y, y + x] = parts[x];
                }
            }

            return m;
        }
    }
}
