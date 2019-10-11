using System;
using System.Collections;

using Juniper.IO;
using Juniper.Units;

using UnityEngine;
using UnityEngine.Events;

using UnityInput = UnityEngine.Input;

namespace Juniper.World.GIS
{
    /// <summary>
    /// Retrieves the user's latitude, longitude, and altitude from satellites orbiting Spaceship
    /// Earth. Only one of this component is allowed on a gameObject at a time. This component
    /// executes in edit mode.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class GPSLocation : MonoBehaviour
    {
        /// <summary>
        /// The time in seconds to wait before checking for the next GPS update.
        /// </summary>
        public float timeBetweenUpdates = 0.1f;

        /// <summary>
        /// Request that GPS updates be accurate to within the given number of meters.
        /// </summary>
        /// <remarks>The actual accuracy may not be the same as the requested accuracy.</remarks>
        [Tooltip("Value in meters")]
        public float desiredAccuracy = 5;

        /// <summary>
        /// Request that the GPS updates after the user has moved a given number of meters.
        /// </summary>
        /// <remarks>The actual update rate may not be the same as the requested rate.</remarks>
        [Tooltip("Value in meters")]
        public float updateDistance = 1;

        /// <summary>
        /// When set to true, don't read the GPS location, instead using a static coordinate
        /// specified in <see cref="Coord"/>.
        /// </summary>
        public bool FakeCoord;

        /// <summary>
        /// Returns true if both the latitude and longitude or non-zero.
        /// </summary>
        /// <value><c>true</c> if has coordinate; otherwise, <c>false</c>.</value>
        public bool HasCoord
        {
            get
            {
                return Coord != null
                    && (Math.Abs(Coord.Latitude) > 0.00001f
                      || Math.Abs(Coord.Longitude) > 0.00001f);
            }
        }

        /// <summary>
        /// If <see cref="FakeCoord"/> is true, this value is the input value used in place of a real
        /// GPS report. If FakeCoord is false, this value is the output value of the last GPS report.
        /// </summary>
        public LatLngPoint Coord;

        /// <summary>
        /// When set to true, the Update loop prints a report showing the internal state of the
        /// component. <see cref="PrintDebugReport"/>.
        /// </summary>
        public bool DebugReport;

        /// <summary>
        /// A Unity Event that is fired when the GPS location is updated. If you are wiring up events
        /// in the Unity Editor, prefer this version. If you are attaching event listeners
        /// programmatically, you should prefer <see cref="PositionUpdated"/>.
        /// </summary>
        public UnityEvent onPositionUpdated = new UnityEvent();

        /// <summary>
        /// A .NET Event that is fired when the GPS location is updated. If you are attaching event
        /// listeners programmatically, prefer this version. If you are wiring up events in the Unity
        /// Editor, you should prefer <see cref="onPositionUpdated"/>.
        /// </summary>
        public event EventHandler PositionUpdated;

        /// <summary>
        /// Fire the <see cref="onPositionUpdated"/> and <see cref="PositionUpdated"/> events.
        /// </summary>
        private void OnPositionUpdated()
        {
            onPositionUpdated?.Invoke();
            PositionUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// The location in PlayerPrefs where the last GPS report will be saved.
        /// </summary>
        private const string COORD_KEY = "JUNIPER_GIS_SAVED_GPS_COORD";

        /// <summary>
        /// The timestamp, in seconds, for when we should query the GPS location for an update.
        /// </summary>
        private float nextUpdateTime;

        /// <summary>
        /// The last GPS report, used to detect when changes have been made.
        /// </summary>
        private LocationInfo lastLocation;

        /// <summary>
        /// Return true if the user has disabled location services, or if <see cref="FakeCoord"/> is
        /// set to true.
        /// </summary>
        /// <value><c>true</c> if use fake coordinate; otherwise, <c>false</c>.</value>
        private bool UseFakeCoord
        {
            get
            {
                return !Location.isEnabledByUser || FakeCoord || Application.isEditor;
            }
        }

        private bool lastUseFakeCoord;

        /// <summary>
        /// Enables the compass (which is necessary for GPS updates), and attempts to retrieve the
        /// last GPS report value from the previous session out of PlayerPrefs.
        /// </summary>
        public void Awake()
        {
            nextUpdateTime = Time.unscaledTime;
            OnValidate();
        }

        private readonly ITextDecoder<LatLngPoint> coordFactory = new JsonFactory<LatLngPoint>();

        public void OnValidate()
        {
            if (!HasCoord)
            {
                if (UseFakeCoord)
                {
                    Coord = new LatLngPoint(38.881621f, -77.072478f, 0);
                }
                else if (PlayerPrefs.HasKey(COORD_KEY))
                {
                    Coord = coordFactory.Parse(PlayerPrefs.GetString(COORD_KEY));
                }
            }
        }

        /// <summary>
        /// Starts up the GPS service.
        /// </summary>
        public void OnEnable()
        {
            if (Location.isEnabledByUser && !UseFakeCoord)
            {
                this.Run(StartGPSTrackingCoroutine());
            }
#if !UNITY_EDITOR
            else
            {
                Debug.LogWarning("GPS service unavailable");
            }
#endif
        }

        /// <summary>
        /// Shuts down location services.
        /// </summary>
        public void OnDisable()
        {
            Location.Stop();
        }

        /// <summary>
        /// The location service that gets us live GPS values.
        /// </summary>
        /// <value>The location.</value>
        private static LocationService Location
        {
            get
            {
                return UnityInput.location;
            }
        }

        /// <summary>
        /// The current tracking status of <see cref="Location"/>.
        /// </summary>
        /// <value>The status.</value>
        public LocationServiceStatus Status
        {
            get
            {
                return Location.status;
            }
        }

        /// <summary>
        /// A coroutine that starts up the GPS service and waits for it to either fail (in which
        /// case, this component will be disabled), or successfully start running.
        /// </summary>
        /// <returns>The GPST racking coroutine.</returns>
        private IEnumerator StartGPSTrackingCoroutine()
        {
            if (Status == LocationServiceStatus.Stopped)
            {
                Location.Start(desiredAccuracy, updateDistance);
            }

            while (Status != LocationServiceStatus.Running
                && Status != LocationServiceStatus.Failed)
            {
                yield return null;
            }

            if (Status == LocationServiceStatus.Failed)
            {
                Debug.LogError("Could not start the GPS service");
                enabled = false;
            }
        }

        /// <summary>
        /// If the time between reports has elapsed, retrieve a new GPS report, saving the results to
        /// PlayerPrefs and firing the <see cref="onPositionUpdated"/> and <see
        /// cref="PositionUpdated"/> events if one is found.
        /// </summary>
        public void Update()
        {
            if (UseFakeCoord != lastUseFakeCoord)
            {
                UnityInput.compass.enabled = !UseFakeCoord;
            }

            lastUseFakeCoord = UseFakeCoord;

            if (Status == LocationServiceStatus.Running
                && !UseFakeCoord
                && Time.unscaledTime >= nextUpdateTime)
            {
                nextUpdateTime += timeBetweenUpdates;
                var newLocation = Location.lastData;
                if (newLocation.timestamp > lastLocation.timestamp)
                {
                    Coord = new LatLngPoint(newLocation.latitude, newLocation.longitude, newLocation.altitude);
                    PlayerPrefs.SetString(COORD_KEY, coordFactory.ToString(Coord));
                    lastLocation = newLocation;
                    OnPositionUpdated();
                }
            }

            if (DebugReport)
            {
                PrintDebugReport();
            }
        }

        /// <summary>
        /// Prints the internal state of the GPS tracking.
        /// </summary>
        private void PrintDebugReport()
        {
            if (UseFakeCoord)
            {
                ScreenDebugger.Print("  Using fake location");
            }
            else if (!UnityInput.location.isEnabledByUser)
            {
                ScreenDebugger.Print("  User has disabled location services");
            }
            else if (UnityInput.location.status == LocationServiceStatus.Stopped)
            {
                ScreenDebugger.Print("  Starting location services");
            }
            else if (UnityInput.location.status == LocationServiceStatus.Initializing)
            {
                ScreenDebugger.Print("  Initializing location services");
            }
            else if (UnityInput.location.status == LocationServiceStatus.Failed)
            {
                ScreenDebugger.Print("  Unable to determine device location");
            }

            if (!HasCoord)
            {
                ScreenDebugger.Print("  Waiting for location lock");
            }
            else
            {
                if (!UseFakeCoord)
                {
                    var ts = lastLocation.timestamp.UnixTimestampToDateTime();
                    var sinceUpdate = EditableDateTime.Now - ts;
                    ScreenDebugger.PrintFormat("  At [{0:mm\\:ss\\.fff}]: accuracy [+-{1}], altitude {2}",
                        sinceUpdate,
                        lastLocation.horizontalAccuracy.Label(UnitOfMeasure.Meters, 3),
                        lastLocation.altitude.Label(UnitOfMeasure.Meters, 3));
                }

                ScreenDebugger.PrintFormat("  Coordinate: {0}", HasCoord ? Coord.ToDMS(5) : "N/A");
            }
        }
    }
}