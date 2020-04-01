using UnityEngine;

namespace Juniper
{
    public static class ColorExt
    {
        public static readonly Color TransparentBlack = new Color(0, 0, 0, 0);

        public static Color ToColor(this Color3 color)
        {
            color = color.ConvertTo(ColorSpace.RGB);
            return new Color(color.X, color.Y, color.Z);
        }

        public static Color3 ToColor3(this Color color)
        {
            return new Color3(
                color.r,
                color.g,
                color.b,
                ColorSpace.RGB);
        }
    }
}
