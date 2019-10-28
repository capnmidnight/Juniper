using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;

using Juniper.Climate;
using Juniper.IO;
using Juniper.Progress;
using Juniper.Security;
using Juniper.Units;
using Juniper.World.GIS;

using UnityEngine;

namespace Juniper.World.Climate
{
    /// <summary>
    /// A component for managing access to weather reports. Only one of these components is allowed
    /// on a gameObject. This component also requires a <see cref="GPSLocation"/> component on the
    /// gameObject. This component executes in the editor.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GPSLocation))]
    public class Weather : MonoBehaviour
#if UNITY_EDITOR
        , ICredentialReceiver
#endif
    {
        /// <summary>
        /// A key for storing the last weather report in the Player Prefs data store.
        /// </summary>
        private const string REPORT_KEY = "JUNIPER_GIS_OPENWEATHERMAP_CACHED_WEATHER_REPORT";

        /// <summary>
        /// Used to compare to the most recent report to see if it needs to be cached.
        /// </summary>
        private string lastReportJSON;

        /// <summary>
        /// The API authorization key for OpenWeatherMap.
        /// </summary>
        [SerializeField]
        [HideInNormalInspector]
        private string owmApiKey;

#if UNITY_EDITOR
        public string CredentialFile
        {
            get
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "openweathermap.txt");
                return keyFile;
            }
        }

        public void ReceiveCredentials(string[] args)
        {
            if (args == null)
            {
                owmApiKey = null;
            }
            else
            {
                owmApiKey = args[0];
            }
        }
#endif

        /// <summary>
        /// The time, in minutes, to wait between requesting weather reports.
        /// </summary>
        [Range(5, 24 * 60)]
        public float MinutesBetweenReports = 15;

        /// <summary>
        /// The distance, in miles, to allow to be traversed away from the location of the previous
        /// weather report to consider the location different enough to warrant a new weather report.
        /// </summary>
        public float MilesBetweenReports = 1;

        /// <summary>
        /// When set to true, the component will not request weather reports from the weather service
        /// and will instead use a static report specified within the Unity Editor.
        /// </summary>
        public bool FakeWeather;

        /// <summary>
        /// The static, fake weather report to use when <see cref="FakeWeather"/> is set to true.
        /// This item has a context menu item "Get weather report now", that calls <see cref="GetReport"/>.
        /// </summary>
        [ContextMenuItem("Get weather report now", "GetReport")]
        public FakeWeatherReport weather;

        /// <summary>
        /// When set to true, the Update loop will include printing out the internal values of the component;
        /// </summary>
        public bool DebugReport;

        /// <summary>
        /// An object on which to wait for completion of the report.
        /// </summary>
        private Task reportTask;

        /// <summary>
        /// If there is a valid <see cref="currentWeather"/> value, returns the SunsetTime from it.
        /// </summary>
        /// <value>The sunset.</value>
        public DateTime? Sunset
        {
            get
            {
                return currentWeather?.SunsetTime;
            }
        }

        /// <summary>
        /// If there is a valid <see cref="currentWeather"/> value, returns the CloudCover from it.
        /// </summary>
        /// <value>The cloud cover.</value>
        public float? CloudCover
        {
            get
            {
                return currentWeather?.CloudCover;
            }
        }

        /// <summary>
        /// If there is a valid <see cref="currentWeather"/> value, returns the WindDirection from it.
        /// </summary>
        /// <value>The wind direction.</value>
        public float? WindDirection
        {
            get
            {
                return currentWeather?.WindDirection;
            }
        }

        /// <summary>
        /// If there is a valid <see cref="currentWeather"/> value, returns the WindSpeed from it.
        /// </summary>
        /// <value>The wind speed.</value>
        public float? WindSpeed
        {
            get
            {
                return currentWeather?.WindSpeed;
            }
        }

        /// <summary>
        /// Retrieves a new weather report, if one is <see cref="Ready"/> to retrieve reports.
        /// </summary>
        public void GetReport()
        {
            if (Ready)
            {
                reportTask = GetReport(true);
            }
        }

        private ISerializer serializer;

        /// <summary>
        /// If the <see cref="MinutesBetweenReports"/> expires, or the user has traveled more than
        /// <see cref="MilesBetweenReports"/> since the last weather report, retrieves a new weather report.
        /// </summary>
        public void Update()
        {
            if (location == null)
            {
                Find.Any(out location);
            }

            if (weatherService == null && !string.IsNullOrEmpty(owmApiKey))
            {
                if (PlayerPrefs.HasKey(REPORT_KEY))
                {
                    lastReportJSON = PlayerPrefs.GetString(REPORT_KEY);
                }
                var json = new JsonFactory();
                serializer = json;
                weatherService = new OpenWeatherMap.API(json, owmApiKey, lastReportJSON);
            }

            if (FakeWeather)
            {
                if (!wasFakeWeather && weather != null)
                {
                    weather.reportTime = EditableDateTime.Now;

                    var gps = GetComponent<GPSLocation>();
                    if (gps != null && gps.HasCoord)
                    {
                        weather.location = gps.Coord;
                    }
                }

                currentWeather = weather;
            }
            else if (Ready)
            {
                weatherService.ReportRadiusMeters = MilesBetweenReports.Convert(UnitOfMeasure.Miles, UnitOfMeasure.Meters);
                weatherService.ReportTTLMinutes = MinutesBetweenReports;
                reportTask = GetReport(false);
            }

            wasFakeWeather = FakeWeather;

            if (DebugReport)
            {
                PrintDebugReport();
            }
        }

        /// <summary>
        /// Indicates whether or not a <see cref="FakeWeather"/> report was used in the last frame,
        /// used to determine when the value of FakeWeather changes.
        /// </summary>
        private bool wasFakeWeather;

        /// <summary>
        /// The weather service from which weather reports are retrieved.
        /// </summary>
        private IWeatherAPI weatherService;

        /// <summary>
        /// The most recent weather report.
        /// </summary>
        private IWeatherReport currentWeather;

        /// <summary>
        /// The location component that tells us where we are located.
        /// </summary>
        private GPSLocation location;

        /// <summary>
        /// Returns true when the component has a <see cref="weatherService"/> and a <see
        /// cref="location"/> value that has a value GPS coordinate.
        /// </summary>
        /// <value><c>true</c> if ready; otherwise, <c>false</c>.</value>
        private bool Ready
        {
            get
            {
                return !reportTask.IsRunning()
                    && location != null
                    && location.HasCoord
                    && weatherService != null;
            }
        }
        /// <summary>
        /// Retrieves a new weather report.
        /// </summary>
        /// <returns>The report coroutine.</returns>
        /// <param name="force">If set to <c>true</c> force.</param>
        private async Task GetReport(bool force, IProgress prog)
        {
            if (Ready)
            {
                await Request(location.Coord, force, prog);
            }

            currentWeather = weatherService.LastReport;
            if (currentWeather != null)
            {
                currentWeather.AtmosphericPressure.MaybeSet(ref weather.atmosphericPressure);
                currentWeather.CloudCover.MaybeSet(ref weather.cloudCover);
                currentWeather.Location.MaybeSet(ref weather.location);
                currentWeather.Humidity.MaybeSet(ref weather.humidity);
                currentWeather.Temperature.MaybeSet(ref weather.temperature);
                currentWeather.AtmosphericVisibility.MaybeSet(ref weather.visibility);
                currentWeather.WindDirection.MaybeSet(ref weather.windDirection);
                currentWeather.WindSpeed.MaybeSet(ref weather.windSpeed);
            }
        }

        private Task GetReport(bool force)
        {
            return GetReport(force, null);
        }

        /// <summary>
        /// Initiate a new request for a weather report at a given location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="force">   </param>
        /// <returns></returns>
        public async Task Request(LatLngPoint location, bool force, IProgress prog)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable || force)
            {
                var report = await weatherService.Request(location, force, prog);
                var reportJSON = serializer.ToString(report);
                if (reportJSON != lastReportJSON)
                {
                    PlayerPrefs.SetString(REPORT_KEY, reportJSON);
                    lastReportJSON = reportJSON;
                }
            }
        }

        public Task Request(LatLngPoint location, bool force)
        {
            return Request(location, force, null);
        }

        /// <summary>
        /// If <see cref="DebugReport"/> is set to true, prints a report that displays the status of
        /// the component and the values it is using for the light estimation.
        /// </summary>
        private void PrintDebugReport()
        {
            if (FakeWeather)
            {
                ScreenDebugger.Print("  Using fake weather report");
            }
            else if (weatherService == null)
            {
                ScreenDebugger.Print("  No weather service");
            }

            if (currentWeather == null)
            {
                ScreenDebugger.Print("  No weather report");
            }
            else if (currentWeather.ErrorMessage != null)
            {
                ScreenDebugger.Print($"  ERROR: {currentWeather.ErrorMessage}");
            }
            else
            {
                ScreenDebugger.Print($"  City: {currentWeather.City}, {currentWeather.Country}:");
                ScreenDebugger.Print($"  Report time: {currentWeather.ReportTime.FixTime()}");
                ScreenDebugger.Print($"  Current time: {EditableDateTime.Now.FixTime()}");
                ScreenDebugger.Print($"  Time since report: {EditableDateTime.Now - currentWeather.ReportTime}");
                ScreenDebugger.Print($"  Conditions: {currentWeather.Conditions}");
                ScreenDebugger.Print($"  Cloud coverage: {currentWeather.CloudCover.Label(UnitOfMeasure.Proportion)}");
                ScreenDebugger.Print($"  Temperature: {currentWeather.Temperature.Label(UnitOfMeasure.Celsius)}");
                ScreenDebugger.Print($"  Humidity: {currentWeather.Humidity.Label(UnitOfMeasure.Proportion)}");
                ScreenDebugger.Print($"  Pressure: {currentWeather.AtmosphericPressure.Label(UnitOfMeasure.Hectopascals)}");
                ScreenDebugger.Print($"  Visibility: {currentWeather.AtmosphericVisibility.Label(UnitOfMeasure.Meters)}");
                ScreenDebugger.Print($"  Wind Direction {WindDirection.Label(UnitOfMeasure.Degrees)}");
                ScreenDebugger.Print($"  Wind Speed {WindSpeed.Label(UnitOfMeasure.MetersPerSecond)}");
                ScreenDebugger.Print($"  Sunrise: {currentWeather.SunriseTime?.FixTime()}");
                ScreenDebugger.Print($"  Sunset: {currentWeather.SunsetTime?.FixTime()}");
            }
        }
    }
}