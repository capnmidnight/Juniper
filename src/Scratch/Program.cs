using System;

namespace Scratch
{
    public class Program
    {
        public static void Main()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.Name.Contains("nullableattribute", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine($"{asm.FullName} -> {type.FullName}");
                    }
                }
            }
        }
    }
}
