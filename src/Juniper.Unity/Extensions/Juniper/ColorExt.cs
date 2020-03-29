using JuniperColor = Juniper.Color;
using UnityColor = UnityEngine.Color;

namespace Juniper
{
    public static class ColorExt
    {
        public static UnityColor ToUnityColor(this JuniperColor color)
        {
            color = color.ConvertTo(ColorSpace.RGB);
            return new UnityColor(color.X, color.Y, color.Z);
        }

        public static JuniperColor ToJuniperColor(this UnityColor color)
        {
            return new JuniperColor(
                color.r,
                color.g,
                color.b,
                ColorSpace.RGB);
        }
    }
}
