using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Juniper.Serialization;

using static System.Console;

namespace Juniper.Json.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var inDir = "C:\\Users\\smcbeth.DLS-INC\\Projects\\Yarrow\\StreamingAssets";
            IFactory<Yarrow.YarrowMetadata> json = new JsonFactory().Specialize<Yarrow.YarrowMetadata>();
            var tasks = new List<Task>();
            foreach (var file in Directory.GetFiles(inDir, "*.json"))
            {
                var inFile = file;
                var outFile = Path.Combine("C:\\Users\\smcbeth.DLS-INC\\Desktop", Path.GetFileName(inFile));
                if (File.Exists(outFile))
                {
                    inFile = outFile;
                }
                tasks.Add(Task.Run(async () =>
                {
                    WriteLine("read " + inFile);
                    var meta = json.Load(inFile);
                    meta.navPoints.Add("TESTSTESTSTESETST");
                    WriteLine("write " + outFile);
                    json.Save(outFile, meta);
                }));
            }

            var done = Task.WhenAll(tasks.ToArray());
            done.Wait();
            if (done.IsFaulted)
            {

            }
        }
    }
}
