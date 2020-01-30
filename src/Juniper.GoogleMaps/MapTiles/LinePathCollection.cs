using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using static System.Math;

namespace Juniper.World.GIS.Google.MapTiles
{
    public class LinePathCollection : IList<string>
    {
        public LinePathStyle Style { get; }

        public List<string> Points { get; }

        public bool EncodePath { get; }

        public LinePathCollection(LinePathStyle style, bool encodePath, IEnumerable<string> points)
        {
            if (points is null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            Style = style ?? throw new ArgumentNullException(nameof(style));
            EncodePath = encodePath;
            Points = new List<string>(points);

            if (Points.Count < 2)
            {
                throw new ArgumentException("There needs to be at least 2 points in a path for it to be renderable.", nameof(points));
            }
        }

        public LinePathCollection(LinePathStyle style, bool encodePath, params string[] points)
            : this(style, encodePath, points.AsEnumerable()) { }

        public LinePathCollection(LinePathStyle style, bool encodePath, IEnumerable<LatLngPoint> points)
            : this(style, encodePath, points.Select(p => p.ToString(CultureInfo.InvariantCulture))) { }

        public LinePathCollection(LinePathStyle style, bool encodePath, params LatLngPoint[] points)
            : this(style, encodePath, points.AsEnumerable()) { }

        public LinePathCollection(bool encodePath, IEnumerable<string> points)
            : this(default, encodePath, points) { }

        public LinePathCollection(bool encodePath, params string[] points)
            : this(default, encodePath, points) { }

        public LinePathCollection(bool encodePath, IEnumerable<LatLngPoint> points)
            : this(default, encodePath, points) { }

        public LinePathCollection(bool encodePath, params LatLngPoint[] points)
            : this(default, encodePath, points) { }

        public LinePathCollection(LinePathStyle style, IEnumerable<string> points)
            : this(style, false, points) { }

        public LinePathCollection(LinePathStyle style, params string[] points)
            : this(style, false, points) { }

        public LinePathCollection(LinePathStyle style, IEnumerable<LatLngPoint> points)
            : this(style, false, points) { }

        public LinePathCollection(LinePathStyle style, params LatLngPoint[] points)
            : this(style, false, points) { }

        public LinePathCollection(IEnumerable<string> points)
            : this(default, false, points) { }

        public LinePathCollection(params string[] points)
            : this(default, false, points) { }

        public LinePathCollection(IEnumerable<LatLngPoint> points)
            : this(default, false, points) { }

        public LinePathCollection(params LatLngPoint[] points)
            : this(default, false, points) { }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var delim = "";
            if (Style != default)
            {
                _ = sb.Append(Style);
                delim = "|";
            }

            if (EncodePath)
            {
                _ = sb.Append("enc:")
                  .Append(EncodePolyline(Points));
            }
            else
            {
                foreach (var point in Points)
                {
                    _ = sb.Append(delim)
                      .Append(point);
                    delim = "|";
                }
            }

            return sb.ToString();
        }

        public static int EncodePolylinePart(StringBuilder sb, double part, ref int firstPart)
        {
            if (sb is null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

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
                _ = sb.Append((char)dec);
            }

            return val;
        }

        public static string EncodePolyline(IEnumerable<string> points)
        {
            if (points is null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            var sb = new StringBuilder();
            var lastLat = 0;
            var lastLng = 0;
            foreach (var point in points)
            {
                EncodePolylineEntry(sb, point, ref lastLat, ref lastLng);
            }

            return sb.ToString();
        }

        public static void EncodePolylineEntry(StringBuilder sb, string point, ref int firstLat, ref int firstLng)
        {
            var parts = point.SplitX(',');
            var values = parts.Select(p => double.Parse(p.Trim(), CultureInfo.InvariantCulture)).ToArray();
            _ = EncodePolylinePart(sb, values[0], ref firstLat);
            _ = EncodePolylinePart(sb, values[1], ref firstLng);
        }

        public int IndexOf(string item)
        {
            return Points.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            Points.Insert(index, item);
        }

        public void Insert(int index, LatLngPoint item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Points.Insert(index, item.ToString(CultureInfo.InvariantCulture));
        }

        public void RemoveAt(int index)
        {
            Points.RemoveAt(index);
        }

        public string this[int index]
        {
            get { return Points[index]; }
            set { Points[index] = value; }
        }

        public void Add(string item)
        {
            Points.Add(item);
        }

        public void Add(LatLngPoint item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Points.Add(item.ToString(CultureInfo.InvariantCulture));
        }

        public void Clear()
        {
            Points.Clear();
        }

        public bool Contains(string item)
        {
            return Points.Contains(item);
        }

        public bool Contains(LatLngPoint item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return Contains(item.ToString(CultureInfo.InvariantCulture));
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            Points.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            return Points.Remove(item);
        }

        public bool Remove(LatLngPoint item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return Remove(item.ToString(CultureInfo.InvariantCulture));
        }

        public int Count => Points.Count;

        public bool IsReadOnly => ((IList<string>)Points).IsReadOnly;

        public IEnumerator<string> GetEnumerator()
        {
            return ((IList<string>)Points).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<string>)Points).GetEnumerator();
        }
    }
}