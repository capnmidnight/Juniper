using Juniper.World.GIS;

using System;

using UnityEngine;

namespace Juniper.World
{
    /// <summary>
    /// Calculates the position of the Sun in relation to the user's current position on spaceship
    /// Earth. Only one of this component is allowed on a gameObject at a time. This component also
    /// requires a <see cref="GPSLocation"/> component on the same gameObject and a <see
    /// cref="CompassRose"/> component somewhere in the scene. This component executes in the editor.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GPSLocation))]
    public class SunPosition : MonoBehaviour
    {
        /// <summary>
        /// When set to true, the component will use a static, set time (set in the field <see
        /// cref="time"/> instead of the current system time.
        /// </summary>
        public bool FakeTime;

        /// <summary>
        /// The static time to use, when <see cref="FakeTime"/> is set to true.
        /// </summary>
        public EditableDateTime time;

        /// <summary>
        /// The pose of the sun from the user's current vantage point.
        /// </summary>
        [HideInInspector]
        public HorizontalSphericalPosition orientation;

        /// <summary>
        /// When set to true, the Update function will also print a report showing the internal state
        /// of the component.
        /// </summary>
        public bool DebugReport;

        /// <summary>
        /// Returns true when <see cref="location"/> is not null and has a valid GPS coordinate, and
        /// <see cref="compass"/> is not null and has a valid Heading.
        /// </summary>
        /// <value><c>true</c> if can calculate current local position; otherwise, <c>false</c>.</value>
        public bool CanCalculateCurrentLocalPosition
        {
            get
            {
                return compass != null
                    && compass.HasHeading
                    && location?.HasCoord == true;
            }
        }

        /// <summary>
        /// Retrieves the current <see cref="location"/> and/or <see cref="compass"/>, if they are
        /// needed, updates the <see cref="currentTime"/>, calculates the correct sun position from
        /// that time, and optionally prints a debug report.
        /// </summary>
        public void Update()
        {
            if (location == null)
            {
                location = GetComponent<GPSLocation>();
            }

            if (compass == null)
            {
                compass = ComponentExt.FindAny<CompassRose>();
            }

            currentTime = GetTime();

            orientation = location.Coord.ToSunPosition(currentTime);

            if (DebugReport)
            {
                PrintDebugReport();
            }
        }

        /// <summary>
        /// The GPS management component.
        /// </summary>
        private GPSLocation location;

        /// <summary>
        /// The magnetic compass management component.
        /// </summary>
        private CompassRose compass;

        /// <summary>
        /// The time for which the sun position is being calculated.
        /// </summary>
        private DateTime currentTime;

        /// <summary>
        /// Gets either the fake time the user specified for the component, or the current system
        /// time if <see cref="FakeTime"/> is set to false.
        /// </summary>
        /// <returns>The time.</returns>
        private DateTime GetTime()
        {
            if (!FakeTime)
            {
                time = EditableDateTime.Now.FixTime();
            }

            return time.Value.ToUniversalTime().FixTime();
        }

        /// <summary>
        /// Prints a report showing the internal state of the component.
        /// </summary>
        private void PrintDebugReport()
        {
            ScreenDebugger.PrintFormat("  {0}", currentTime.ToLocalTime().FixTime());
            if (location == null)
            {
                ScreenDebugger.Print("  No location service");
            }
            else if (!location.HasCoord)
            {
                ScreenDebugger.Print("  No location yet");
            }

            if (compass == null)
            {
                ScreenDebugger.Print("  No compass service");
            }
            else if (!compass.HasHeading)
            {
                ScreenDebugger.Print("  No compass heading yet");
            }

            if (location?.HasCoord == true && compass?.HasHeading == true)
            {
                ScreenDebugger.PrintFormat("  Azimuth {0}, Altitude {1}",
                    orientation.AzimuthDegrees.Label(UnitOfMeasure.Degrees, 3),
                    orientation.AltitudeDegrees.Label(UnitOfMeasure.Degrees, 3));
            }
        }
    }
}