using System.Text.RegularExpressions;

namespace System.Collections.Generic;

/// <summary>
/// Sorts a collection of strings, by first splitting out any numbers and treating them
/// as numbers, rather than lexicographic strings.
/// </summary>
/// <example><code><![CDATA[
/// var files = new List<string> { "file003", "file1", "file04", "file02" };
/// files.Sort(new NatrualSortComparer());
/// files.Matches(new []{ "file1", "file02", "file003", "file04" }); // --> true
/// ]]></code></example>
public class NaturalSortComparer : IComparer<string>
{
    /// <summary>
    /// Creates a new NaturalSortComparer.
    /// </summary>
    /// <param name="inAscendingOrder">
    /// Set to false to reverse the order of the sort. Defaults to true.
    /// </param>
    public NaturalSortComparer(bool inAscendingOrder = true)
    {
        isAscending = inAscendingOrder;
    }

    private static readonly Regex SplitNums = new("([0-9]+)", RegexOptions.Compiled);

    /// <summary>
    /// Compare two strings to see where they should be in relation to each other within a collection.
    /// </summary>
    /// <param name="x">The left string.</param>
    /// <param name="y">The right string.</param>
    /// <returns>An integer representing the direction the in which to move the items.</returns>
    public int Compare(string? x, string? y)
    {
        if (x is null)
        {
            return y is null
                ? 0
                : 1;
        }
        else if (y is null)
        {
            return -1;
        }

        if (x == y)
        {
            return 0;
        }

        if (!table.TryGetValue(x, out var x1))
        {
            x1 = SplitNums.Split(x.Replace(" ", ""));
            table.Add(x, x1);
        }

        if (!table.TryGetValue(y, out var y1))
        {
            y1 = SplitNums.Split(y.Replace(" ", ""));
            table.Add(y, y1);
        }

        int returnVal;

        for (var i = 0; i < x1.Length && i < y1.Length; i++)
        {
            if (x1[i] != y1[i])
            {
                returnVal = PartCompare(x1[i], y1[i]);
                return isAscending ? returnVal : -returnVal;
            }
        }

        if (y1.Length > x1.Length)
        {
            returnVal = 1;
        }
        else if (x1.Length > y1.Length)
        {
            returnVal = -1;
        }
        else
        {
            returnVal = 0;
        }

        return isAscending ? returnVal : -returnVal;
    }

    /// <summary> Finish implementing the IComparer<T> interface. </summary> <returns>The
    /// collections. generic. IC omparer< system. string>. compare.</returns> <param name="x">The
    /// x coordinate.</param> <param name="y">The y coordinate.</param>
    int IComparer<string>.Compare(string? x, string? y)
    {
        return Compare(x, y);
    }

    /// <summary>
    /// Whether or not to sort in ascending or descending order.
    /// </summary>
    private readonly bool isAscending;

    /// <summary>
    /// Used to memoize strings to their split representation.
    /// </summary>
    private readonly Dictionary<string, string[]> table = new(5);

    /// <summary>
    /// Compares two subparts of a string to see if they are numbers.
    /// </summary>
    /// <param name="left">The subpart of the first string.</param>
    /// <param name="right">The subpart of the second string.</param>
    /// <returns>A comparison shift amount, usable for sorting algorithms.</returns>
    private static int PartCompare(string left, string right)
    {
        if (!int.TryParse(left, out var x)
            || !int.TryParse(right, out var y))
        {
            return string.CompareOrdinal(left, right);
        }
        else
        {
            return x.CompareTo(y);
        }
    }
}