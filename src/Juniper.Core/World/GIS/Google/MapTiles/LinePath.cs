using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Juniper.World.GIS;

using static System.Math;

namespace Juniper.GIS.Google.MapTiles
{
    public class LinePath : IList<string>
    {
        public readonly LinePathStyle style;
        public readonly List<string> points = new List<string>();
        public readonly bool encodePath;

        public LinePath(LinePathStyle style, bool encodePath, IEnumerable<string> points)
        {
            this.style = style;
            this.encodePath = encodePath;
            this.points.AddRange(points);
            if (this.points.Count < 2)
            {
                throw new ArgumentException(nameof(points), "There needs to be at least 2 points in a path for it to be renderable.");
            }
        }

        public LinePath(LinePathStyle style, bool encodePath, params string[] points)
            : this(style, encodePath, points.AsEnumerable()) { }

        public LinePath(LinePathStyle style, bool encodePath, IEnumerable<LatLngPoint> points)
            : this(style, encodePath, points.Select(p => p.ToString())) { }

        public LinePath(LinePathStyle style, bool encodePath, params LatLngPoint[] points)
            : this(style, encodePath, points.AsEnumerable()) { }

        public LinePath(bool encodePath, IEnumerable<string> points)
            : this(default, encodePath, points) { }

        public LinePath(bool encodePath, params string[] points)
            : this(default, encodePath, points) { }

        public LinePath(bool encodePath, IEnumerable<LatLngPoint> points)
            : this(default, encodePath, points) { }

        public LinePath(bool encodePath, params LatLngPoint[] points)
            : this(default, encodePath, points) { }

        public LinePath(LinePathStyle style, IEnumerable<string> points)
            : this(style, false, points) { }

        public LinePath(LinePathStyle style, params string[] points)
            : this(style, false, points) { }

        public LinePath(LinePathStyle style, IEnumerable<LatLngPoint> points)
            : this(style, false, points) { }

        public LinePath(LinePathStyle style, params LatLngPoint[] points)
            : this(style, false, points) { }

        public LinePath(IEnumerable<string> points)
            : this(default, false, points) { }

        public LinePath(params string[] points)
            : this(default, false, points) { }

        public LinePath(IEnumerable<LatLngPoint> points)
            : this(default, false, points) { }

        public LinePath(params LatLngPoint[] points)
            : this(default, false, points) { }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var delim = "";
            if (style != default)
            {
                sb.Append(style);
                delim = "|";
            }

            if (encodePath)
            {
                sb.Append($"enc:{EncodePolyline(points)}");
            }
            else
            {
                foreach (var point in points)
                {
                    sb.Append($"{delim}{point}");
                    delim = "|";
                }
            }

            return sb.ToString();
        }

        public static int EncodePolylinePart(StringBuilder sb, double part, ref int firstPart)
        {
            var val = (int)Round(part * 1e5);
            var delta = val - firstPart;
            firstPart = val;

            var comp = delta << 1;
            if (part < 0)
            {
                comp = ~comp;
            }

            const int CHUNK_SIZE = 5;
            var binString = Convert.ToString(comp, 2);
            var numGroups = (int)Ceiling((float)binString.Length / CHUNK_SIZE);
            var paddedLen = numGroups * CHUNK_SIZE;
            binString = binString.PadLeft(paddedLen, '0');
            for (var i = numGroups - 1; i >= 0; --i)
            {
                var chunk = binString.Substring(i * CHUNK_SIZE, CHUNK_SIZE);
                var dec = Convert.ToInt32(chunk, 2);
                if (i > 0)
                {
                    dec |= 0x20;
                }
                dec += 63;
                sb.Append((char)dec);
            }

            return val;
        }

        public static string EncodePolyline(IEnumerable<string> points)
        {
            var sb = new StringBuilder();
            int lastLat = 0, lastLng = 0;
            foreach (var point in points)
            {
                EncodePolylineEntry(sb, point, ref lastLat, ref lastLng);
            }

            return sb.ToString();
        }

        public static void EncodePolylineEntry(StringBuilder sb, string point, ref int firstLat, ref int firstLng)
        {
            var parts = point.SplitX(',');
            var values = parts.Select(p => double.Parse(p.Trim())).ToArray();
            EncodePolylinePart(sb, values[0], ref firstLat);
            EncodePolylinePart(sb, values[1], ref firstLng);
        }

        public int IndexOf(string item)
        {
            return points.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            points.Insert(index, item);
        }

        public void Insert(int index, LatLngPoint item)
        {
            points.Insert(index, item.ToString());
        }

        public void RemoveAt(int index)
        {
            points.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return points[index];
            }

            set
            {
                points[index] = value;
            }
        }

        public void Add(string item)
        {
            points.Add(item);
        }

        public void Add(LatLngPoint item)
        {
            points.Add(item.ToString());
        }

        public void Clear()
        {
            points.Clear();
        }

        public bool Contains(string item)
        {
            return points.Contains(item);
        }

        public bool Contains(LatLngPoint item)
        {
            return Contains(item.ToString());
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            points.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            return points.Remove(item);
        }

        public bool Remove(LatLngPoint item)
        {
            return Remove(item.ToString());
        }

        public int Count
        {
            get
            {
                return points.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<string>)points).IsReadOnly;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IList<string>)points).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<string>)points).GetEnumerator();
        }
    }
}