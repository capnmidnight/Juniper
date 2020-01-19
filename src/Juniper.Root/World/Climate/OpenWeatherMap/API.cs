using System;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Juniper.Climate;
using Juniper.IO;
using Juniper.Units;
using Juniper.World.GIS;

namespace Juniper.World.Climate.OpenWeatherMap
{
    /// <summary>
    /// An implementation of OpenWeatherMap's REST API.
    /// </summary>
    public class API : IWeatherAPI
    {
        /// <summary>
        /// The host for the service.
        /// </summary>
        private const string serverURI = "https://api.openweathermap.org";

        /// <summary>
        /// The version number of the service we're targeting.
        /// </summary>
        private static readonly Version version = new Version(2, 5);

        /// <summary>
        /// The directory in which to find the API on the host.
        /// </summary>
        private const string operation = "weather";

        /// <summary>
        /// Units of measure to request from the API.
        /// </summary>
        private const string units = "metric";

        /// <summary>
        /// Factory used to serialize objects for local caching.
        /// </summary>
        private readonly IFactory<WeatherReport, MediaType.Application> weatherFactory;
        private readonly IFactory<WeatherReportException, MediaType.Application> errorFactory;

        /// <summary>
        /// The key to authenticate with the API
        /// </summary>
        private readonly string apiKey;

        /// <summary>
        /// Get the last weather report that was retrieved for the server.
        /// </summary>
        public IWeatherReport LastReport { get; private set; }

        /// <summary>
        /// The amount of time, in minutes, to allow to pass between requesting reports.
        /// </summary>
        public float ReportTTLMinutes { get; set; }

        /// <summary>
        /// The radius, in meters, to allow the user to travel before we request a new report
        /// outside of the normal <see cref="ReportTTLMinutes"/> time frame.
        /// </summary>
        public float ReportRadiusMeters { get; set; }

        /// <summary>
        /// Initialize a new API requester object with the given authentication API key.
        /// </summary>
        /// <param name="factory">Factory used to serialize and deserialize objects.</param>
        /// <param name="apiKey">The OpenWeatherMap API key to use for authentication.</param>
        /// <param name="lastReportJSON">The value of the last report we received, if there was any.</param>
        public API(IFactory<WeatherReport, MediaType.Application> factory, string apiKey, string lastReportJSON)
        {
            weatherFactory = factory;
            errorFactory = new JsonFactory<WeatherReportException>();
            this.apiKey = apiKey;

            if (lastReportJSON is object)
            {
                if (factory.TryParse(lastReportJSON, out var report))
                {
                    LastReport = report;
                }
            }
        }

        public API(IFactory<WeatherReport, MediaType.Application> factory, string apiKey)
            : this(factory, apiKey, null) { }

        /// <summary>
        /// Returns true when <see cref="LastReport"/> is null, LastReport indicates and error
        /// occurred, the time since the last report exceeds the <see cref="ReportTTLMinutes"/>, or
        /// the distance from <paramref name="location"/> to the last report's location is more than
        /// <see cref="ReportRadiusMeters"/>.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private bool NeedsNewReport(LatLngPoint location)
        {
            if (LastReport is null || LastReport.ErrorMessage is object)
            {
                return true;
            }
            else if (LastReport.Location is null)
            {
                return true;
            }
            else
            {
                var dt = DateTime.Now - LastReport.ReportTime;
                var distanceMeters = location.Distance(LastReport.Location);
                return dt.TotalMinutes >= ReportTTLMinutes || distanceMeters >= ReportRadiusMeters;
            }
        }

        /// <summary>
        /// Encapsulates an error response from the API server. Objects of this type are Serializable.
        /// </summary>
        [Serializable]
        public class WeatherReportException : Exception, ISerializable
        {
            private WeatherReportException()
            {
            }

            private WeatherReportException(string message)
                : base(message)
            {
            }

            /// <summary>
            /// Create a new error message from an HTTP response error message.
            /// </summary>
            /// <param name="featureName"></param>
            /// <param name="message"></param>
            public WeatherReportException(string featureName, string message)
                : this($"ERROR [{featureName}]: {message}")
            {
            }

            /// <summary>
            /// Create a new error message from a captured exception
            /// </summary>
            /// <param name="featureName"></param>
            /// <param name="exp"></param>
            public WeatherReportException(string featureName, Exception exp)
                : base(exp.ToShortString(featureName), exp)
            {
            }

            /// <summary>
            /// Deserializes an Error.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
            protected WeatherReportException(SerializationInfo info, StreamingContext context)
                : base(info?.GetString("error") ?? throw new ArgumentNullException(nameof(info)))
            {
            }

            /// <summary>
            /// Serializes the Error.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (info is null)
                {
                    throw new ArgumentNullException(nameof(info));
                }

                info.AddValue("error", Message);
            }
        }

        /// <summary>
        /// Initiate a new request for a weather report at a given location.
        /// </summary>
        /// <param name="location">The current location for which to retrieve a report. Reports need to be at least <see cref="ReportTTLMinutes"/> minutes or <see cref="ReportRadiusMeters"/> meters apart.</param>
        /// <param name="force">Force downloading a new report, regardless of how far we are from the last report location.</param>
        /// <param name="prog">A progress tracker, if any.</param>
        /// <returns></returns>
        public async Task<IWeatherReport> GetWeatherReportAsync(LatLngPoint location, bool force, IProgress prog = null)
        {
            if (location is null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            prog.Report(0);
            if (NeedsNewReport(location) || force)
            {
                var url = new Uri($"{serverURI}/data/{version.ToString(2)}/{operation}?lat={location.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={location.Longitude.ToString(CultureInfo.InvariantCulture)}&units={units}&appid={apiKey}");
                try
                {
                    var requester = HttpWebRequestExt.Create(url);
                    requester.Accept = MediaType.Application.Json;
                    using var response = await requester
                        .GetAsync()
                        .ConfigureAwait(false);
                    if (weatherFactory.TryDeserialize<WeatherReport>(response, out var report))
                    {
                        LastReport = report;
                    }
                    else
                    {
                        throw ErrorReport = new WeatherReportException("GetNewReport", "No response: " + url);
                    }
                }
                catch (Exception exp)
                {
                    throw ErrorReport = new WeatherReportException("GetNewReport", exp.Message + ": " + url);
                }
            }

            prog.Report(1);
            return LastReport;
        }

        private WeatherReportException ErrorReport
        {
            set
            {
                var reportJSON = errorFactory.ToString(value);
                weatherFactory.TryParse(reportJSON, out var errorReport);
                LastReport = errorReport;
            }
        }
    }
}