using System;
using System.IO;

namespace File_Copier
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Command expects two arguments");
                for(int i = 0; i < args.Length; ++i)
                {
                    Console.Error.WriteLine("{0}: {1}", i, args[i]);
                }
            }
            else
            {
                for(int i = 0; i < args.Length; ++i)
                {
                    if(args[i].EndsWith("\"")
                        || args[i].EndsWith("'"))
                    {
                        args[i] = args[i].Substring(1, args[i].Length - 2);
                    }
                }

                var source = new DirectoryInfo(args[0]);
                var dest1 = new DirectoryInfo(args[1]);
                if (!source.Exists)
                {
                    Console.Error.WriteLine("Source directory does not exist");
                }
                else
                {
                    var dest2 = dest1.Parent;

                    Console.WriteLine("Copying from {0} to {1}", source.FullName, dest1.FullName);

                    foreach(var file in source.GetFiles())
                    {
                        var dest = file.Name.StartsWith("Juniper")
                            ? dest1
                            : dest2;

                        if (file.Extension != ".pdb"
                            || dest.Name.EndsWith("Debug"))
                        {
                            dest.Create();

                            var destFileName = Path.Combine(
                                dest.FullName,
                                file.Name);

                            Console.WriteLine(destFileName);
                            file.CopyTo(destFileName, true);
                        }
                    }
                }
            }
        }
    }
}
