using System;
using System.Text.RegularExpressions;

namespace Juniper.Google.Maps.StreetView
{
    public struct PanoID
    {
        public static readonly PanoID Empty = new PanoID(string.Empty);

        private static readonly Regex PANO_PATTERN = new Regex("^[a-zA-Z0-9_\\-]+$", RegexOptions.Compiled);

        public static bool TryParse(string text, out PanoID pano)
        {
            if (PANO_PATTERN.IsMatch(text))
            {
                pano = new PanoID(text);
                return true;
            }
            else
            {
                pano = Empty;
                return false;
            }
        }

        public static PanoID Parse(string text)
        {
            if (TryParse(text, out var pano))
            {
                return pano;
            }
            else
            {
                throw new FormatException("Invalid Panorama ID " + text);
            }
        }

        private readonly string id;

        public PanoID(string id)
        {
            this.id = id;
        }

        public override string ToString()
        {
            return id;
        }

        public static explicit operator string(PanoID value)
        {
            return value.ToString();
        }

        public static explicit operator PanoID(string value)
        {
            return new PanoID(value);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is PanoID pano && Equals(pano)
                || obj is string str && id == str;
        }

        public bool Equals(PanoID pano)
        {
            return pano.id == id;
        }

        public static bool operator ==(PanoID left, PanoID right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PanoID left, PanoID right)
        {
            return !(left == right);
        }
    }
}