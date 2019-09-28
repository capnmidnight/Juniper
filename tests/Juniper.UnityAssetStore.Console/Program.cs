using System.Threading.Tasks;

namespace Juniper.UnityAssetStore
{
    internal class Program
    {
        private static async Task Main()
        {
            var req = new Requester();

            var x = await req.GetTopFreeAssets("6");

            System.Console.ReadLine();
        }
    }
}