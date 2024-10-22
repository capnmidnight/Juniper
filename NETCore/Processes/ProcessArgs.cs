namespace Juniper.Processes;

public static class ProcessArgs
{
    public static Dictionary<string, string> ToArgsDict(this string[] args)
    {
        var dict = new Dictionary<string, string>();
        dict.SetValues(args);
        return dict;
    }

    public static void SetValues(this IDictionary<string, string> dict, params string[] args)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        for (var i = 0; i < args.Length; ++i)
        {
            if (args[i].StartsWith("--", StringComparison.OrdinalIgnoreCase))
            {
                var key = args[i][2..];
                if (i == args.Length - 1
                    || args[i + 1].StartsWith("--", StringComparison.OrdinalIgnoreCase))
                {
                    dict[key] = "True";
                }
                else
                {
                    dict[key] = args[++i];
                }
            }
        }
    }
}
