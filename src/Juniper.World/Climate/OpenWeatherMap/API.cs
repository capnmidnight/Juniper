using System;
using System.Net;
using System.Runtime.Serialization;
using Juniper.Climate;
using Juniper.Progress;
using Juniper.Serialization;
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
        private readonly ISerializer serializer;

        /// <summary>
        /// factory used to deserialize objects from the data stream.
        /// </summary>
        private readonly IDeserializer deserializer;

        /// <summary>
        /// The key to authenticate with the API
        /// </summary>
        private readonly string apiKey;

        /// <summary>
        /// Get the last weather report that was retrieved for the server.
        /// </summary>
        public IWeatherReport LastReport
        {
            get; private set;
        }

        /// <summary>
        /// The amount of time, in minutes, to allow to pass between requesting reports.
        /// </summary>
        public float ReportTTLMinutes
        {
            get; set;
        }

        /// <summary>
        /// The radius, in meters, to allow the user to travel before we request a new report
        /// outside of the normal <see cref="ReportTTLMinutes"/> time frame.
        /// </summary>
        public float ReportRadiusMeters
        {
            get; set;
        }

        /// <summary>
        /// Initialize a new API requester object with the given authentication API key.
        /// </summary>
        /// <param name="serializer">Factory used to serialize objects.</param>
        /// <param name="deserializer">The factory used for deserializing objects.</param>
        /// <param name="apiKey">The OpenWeatherMap API key to use for authentication.</param>
        /// <param name="lastReportJSON">The value of the last report we received, if there was any.</param>
        public API(ISerializer serializer, IDeserializer deserializer, string apiKey, string lastReportJSON = null)
        {
            this.serializer = serializer;
            this.deserializer = deserializer;
            this.apiKey = apiKey;

            if (lastReportJSON != null)
            {
                if (deserializer.TryParse<WeatherReport>(lastReportJSON, out var report))
                {
                    LastReport = report;
                }
            }
        }

        /// <summary>
        /// Initialize a new API requester object with the given authentication API key.
        /// </summary>
        /// <param name="factory">Factory used to serialize and deserialize objects.</param>
        /// <param name="apiKey">The OpenWeatherMap API key to use for authentication.</param>
        /// <param name="lastReportJSON">The value of the last report we received, if there was any.</param>
        public API(IFactory factory, string apiKey, string lastReportJSON = null)
            : this(factory, factory, apiKey, lastReportJSON)
        {
        }

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
            if (LastReport == null || LastReport.ErrorMessage != null)
            {
                return true;
            }
            else if (LastReport.Location == null)
            {
                return true;
            }
            else
            {
                var dt = DateTime.Now - LastReport.ReportTime;
                var distanceMeters = location.Distance(LastReport.Location.Value);
                return dt.TotalMinutes >= ReportTTLMinutes || distanceMeters >= ReportRadiusMeters;
            }
        }

        /// <summary>
        /// Encapsulates an error response from the API server. Objects of this type are Serializable.
        /// </summary>
        [Serializable]
        private class Error : ISerializable
        {
            /// <summary>
            /// An error message that is hopefully easier to read than a full stack trace.
            /// </summary>
            public readonly string error;

            /// <summary>
            /// Create a new error message from a captured exception
            /// </summary>
            /// <param name="featureName"></param>
            /// <param name="exp"></param>
            public Error(string featureName, Exception exp)
            {
                error = exp.ToShortString(featureName);
            }

            /// <summary>
            /// Create a new error message from an HTTP response error message.
            /// </summary>
            /// <param name="featureName"></param>
            /// <param name="message"></param>
            public Error(string featureName, string message)
            {
                error = $"ERROR [{featureName}]: {message}";
            }

            /// <summary>
            /// Deserializes an Error.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            protected Error(SerializationInfo info, StreamingContext context)
            {
                error = info.GetString(nameof(error));
            }

            /// <summary>
            /// Serializes the Error.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(error), error);
            }
        }

        /// <summary>
        /// Initiate a new request for a weather report at a given location.
        /// </summary>
        /// <param name="location">The current location for which to retrieve a report. Reports need to be at least <see cref="ReportTTLMinutes"/> minutes or <see cref="ReportRadiusMeters"/> meters apart.</param>
        /// <param name="force">Force downloading a new report, regardless of how far we are from the last report location.</param>
        /// <param name="prog">A progress tracker, if any.</param>
        /// <returns></returns>
        public async void Request(LatLngPoint location, bool force, IProgress prog = null)
        {
            if (NeedsNewReport(location) || force)
            {
                string reportJSON;

                var url = $"{serverURI}/data/{version.ToString(2)}/{operation}?lat={location.Latitude}&lon={location.Longitude}&units={units}&appid={apiKey}";
                try
                {
                    var requester = HttpWebRequestExt.Create(url);
                    requester.Accept = "application/json";
                    using (var response = await requester.Get())
                    {
                        if(deserializer.TryDeserialize<WeatherReport>(response, out var report))
                        {
                            reportJSON = serializer.Serialize(report);
                            LastReport = report;
                        }
                        else
                        {
                            ErrorReport = new Error("GetNewReport", "No response: " + url);
                        }
                    }
                }
                catch (Exception exp)
                {
                    ErrorReport = new Error("GetNewReport", exp.Message + ": " + url);
                    throw;
                }
            }
            else
            {
                prog?.Report(1);
            }
        }

        private Error ErrorReport
        {
            set
            {
                var reportJSON = serializer.Serialize(value);
                deserializer.TryParse<WeatherReport>(reportJSON, out var errorReport);
                LastReport = errorReport;
            }
        }
    }
}