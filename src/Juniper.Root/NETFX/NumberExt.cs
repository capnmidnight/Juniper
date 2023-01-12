using System.Numerics;
using System.Text;

using static System.Math;

namespace System
{
    /// <summary>
    /// Extension methods for different Number types.
    /// </summary>
    public static class NumberExt
    {
        private static void AppendSigFig(float value, int numDigits, StringBuilder output)
        {
            if (numDigits < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numDigits), "Must specify at least 1 significant digit.");
            }

            if (value == 0)
            {
                output.Append('0');
            }
            else if (float.IsPositiveInfinity(value))
            {
                output.Append('∞');
            }
            else if (float.IsNegativeInfinity(value))
            {
                output.Append("-∞");
            }
            else
            {
                if (value < 0)
                {
                    output.Append('-');
                    value = -value;
                }

                var numLeftDigits = (int)Log10(value) + 1;

                var numRightDigits = numDigits - numLeftDigits;

                var clampingFactor = Pow(10, numRightDigits);
                var clampedValue = Round(value * clampingFactor) / clampingFactor;
                output.Append(clampedValue);

                if (numRightDigits > 0)
                {
                    var indexOfDecimal = -1;

                    for (var i = 0; i < output.Length && indexOfDecimal == -1; ++i)
                    {
                        if (output[i] == '.')
                        {
                            indexOfDecimal = i;
                        }
                    }

                    if (indexOfDecimal == -1)
                    {
                        output.Append('.');
                        indexOfDecimal = output.Length - 1;
                    }

                    var totalStringLength = numDigits + indexOfDecimal;
                    while (output.Length < totalStringLength)
                    {
                        output.Append('0');
                    }
                }
            }
        }
        private static void AppendSigFig(double value, int numDigits, StringBuilder output)
        {
            if (numDigits < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numDigits), "Must specify at least 1 significant digit.");
            }

            if (value == 0)
            {
                output.Append('0');
            }
            else if (double.IsPositiveInfinity(value))
            {
                output.Append('∞');
            }
            else if (double.IsNegativeInfinity(value))
            {
                output.Append("-∞");
            }
            else
            {
                if (value < 0)
                {
                    output.Append('-');
                    value = -value;
                }

                var numLeftDigits = (int)Log10(value) + 1;

                var numRightDigits = numDigits - numLeftDigits;

                var clampingFactor = Pow(10, numRightDigits);
                var clampedValue = Round(value * clampingFactor) / clampingFactor;
                output.Append(clampedValue);

                if (numRightDigits > 0)
                {
                    var indexOfDecimal = -1;

                    for (var i = 0; i < output.Length && indexOfDecimal == -1; ++i)
                    {
                        if (output[i] == '.')
                        {
                            indexOfDecimal = i;
                        }
                    }

                    if (indexOfDecimal == -1)
                    {
                        output.Append('.');
                        indexOfDecimal = output.Length - 1;
                    }

                    var totalStringLength = numDigits + indexOfDecimal;
                    while (output.Length < totalStringLength)
                    {
                        output.Append('0');
                    }
                }
            }
        }

        /// <summary>
        /// Print a number to a string with the proper number of significant digits (not just number
        /// of digits after the decimal). In other words, `2345` printed to 3 significant digits will
        /// print as `2350`.
        /// </summary>
        /// <param name="value">The number to format.</param>
        /// <param name="numDigits">The number of significant digits to print.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">When numDigits is less than 1.</exception>
        public static string SigFig(this float value, int numDigits)
        {
            var output = new StringBuilder();
            AppendSigFig(value, numDigits, output);
            return output.ToString();
        }

        /// <summary>
        /// Print a number to a string with the proper number of significant digits (not just number
        /// of digits after the decimal). In other words, `2345` printed to 3 significant digits will
        /// print as `2350`.
        /// </summary>
        /// <param name="value">The number to format.</param>
        /// <param name="numDigits">The number of significant digits to print.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">When numDigits is less than 1.</exception>
        public static string SigFig(this double value, int numDigits)
        {
            var output = new StringBuilder();
            AppendSigFig(value, numDigits, output);
            return output.ToString();
        }

        /// <summary>
        /// Print a number to a string with the proper number of significant digits (not just number
        /// of digits after the decimal). In other words, `2345` printed to 3 significant digits will
        /// print as `2350`.
        /// </summary>
        /// <param name="value">The number to format.</param>
        /// <param name="numDigits">The number of significant digits to print.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">When numDigits is less than 1.</exception>
        public static string SigFig(this Vector2 value, int numDigits)
        {
            var output = new StringBuilder();
            output.Append('(');
            AppendSigFig(value.X, numDigits, output);
            output.Append(", ");
            AppendSigFig(value.Y, numDigits, output);
            output.Append(')');
            return output.ToString();
        }

        /// <summary>
        /// Print a number to a string with the proper number of significant digits (not just number
        /// of digits after the decimal). In other words, `2345` printed to 3 significant digits will
        /// print as `2350`.
        /// </summary>
        /// <param name="value">The number to format.</param>
        /// <param name="numDigits">The number of significant digits to print.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">When numDigits is less than 1.</exception>
        public static string SigFig(this Vector3 value, int numDigits)
        {
            var output = new StringBuilder();
            output.Append('(');
            AppendSigFig(value.X, numDigits, output);
            output.Append(", ");
            AppendSigFig(value.Y, numDigits, output);
            output.Append(", ");
            AppendSigFig(value.Z, numDigits, output);
            output.Append(')');
            return output.ToString();
        }

        /// <summary>
        /// Print a number to a string with the proper number of significant digits (not just number
        /// of digits after the decimal). In other words, `2345` printed to 3 significant digits will
        /// print as `2350`.
        /// </summary>
        /// <param name="value">The number to format.</param>
        /// <param name="numDigits">The number of significant digits to print.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">When numDigits is less than 1.</exception>
        public static string SigFig(this Vector4 value, int numDigits)
        {
            var output = new StringBuilder();
            output.Append('(');
            AppendSigFig(value.X, numDigits, output);
            output.Append(", ");
            AppendSigFig(value.Y, numDigits, output);
            output.Append(", ");
            AppendSigFig(value.Z, numDigits, output);
            output.Append(", ");
            AppendSigFig(value.W, numDigits, output);
            output.Append(')');
            return output.ToString();
        }

        /// <summary>
        /// Print a nullable number to a string with the proper number of significant digits (not just
        /// number of digits after the decimal). In other words, `2345` printed to 3 significant digits
        /// will print as `2350`.
        /// </summary>
        /// <param name="value">The number to format.</param>
        /// <param name="numDigits">The number of significant digits to print.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">When numDigits is less than 1.</exception>
        public static string SigFig(this float? value, int numDigits)
        {
            return value?.SigFig(numDigits);
        }

        /// <summary>
        /// Print a nullable number to a string with the proper number of significant digits (not just
        /// number of digits after the decimal). In other words, `2345` printed to 3 significant digits
        /// will print as `2350`.
        /// </summary>
        /// <param name="value">The number to format.</param>
        /// <param name="numDigits">The number of significant digits to print.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">When numDigits is less than 1.</exception>
        public static string SigFig(this Vector2? value, int numDigits)
        {
            return value?.SigFig(numDigits);
        }

        /// <summary>
        /// Print a nullable number to a string with the proper number of significant digits (not just
        /// number of digits after the decimal). In other words, `2345` printed to 3 significant digits
        /// will print as `2350`.
        /// </summary>
        /// <param name="value">The number to format.</param>
        /// <param name="numDigits">The number of significant digits to print.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">When numDigits is less than 1.</exception>
        public static string SigFig(this Vector3? value, int numDigits)
        {
            return value?.SigFig(numDigits);
        }

        /// <summary>
        /// Print a nullable number to a string with the proper number of significant digits (not just
        /// number of digits after the decimal). In other words, `2345` printed to 3 significant digits
        /// will print as `2350`.
        /// </summary>
        /// <param name="value">The number to format.</param>
        /// <param name="numDigits">The number of significant digits to print.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">When numDigits is less than 1.</exception>
        public static string SigFig(this Vector4? value, int numDigits)
        {
            return value?.SigFig(numDigits);
        }

        /// <summary>
        /// Fit an integer value into an Enumeration range.
        /// </summary>
        /// <typeparam name="T">The enumeration range to fit.</typeparam>
        /// <param name="value">The value to clamp</param>
        /// <returns>The integer cast to the enum type <typeparamref name="T"/>.</returns>
        public static T ClampTo<T>(this int value) where T : struct
        {
            var values = Enum.GetValues(typeof(T));

            var ints = values.Cast<int>().ToArray();

            var min = ints.Min();
            var max = ints.Max();

            value = Max(min, Min(max, value));
            var realValue = default(T);
            for (var i = 0; i < values.Length; ++i)
            {
                if (ints[i] == value)
                {
                    realValue = (T)values.GetValue(i);
                }
            }

            return realValue;
        }

        public static int Clamp(this decimal value)
        {
            if (value < 0)
            {
                return -1;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static int Clamp(this double value)
        {
            if (value < 0)
            {
                return -1;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static int Clamp(this float value)
        {
            if (value < 0)
            {
                return -1;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static int Clamp(this long value)
        {
            if (value < 0)
            {
                return -1;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static int Clamp(this int value)
        {
            if (value < 0)
            {
                return -1;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static int Clamp(this short value)
        {
            if (value < 0)
            {
                return -1;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Maximum amount of difference between two floats to consider them the "same" number.
        /// </summary>
        public const float ALPHA = 1e-7f;

        /// <summary>
        /// Determines if two nullable values differ significantly from each other.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if only <paramref name="here"/> or <paramref name="there"/> were null, or if
        /// they are both not null and their values differ by more than ALPHA. <c>false</c> otherwise.
        /// </returns>
        /// <param name="here">Here.</param>
        /// <param name="there">There.</param>
        public static bool DiffersFrom(this float? here, float? there)
        {
            return here.HasValue != there.HasValue
                || (here is not null
                    && there is not null
                    && Abs(there.Value - here.Value) > ALPHA);
        }

        /// <summary>
        /// Loops a nullable circular value around an range.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static float? Repeat(this float? value, float range)
        {
            if (value is null)
            {
                return null;
            }
            else
            {
                return value.Value.Repeat(range);
            }
        }

        /// <summary>
        /// Loops a circular value around an range. Unity has this function already, but we
        /// re-implement it here to make sure the Units subsystem does not depend on Unity.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static float Repeat(this float value, float range)
        {
            while (value < 0)
            {
                value += range;
            }

            while (value > range)
            {
                value -= range;
            }

            return value;
        }

        /// <summary>
        /// Loops a circular value around an range. Unity has this function already, but we
        /// re-implement it here to make sure the Units subsystem does not depend on Unity.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static double Repeat(this double value, double range)
        {
            while (value < 0)
            {
                value += range;
            }

            while (value > range)
            {
                value -= range;
            }

            return value;
        }
    }
}