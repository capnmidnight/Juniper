using System;

using Juniper.Statistics;

using UnityEngine;
using UnityEngine.Events;

using UnityInput = UnityEngine.Input;

namespace Juniper.Unity.Widgets
{
    /// <summary>
    /// This component manages the activity of the user picking up a mobile device and holding it,
    /// firing an event when they first pick up an object and firing another event if there has been
    /// no movement for a set amount of time (defaults to 30 seconds);
    /// </summary>
    public class Shaker : MonoBehaviour
    {
        /// <summary>
        /// The number of seconds to wait with no activity to consider the user inactive.
        /// </summary>
        public float timeout = 30;

        /// <summary>
        /// The amount of deviation from the mean to allow before considering a gyro sample to be a "shake".
        /// </summary>
        public float TorqueK = 0.01f;

        /// <summary>
        /// The amount of deviation from the mean to allow before considering an accelerometer sample
        /// to be a "shake".
        /// </summary>
        public float ForceK = 0.005f;

        /// <summary>
        /// Set to true to also wake up the device after the screen has been touched.
        /// </summary>
        public bool includeMouseAndTouchDetection = true;

        /// <summary>
        /// Set to true to include a debug report on ScreenDebugger.
        /// </summary>
        public bool DebugReport;

        /// <summary>
        /// An event that is fired when the device has been shaken after a period of inactivity.
        /// </summary>
        public UnityEvent onShook = new UnityEvent();

        /// <summary>
        /// An event that is fired when there hasn't been any movement in for <see cref="timeout"/> seconds.
        /// </summary>
        public UnityEvent onInactive = new UnityEvent();

        /// <summary>
        /// An event that is fired when the device has been shaken after a period of inactivity.
        /// </summary>
        public event EventHandler Shook;

        /// <summary>
        /// An event that is fired when there hasn't been any movement in for <see cref="timeout"/> seconds.
        /// </summary>
        public event EventHandler Inactive;

        /// <summary>
        /// Gets the current user's status as active or inactive.
        /// </summary>
        /// <value><c>true</c> if is inactive; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get; private set;
        }

        /// <summary>
        /// Triggers the statistical analyzer in 3 seconds, to wait for the device gyro to settle down.
        /// </summary>
        public void OnEnable()
        {
            UnityInput.gyro.enabled = SystemInfo.supportsGyroscope;
            Invoke(nameof(StartSampling), 3);
        }

        /// <summary>
        /// Shuts down the gyro and statistical analysis.
        /// </summary>
        public void OnDisable()
        {
            CancelInvoke();
            UnityInput.gyro.enabled = false;
            torques = null;
        }

        /// <summary>
        /// Measures any shake or pointer movement to figure out if events need to be fired.
        /// </summary>
        public void Update()
        {
            torques?.Add(UnityInput.gyro.rotationRate.magnitude);
            accelerations?.Add(UnityInput.acceleration.magnitude);

            if (Application.isEditor || HasShaken || HasPointerMoved)
            {
                Restart();
                OnShook();
            }

            prevMousePosition = UnityInput.mousePosition;

            if (DebugReport)
            {
                PrintDebugReport();
            }
        }

        /// <summary>
        /// The number number of gyroscope samples to analyze in sequence.
        /// </summary>
        private const int RINGBUFFER_SIZE = 15;

        /// <summary>
        /// A statistical analyzer for the gyroscope data to determine if a shake has happened.
        /// </summary>
        private SingleStatistics torques;

        /// <summary>
        /// A statistical analyzer for the accelerometer data to determine if a shake has happened.
        /// </summary>
        private SingleStatistics accelerations;

        /// <summary>
        /// Keeping track of the moues position of the last frame to detect when the mouse has moved,
        /// if <see cref="includeMouseAndTouchDetection"/> is set to true.
        /// </summary>
        private Vector3 prevMousePosition;

        /// <summary>
        /// returns true if the statistical analyzer says there have been a lot of vibrations outside
        /// of the expected <see cref="TorqueK"/> or <see cref="ForceK"/> range.
        /// </summary>
        /// <value><c>true</c> if has shaken; otherwise, <c>false</c>.</value>
        private bool HasShaken
        {
            get
            {
                return HasTorqued || HasAccelerated;
            }
        }

        /// <summary>
        /// Returns true when the torque values breach the torque threashold.
        /// </summary>
        /// <value><c>true</c> if has torqued; otherwise, <c>false</c>.</value>
        private bool HasTorqued
        {
            get
            {
                return torques?.IsSaturated == true
                    && torques.StandardDeviation > TorqueK;
            }
        }

        /// <summary>
        /// Returns true when the acceleration values breach the force threshold.
        /// </summary>
        /// <value><c>true</c> if has accelerated; otherwise, <c>false</c>.</value>
        private bool HasAccelerated
        {
            get
            {
                return accelerations?.IsSaturated == true
                    && accelerations.StandardDeviation > ForceK;
            }
        }

        /// <summary>
        /// Returns true if <see cref="includeMouseAndTouchDetection"/> has been set and the mouse
        /// has been moved or the touch screen touched.
        /// </summary>
        /// <value><c>true</c> if has pointer moved; otherwise, <c>false</c>.</value>
        private bool HasPointerMoved
        {
            get
            {
                return includeMouseAndTouchDetection
                    && (UnityInput.anyKeyDown
                        || UnityInput.mousePosition != prevMousePosition
                        || UnityInput.touchCount > 0);
            }
        }

        /// <summary>
        /// Fires the <see cref="onShook"/> and <see cref="Shook"/> events.
        /// </summary>
        private void OnShook()
        {
            if (!IsActive)
            {
                IsActive = true;
                onShook?.Invoke();
                Shook?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires the <see cref="onInactive"/> and <see cref="Inactive"/> events.
        /// </summary>
        private void OnInactive()
        {
            if (IsActive)
            {
                IsActive = false;
                onInactive?.Invoke();
                Inactive?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets up the statistical analyzer
        /// </summary>
        private void StartSampling()
        {
            torques = new SingleStatistics(RINGBUFFER_SIZE);
            accelerations = new SingleStatistics(RINGBUFFER_SIZE);
            Restart();
        }

        /// <summary>
        /// Restart waiting for inactivity.
        /// </summary>
        private void Restart()
        {
            CancelInvoke();
            Invoke(nameof(OnInactive), timeout);
        }

        /// <summary>
        /// Prints a report to the screen that reveals the internal state of the component.
        /// </summary>
        private void PrintDebugReport()
        {
            if (torques == null)
            {
                ScreenDebugger.Print("Torque sampling hasn't started yet.");
            }
            else
            {
                ScreenDebugger.PrintFormat("Torqued {3} = |{0}| >{1}< {2}",
                                     torques.Mean.Label(UnitOfMeasure.Degrees, 3),
                                     torques.StandardDeviation.Label(UnitOfMeasure.Degrees),
                                     TorqueK,
                                     HasTorqued);
            }

            if (accelerations == null)
            {
                ScreenDebugger.Print("Acceleration sampling hasn't started yet.");
            }
            else
            {
                ScreenDebugger.PrintFormat("Acceled {3} = |{0}| >{1}< {2}",
                                     accelerations.Mean.Label(UnitOfMeasure.Degrees, 3),
                                     accelerations.StandardDeviation.Label(UnitOfMeasure.Degrees),
                                     ForceK,
                                     HasAccelerated);
                ScreenDebugger.PrintFormat("F{0} L{1} m{2} M{3} _|_{4} |{5}| >{6}< {7}", accelerations.Mean, accelerations.StandardDeviation, ForceK);
            }

            ScreenDebugger.PrintFormat("Acceled {0} || Torqued {1} = Shaken {2}", HasAccelerated, HasTorqued, HasShaken);
        }
    }
}