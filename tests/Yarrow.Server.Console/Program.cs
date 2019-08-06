using Juniper.HTTP;

namespace Yarrow.Server.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var server = new YarrowServer(
                args,
                System.Console.WriteLine,
                System.Console.WriteLine,
                System.Console.Error.WriteLine);
        }
    }
}