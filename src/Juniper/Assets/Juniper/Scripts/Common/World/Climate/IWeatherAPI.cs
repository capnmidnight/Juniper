using Juniper.Progress;
using Juniper.World.GIS;

namespace Juniper.World.Climate
{
    public interface IWeatherAPI
    {
        /// <summary>
        /// The amount of time, in minutes, to allow to pass between requesting reports.
        /// </summary>
        float ReportTTLMinutes
        {
            set;
        }

        /// <summary>
        /// The radius, in meters, to allow the user to travel before we request a new reportt
        /// outside of the normal <see cref="ReportTTLMinutes"/> time frame.
        /// </summary>
        float ReportRadiusMeters
        {
            set;
        }

        /// <summary>
        /// Get the last weather report that was retreived for the server.
        /// </summary>
        IWeatherReport LastReport
        {
            get;
        }

        /// <summary>
        /// Initiate a new request for a weather report at a given location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="force"></param>
        /// <param name="prog"></param>
        void Request(LatLngPoint location, bool force, IProgress prog = null);
    }
}
