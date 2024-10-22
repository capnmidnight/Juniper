using Juniper;
using Juniper.Imaging.Ico;

internal class Program
{
    private static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: Expected at least 2 arguments: [...input files] [output file name]");
            return -1;
        }

        var overwrite = args.Any(arg => arg == "-y");

        var inputFiles = args
            .SkipLast(1)
            .Select(arg => new FileInfo(arg))
            .ToArray();

        var notExists = inputFiles.Where(f => !f.Exists);
        if (notExists.Any())
        {
            Console.Error.WriteLine("Error: The following file(s) do not exist: {0}", notExists.Select(f => f.FullName).ToArray().Join(", "));
            return -1;
        }

        var here = new DirectoryInfo(Directory.GetCurrentDirectory());
        var outputFile = here.Touch(args.Last()).AddExtension(MediaType.Image_Vendor_MicrosoftIcon);

        var writeFile = !outputFile.Exists;
        if (!writeFile)
        {
            writeFile = overwrite;
            if (overwrite)
            {
                Console.WriteLine("Output file {0} already exists. Auto-overwrite specified.", outputFile.FullName);
            }
            else
            {
                Console.Write("Output file {0} aleady exists. Do you want to want to overwrite? (Y/n)", outputFile.FullName);
                var response = Console.ReadLine();
                if (response?.Trim().ToLower() != "n")
                {
                    writeFile = true;
                }
            }
        }

        if (writeFile)
        {
            Ico.Concatenate(outputFile, inputFiles);
            Console.WriteLine("Output written to {0}", outputFile.FullName);
        }
        else
        {
            Console.WriteLine("Operation cancelled");
        }

        return 0;
    }
}