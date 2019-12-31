using System.Threading.Tasks;

namespace Juniper.UnityAssetStore
{
    public static class Program
    {
        public static async Task Main()
        {
            var req = new Requester();
            _ = await req
                .GetTopFreeAssetsAsync("6")
                .ConfigureAwait(false);

            System.Console.ReadLine();
        }
    }
}