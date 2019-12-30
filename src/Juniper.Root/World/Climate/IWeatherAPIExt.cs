using System.Threading.Tasks;

using Juniper.Progress;
using Juniper.World.GIS;

namespace Juniper.Climate
{

    public static class IWeatherAPIExt
    {
        public static Task<IWeatherReport> Request(this IWeatherAPI report, LatLngPoint location, bool force)
        {
            return report.GetWeatherReportAsync(location, force, null);
        }
    }
}