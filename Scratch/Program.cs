using System;

namespace Scratch
{
    class Program
    {

        static void Main(string[] args)
        {
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(var type in asm.GetTypes())
                {
                    if(type.Name.Contains("nullableattribute", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine($"{asm.FullName} -> {type.FullName}");
                    }
                }
            }
        }
    }
}
