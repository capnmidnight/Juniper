using Juniper.Statistics;
using Juniper.Unity.Display;

using System;

using UnityEngine;

using UnityInput = UnityEngine.Input;

namespace Juniper.Unity.World
{
    /// <summary>
    /// A component for managing orientation to Earth's magnetic field (or any sufficiently large
    /// magnetic field in proximity to the device. Let's be honest, a refrigerator magnet is going to
    /// throw this thing off.)
    /// </summary>
    [ExecuteInEditMode]
    public class CompassRose : MonoBehaviour
    {
        /// <summary>
        /// When set to true, the <see cref="toNorthHeading"/> value will not be based off of the
        /// magnetic compass reading and will instead be a static value that the developer sets in
        /// the Unity Editor.
        /// </summary>
        public bool FakeHeading;

        /// <summary>
        /// When <see cref="FakeHeading"/> is set to true, this value is the input value of the
        /// heading offset for an object to align it to north with respect to the camera view. When
        /// FakeHeading is set to false, this value is the output value of that heading.
        /// </summary>
        public float toNorthHeading;

        /// <summary>
        /// The number of values to collect before we can consider the averaging to be valid.
        /// </summary>
        public int averagingIterations = 20;

        /// <summary>
        /// When set to true, the Update loop prints a report on the internal workings of this component.
        /// </summary>
        public bool DebugReport;

        /// <summary>
        /// Set to true when the <see cref="CameraHeading"/> value is valid.
        /// </summary>
        /// <value><c>true</c> if has heading; otherwise, <c>false</c>.</value>
        public bool HasHeading
        {
            get; private set;
        }

        /// <summary>
        /// Setup the statistical analyzers and find the main camera.
        /// </summary>
        public void Awake()
        {
            camT = DisplayManager.MainCamera.transform;
            samples = new SingleStatistics(averagingIterations);
        }

        /// <summary>
        /// If <see cref="UseFakeHeading"/> is false, calculate the angle offset to correct the
        /// positioning of the object.
        /// </summary>
        public void Update()
        {
            if (UseFakeHeading)
            {
                HasHeading = true;
            }
            else
            {
                SetNorth();
            }

            if (!float.IsNaN(toNorthHeading))
            {
                transform.rotation = Quaternion.Euler(0, toNorthHeading, 0);
            }

            if (DebugReport)
            {
                PrintDebugReport();
            }
        }

        /// <summary>
        /// Compass heading readings analzyer.
        /// </summary>
        private SingleStatistics samples;

        /// <summary>
        /// An angle movement smoother for avoiding big jumps from 1 to 359 degrees.
        /// </summary>
        private Angle deltaAngle;

        /// <summary>
        /// The transform of the main camera.
        /// </summary>
        private Transform camT;

        /// <summary>
        /// Gets the angle in the X-axis in which the compass needle is pointing.
        /// </summary>
        /// <value>The compass heading.</value>
        private static float CompassHeading
        {
            get
            {
                return UnityInput.compass.magneticHeading;
            }
        }

        /// <summary>
        /// Gets the angle in the Y-axis in which the camera is facing.
        /// </summary>
        /// <value>The camera heading.</value>
        private float CameraHeading
        {
            get
            {
                return camT.eulerAngles.y;
            }
        }

        /// <summary>
        /// Returns true if the user requested a static heading, the magnetic compass has been
        /// disabled, or the app is running in the Unity Editor.
        /// </summary>
        /// <value><c>true</c> if use fake heading; otherwise, <c>false</c>.</value>
        private bool UseFakeHeading
        {
            get
            {
                return FakeHeading || !UnityInput.compass.enabled && Application.isEditor;
            }
        }

        /// <summary>
        /// Returns true when the camera's forward vector is roughly pointing parrallel to the ground.
        /// </summary>
        /// <value><c>true</c> if can see horizon; otherwise, <c>false</c>.</value>
        private bool CanSeeHorizon
        {
            get
            {
                var headingEuler = new Vector3(0, CameraHeading, 0);
                var headingQuat = Quaternion.Euler(headingEuler);
                var headingVec = headingQuat * Vector3.forward;
                return Vector3.Dot(camT.forward, headingVec) > 0.7f;
            }
        }

        /// <summary>
        /// Returns true when the camera's up vector is not pointing at the ground.
        /// </summary>
        /// <value><c>true</c> if is upside down; otherwise, <c>false</c>.</value>
        private bool IsUpsideDown
        {
            get
            {
                return Vector3.Dot(camT.up, Vector3.up) < 0;
            }
        }

        /// <summary>
        /// Figures out the offset angle to apply to the gameObject to make it convincingly appear
        /// aligned to Magnetic North.
        /// </summary>
        private void SetNorth()
        {
            if (CanSeeHorizon && !IsUpsideDown)
            {
                var sample = CameraHeading - CompassHeading;
                deltaAngle ^= sample;
                samples.Add(deltaAngle);
                if (samples.Mean != null)
                {
                    toNorthHeading = samples.Mean.Value;
                    HasHeading = true;
                }
            }

            if (DebugReport)
            {
                PrintDebugReport();
            }
        }

        /// <summary>
        /// When <see cref="DebugReport"/> is set to true, the <see cref="Update"/> function calls
        /// this function at its end.
        /// </summary>
        private void PrintDebugReport()
        {
            try
            {
                if (!CanSeeHorizon)
                {
                    ScreenDebugger.Print("Can't see the horizon");
                }

                if (IsUpsideDown)
                {
                    ScreenDebugger.Print("Device is upside down");
                }

                if (!HasHeading)
                {
                    ScreenDebugger.Print("No heading available yet");
                }

                if (camT == null)
                {
                    ScreenDebugger.Print("  Please set the MainCamera tag on a camera in the scene");
                }
                else if (UseFakeHeading)
                {
                    ScreenDebugger.Print("  Using a static heading");
                }
                else if (!UnityInput.compass.enabled)
                {
                    ScreenDebugger.Print("  Compass not available");
                }
                else
                {
                    var deltaHeading = CompassHeading - CameraHeading;
                    var sampleProgress = samples.Count / (float)averagingIterations;
                    ScreenDebugger.Print($"  Raw Magnetometer {CompassHeading.Label(UnitOfMeasure.Degrees, 4)}");
                    ScreenDebugger.Print($"  Camera Angle {CameraHeading.Label(UnitOfMeasure.Degrees, 4)}");
                    ScreenDebugger.Print($"  Delta {deltaHeading.Label(UnitOfMeasure.Degrees, 4)}");
                    ScreenDebugger.Print($"  To North {toNorthHeading.Label(UnitOfMeasure.Degrees, 4)}");
                    ScreenDebugger.Print($"  North {toNorthHeading.Label(UnitOfMeasure.Degrees, 4)} ({samples.Count}/{averagingIterations}: {sampleProgress.Label(UnitOfMeasure.Proportion)}, σ = {samples.StandardDeviation})");
                }
            }
            catch (Exception exp)
            {
                ScreenDebugger.PrintException(exp, "CompassRose");
            }
        }
    }
}
