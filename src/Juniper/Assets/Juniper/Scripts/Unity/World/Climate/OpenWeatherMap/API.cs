using System;
using System.Runtime.Serialization;
using Juniper.Data;
using Juniper.Progress;
using Juniper.World.GIS;
using Newtonsoft.Json;

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
        /// The key to authenticate with the API
        /// </summary>
        private readonly string apiKey;

        /// <summary>
        /// Get the last weather report that was retreived for the server.
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
        /// The radius, in meters, to allow the user to travel before we request a new reportt
        /// outside of the normal <see cref="ReportTTLMinutes"/> time frame.
        /// </summary>
        public float ReportRadiusMeters
        {
            get; set;
        }

        /// <summary>
        /// Initialize a new API requester object with the given authenticationi API kye.
        /// </summary>
        /// <param name="apiKey">The OpenWeatherMap API key to use for authentication.</param>
        /// <param name="lastReportJSON">The value of the last report we received, if there was any.</param>
        public API(string apiKey, string lastReportJSON = null)
        {
            this.apiKey = apiKey;

            if (lastReportJSON != null)
            {
                LastReport = JsonConvert.DeserializeObject<WeatherReport>(lastReportJSON);
            }
        }

        /// <summary>
        /// Returns true when <see cref="LastReport"/> is null, LastReport indicates and error
        /// occured, the time since the last report exceeds the <see cref="ReportTTLMinutes"/>, or
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
            public Error(SerializationInfo info, StreamingContext context)
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
                string reportJSON = null;

                var url = $"{serverURI}/data/{version.ToString(2)}/{operation}?lat={location.Latitude}&lon={location.Longitude}&units={units}&appid={apiKey}";
                try
                {
                    var results = await HTTP.GetObject<WeatherReport>(url);
                    var httpStatus = results.Status;
                    var report = results.Value;
                    if (report == null)
                    {
                        var errorObj = new Error("GetNewReport", "No response: " + url);
                        reportJSON = JsonConvert.SerializeObject(errorObj);
                        LastReport = JsonConvert.DeserializeObject<WeatherReport>(reportJSON);
                    }
                    else
                    {
                        reportJSON = JsonConvert.SerializeObject(report);
                        LastReport = report;
                    }
                }
                catch (Exception exp)
                {
                    var errorObj = new Error("GetNewReport", exp.Message + ": " + url);
                    reportJSON = JsonConvert.SerializeObject(errorObj);
                    LastReport = JsonConvert.DeserializeObject<WeatherReport>(reportJSON);
                }
            }
            else
            {
                prog?.Report(1);
            }
        }
    }
}
