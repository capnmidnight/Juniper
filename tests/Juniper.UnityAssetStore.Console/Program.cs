using System.Threading.Tasks;

using Juniper.Json;

namespace Juniper.UnityAssetStore.Console
{
    internal class Program
    {
        private static async Task Main()
        {
            var json = new Factory();
            var req = new Requester(json);

            var x = await req.GetTopFreeAssets("6");

            System.Console.ReadLine();
        }
    }
}