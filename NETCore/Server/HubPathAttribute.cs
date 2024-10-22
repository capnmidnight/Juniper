namespace Juniper;

[AttributeUsage(AttributeTargets.Class)]
public class HubPathAttribute : Attribute
{
    public string Path { get; private set; }

    public HubPathAttribute(string path)
    {
        Path = path;
    }
}
