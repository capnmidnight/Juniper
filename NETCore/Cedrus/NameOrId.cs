namespace Juniper.Cedrus;

public record NameOrId(
    string TypeStamp,
    int? Id = null,
    string? Name = null
);

public static class NameOrIdExt
{
    public static void CheckTypeStamp(this NameOrId[]? inputs, string typeStamp)
    {
        if (inputs is not null && inputs.Any(cv => cv.TypeStamp != typeStamp))
        {
            throw new ArgumentException($"Expected {typeStamp}", nameof(inputs));
        }
    }
    public static void CheckTypeStamp(this NameOrId? input, string typeStamp)
    {
        if (input is not null && input.TypeStamp != typeStamp)
        {
            throw new ArgumentException($"Expected {typeStamp}", nameof(input));
        }
    }

    public static NameOrId[]? OfTypeStamp(this IEnumerable<NameOrId>? input, string typeStamp)
    {
        if (input is null)
        {
            return null;
        }

        var output = input.Where(p => p.TypeStamp == typeStamp).ToArray();
        if (output.Length == 0)
        {
            return null;
        }

        return output;
    }

    public static int[] IDs(this IEnumerable<NameOrId>? input) =>
        input?
            .Where(i => i.Id is not null)
            .Select(i => i.Id!.Value)
            .ToArray()
            ?? [];

    public static string[] Names(this IEnumerable<NameOrId>? input) =>
        input?
            .Where(i => i.Name is not null)
            .Select(i => i.Name!)
            .ToArray()
            ?? [];
}