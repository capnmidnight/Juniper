namespace Juniper;

internal class Entry
{
    public Group Group { get; }

    public string Value { get; }

    public string FieldName { get; }

    public string DeprecationMessage { get; }

    public string[] Extensions { get; }

    public string PrimaryExtension { get; }

    public Entry(Group group, string fieldName, string value, string deprecationMessage, string[] extensions)
    {
        Group = group;
        Value = value;
        FieldName = fieldName;
        DeprecationMessage = deprecationMessage;
        Extensions = extensions;
        if (extensions?.Length >= 1)
        {
            PrimaryExtension = extensions[0];
        }
    }

    public void Write(TextWriter writer)
    {
        if (!string.IsNullOrEmpty(DeprecationMessage))
        {
            writer.WriteLine();
            writer.WriteLine("            [System.Obsolete(\"{0}\")]", DeprecationMessage);
        }

        var slashIndex = Value.IndexOf('/');
        var shortName = Value.Substring(slashIndex + 1);
        writer.Write("            public static readonly {0} {1} = new {0}(\"{2}\"", Group.ClassName, FieldName, shortName);
        if (Extensions != null)
        {
            writer.Write(", new string[] {{ {0} }}", string.Join(", ", Extensions.Select(e => $"\"{e}\"")));
        }

        writer.WriteLine(");");

        if (DeprecationMessage != null)
        {
            writer.WriteLine();
        }
    }
}