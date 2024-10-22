namespace Hjg.Pngcs
{
    /// <summary>
    /// Utility functions for C# porting
    /// </summary>
    ///
    internal static class PngCsUtils
    {
#pragma warning disable IDE1006 // Naming Styles
        internal static bool arraysEqual4(byte[] ar1, byte[] ar2)
#pragma warning restore IDE1006 // Naming Styles
        {
            return (ar1[0] == ar2[0]) &&
                   (ar1[1] == ar2[1]) &&
                   (ar1[2] == ar2[2]) &&
                   (ar1[3] == ar2[3]);
        }

#pragma warning disable IDE1006 // Naming Styles
        internal static bool arraysEqual(byte[] a1, byte[] a2)
#pragma warning restore IDE1006 // Naming Styles
        {
            if (a1.Length != a2.Length)
            {
                return false;
            }

            for (var i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}