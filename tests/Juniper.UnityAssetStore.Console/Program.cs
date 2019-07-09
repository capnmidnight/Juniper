using System.Collections.Generic;
using System.Threading.Tasks;

using Juniper.Json;

namespace Juniper.UnityAssetStore.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var json = new JsonFactory();
            var req = new Requester(json);

            var x = await req.GetTopFreeAssets("6");

            System.Console.ReadLine();
        }
    }
}
