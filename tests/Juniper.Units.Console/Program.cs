using System.Reflection;

namespace Juniper.Console
{
    internal class Program
    {
        private static void Main()
        {
            var classes = typeof(Units.Converter).GetTypeInfo().Assembly.GetTypes();
            foreach (var c in classes)
            {
                System.Console.WriteLine("            {{ UnitOfMeasure.{0}, new Dictionary<UnitOfMeasure, Func<float, float>>() {{", c.Name);
                var methods = c.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (var m in methods)
                {
                    var parameters = m.GetParameters();
                    if (m.ReturnType == typeof(float) && parameters.Length == 1 && parameters[0].ParameterType == typeof(float))
                    {
                        System.Console.WriteLine("                {{ UnitOfMeasure.{1}, {0}.{1} }},", c.Name, m.Name);
                    }
                }
                System.Console.WriteLine("            } },");
            }
        }
    }
}
