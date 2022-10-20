using System.Globalization;
using UnityEngine;

namespace VladislavTsurikov.Extensions
{
    public static class ColorExtensions 
    {
        public static Color ColorFrom256(this Color color, float r, float g, float b, float a = 255) { return new Color(r / 255f, g / 255f, b / 255f, a / 255f); }
        public static Color WithAlpha(this Color color, float alpha) { return new Color(color.r, color.g, color.b, alpha); }
        public static Color FromHex(this Color color, string hexValue, float alpha = 1)
        {
            if (string.IsNullOrEmpty(hexValue)) return Color.clear;

            if (hexValue[0] == '#') hexValue = hexValue.TrimStart('#');
            if (hexValue.Length > 6) hexValue = hexValue.Remove(6, hexValue.Length - 6);

            int value = int.Parse(hexValue, NumberStyles.HexNumber);
            int r = value >> 16 & 255;
            int g = value >> 8 & 255;
            int b = value & 255;
            float a = 255 * alpha;

            return new Color().ColorFrom256(r, g, b, a);
        }
    }
}

