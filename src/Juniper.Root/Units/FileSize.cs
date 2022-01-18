using Accord.Math;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juniper.Units
{
    public static class FileSize
    {
        private static readonly Dictionary<SystemOfMeasure, UnitOfMeasure[]> systemLevels = new Dictionary<SystemOfMeasure, UnitOfMeasure[]>(2)
        {
            [SystemOfMeasure.Metric] = new[] {
                UnitOfMeasure.Bytes,
                UnitOfMeasure.Kilobytes,
                UnitOfMeasure.Megabytes,
                UnitOfMeasure.Gigabytes,
                UnitOfMeasure.Terabytes,
                UnitOfMeasure.Petabytes,
                UnitOfMeasure.Exabytes,
                UnitOfMeasure.Zettabytes,
                UnitOfMeasure.Yotabytes
            },
            [SystemOfMeasure.USCustomary] = new[] {
                UnitOfMeasure.Bytes,
                UnitOfMeasure.Kibibytes,
                UnitOfMeasure.Mibibytes,
                UnitOfMeasure.Gibibytes,
                UnitOfMeasure.Tebibytes,
                UnitOfMeasure.Pebibytes,
                UnitOfMeasure.Exbibytes,
                UnitOfMeasure.Zebibytes,
                UnitOfMeasure.Yobibytes
            }
        };

        public static string Format(long value, UnitOfMeasure units = UnitOfMeasure.Bytes)
        {
            var isNegative = value < 0;
            value = Math.Abs(value);
            var str = Format((ulong)value, units);
            if (isNegative)
            {
                str = System.Globalization.NumberFormatInfo.CurrentInfo.NegativeSign + str;
            }

            return str;
        }

        public static string Format(ulong value, UnitOfMeasure units = UnitOfMeasure.Bytes)
        {
            if (value < 1)
            {
                return "0";
            }
            else
            {
                var system = (from kv in systemLevels
                              where kv.Value.Contains(units)
                              select (SystemOfMeasure?)kv.Key)
                            .FirstOrDefault();
                if (system is null)
                {
                    throw new ArgumentException("Units parameter must be a file type. Given: " + units, nameof(units));
                }

                var systemBase = system == SystemOfMeasure.Metric ? 1000 : 1024;
                var deltaSize = (int)(Math.Log(value) / Math.Log(systemBase));
                var divisor = Math.Pow(systemBase, deltaSize);
                if (2 * value >= systemBase * divisor)
                {
                    deltaSize += 1;
                }

                var levels = systemLevels[system.Value];
                var curSize = levels.IndexOf(units);
                var endSize = Math.Min(curSize + deltaSize, levels.Length - 1);
                var convertTo = levels[endSize];

                return Converter.Label(value, units, convertTo);
            }
        }
    }
}
