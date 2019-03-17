using Juniper.Progress;
using Juniper.World.GIS;

using Newtonsoft.Json;

using System;
using System.Runtime.Serialization;

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
        /// The server response from the last report, to be used to compare with the next report and
        /// quickly determine if the report has changed.
        /// </summary>
        private readonly string lastReportJSON;

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
        /// <param name="apiKey"></param>
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
            if (LastReport == null || LastReport.Error != null)
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
            public Error(SerializationInfo info, StreamingContext context)
            {
                error = info.GetString(nameof(error));
            }

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

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(error), error);
            }
        }

        /// <summary>
        /// Initiate a new request for a weather report at a given location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public IProgress Request(LatLngPoint location, bool force, IProgressReceiver prog = null)
        {
            var subProgress = prog.Split(1)[0];
            if (NeedsNewReport(location) || force)
            {
                string reportJSON = null;

                var url = $"{serverURI}/data/{version.ToString(2)}/{operation}?lat={location.Latitude}&lon={location.Longitude}&units={units}&appid={apiKey}";
                HTTP.GetObject<WeatherReport>(
                    url,
                    report =>
                    {
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
                    },
                    exp =>
                    {
                        var errorObj = new Error("GetNewReport", exp.Message + ": " + url);
                        reportJSON = JsonConvert.SerializeObject(errorObj);
                        LastReport = JsonConvert.DeserializeObject<WeatherReport>(reportJSON);
                    },
                    subProgress);
            }

            return subProgress;
        }
    }
}
