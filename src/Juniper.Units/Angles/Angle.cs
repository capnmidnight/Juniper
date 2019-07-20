using System;

namespace Juniper.Units
{
    /// <summary>
    /// Represents an angle in degrees that doesn't wrap around to 0 when iterating past 360 degrees.
    /// It does this by making the update the smallest delta of value plus whole rotations. e.g. if
    /// the current value is 359, setting the value to 1 will actually return as 361, as <c>Abs(361 -
    /// 351) &lt; Abs(1 - 351)</c>.
    /// </summary>
    public struct Angle
    {
        /// <summary>
        /// Create an angle value at set starting point
        /// </summary>
        /// <param name="v"></param>
        /// <param name="r"></param>
        /// <remarks>
        /// You should try to instantiate the Angle at a value that is close to the first
        /// runtime-value you would receive. The system does not work well for values that oscillate
        /// over ~180 degree changes.
        /// </remarks>
        public Angle(float v, float r)
        {
            currentValue = v;
            rotations = r;
        }

        /// <summary>
        /// Create an angle value at set starting point
        /// </summary>
        /// <param name="v"></param>
        /// <param name="r"></param>
        /// <remarks>
        /// You should try to instantiate the Angle at a value that is close to the first
        /// runtime-value you would receive. The system does not work well for values that oscillate
        /// over ~180 degree changes.
        /// </remarks>
        public Angle(float v) : this(v, 0)
        {
        }

        /// <summary>
        /// Automatically unwrap the value of the Angle, to be able to use with other API fields that
        /// expect angle values in degrees.
        /// </summary>
        /// <param name="a">An <c>Angle</c> object.</param>
        public float ToSingle()
        {
            return currentValue;
        }

        /// <summary>
        /// Automatically unwrap the value of the Angle, to be able to use with other API fields that
        /// expect angle values in degrees.
        /// </summary>
        /// <param name="a">An <c>Angle</c> object.</param>
        public static implicit operator float(Angle a)
        {
            return a.ToSingle();
        }

        /// <summary>
        /// Wrap an angle value in the Angle struct. Useful for using
        /// <c>System.Linq.Enumerable.Cast{T}</c> on a collection of Angles.
        /// </summary>
        /// <param name="f">An angle in degrees</param>
        public static Angle FromSingle(float f)
        {
            return new Angle(f);
        }

        /// <summary>
        /// Wrap an angle value in the Angle struct. Useful for using
        /// <c>System.Linq.Enumerable.Cast{T}</c> on a collection of Angles.
        /// </summary>
        /// <param name="f">An angle in degrees</param>
        public static explicit operator Angle(float f)
        {
            return FromSingle(f);
        }

        /// <summary>
        /// Create a new Angle struct that represents the smallest turn from the old Angle struct in
        /// either the clockwise or counter-clockwise direction that matches the new angle.
        /// </summary>
        /// <param name="v">The new angle value, in degrees.</param>
        /// <returns>
        /// The new Angle struct that minimizes the likelihood of spinning around in weird circles.
        /// </returns>
        public Angle Xor(float v)
        {
            return Update(v);
        }

        /// <summary>
        /// Create a new Angle struct that represents the smallest turn from the old Angle struct in
        /// either the clockwise or counter-clockwise direction that matches the new angle.
        /// </summary>
        /// <param name="a">The old Angle struct.</param>
        /// <param name="v">The new angle value, in degrees.</param>
        /// <returns>
        /// The new Angle struct that minimizes the likelihood of spinning around in weird circles.
        /// </returns>
        public static Angle operator ^(Angle a, float v)
        {
            return a.Xor(v);
        }

        /// <summary>
        /// Whole-number divide the angle and save the remainder, and minimize rotation to the new value
        /// </summary>
        /// <param name="v">The scalar by which to divide.</param>
        /// <returns>The minimized value of clamp(a % v, 0, 360)</returns>
        public Angle Mod(float v)
        {
            return Update(currentValue % v);
        }

        /// <summary>
        /// Whole-number divide the angle and save the remainder, and minimize rotation to the new value
        /// </summary>
        /// <param name="a">The angle to divide.</param>
        /// <param name="v">The scalar by which to divide.</param>
        /// <returns>The minimized value of clamp(a % v, 0, 360)</returns>
        public static Angle operator %(Angle a, float v)
        {
            return a.Mod(v);
        }

        /// <summary>
        /// Add to the angle, and minimize rotation to the new value
        /// </summary>
        /// <param name="v">The scalar by which to add.</param>
        /// <returns>The minimized value of clamp(a + v, 0, 360)</returns>
        public Angle Add(float v)
        {
            return Update(currentValue + v);
        }

        /// <summary>
        /// Add to the angle, and minimize rotation to the new value
        /// </summary>
        /// <param name="a">The angle to add to.</param>
        /// <param name="v">The scalar by which to add.</param>
        /// <returns>The minimized value of clamp(a + v, 0, 360)</returns>
        public static Angle operator +(Angle a, float v)
        {
            return a.Add(v);
        }

        /// <summary>
        /// Subtract from the angle, and minimize rotation to the new value
        /// </summary>
        /// <param name="v">The scalar by which to subtract.</param>
        /// <returns>The minimized value of clamp(a - v, 0, 360)</returns>
        public Angle Subtract(float v)
        {
            return Update(currentValue - v);
        }

        /// <summary>
        /// Subtract from the angle, and minimize rotation to the new value
        /// </summary>
        /// <param name="a">The angle to subtract from.</param>
        /// <param name="v">The scalar by which to subtract.</param>
        /// <returns>The minimized value of clamp(a - v, 0, 360)</returns>
        public static Angle operator -(Angle a, float v)
        {
            return a.Subtract(v);
        }

        /// <summary>
        /// Multiply the angle, and minimize rotation to the new value
        /// </summary>
        /// <param name="v">The scalar by which to multiply.</param>
        /// <returns>The minimized value of clamp(a * v, 0, 360)</returns>
        public Angle Multiply(float v)
        {
            return Update(currentValue * v);
        }

        /// <summary>
        /// Multiply the angle, and minimize rotation to the new value
        /// </summary>
        /// <param name="a">The angle to multiply.</param>
        /// <param name="v">The scalar by which to multiply.</param>
        /// <returns>The minimized value of clamp(a * v, 0, 360)</returns>
        public static Angle operator *(Angle a, float v)
        {
            return a.Multiply(v);
        }

        /// <summary>
        /// Divide the angle, and minimize rotation to the new value
        /// </summary>
        /// <param name="v">The scalar by which to divide.</param>
        /// <returns>The minimized value of clamp(a / v, 0, 360)</returns>
        public Angle Divide(float v)
        {
            return Update(currentValue / v);
        }

        /// <summary>
        /// Divide the angle, and minimize rotation to the new value
        /// </summary>
        /// <param name="a">The angle to divide.</param>
        /// <param name="v">The scalar by which to divide.</param>
        /// <returns>The minimized value of clamp(a / v, 0, 360)</returns>
        public static Angle operator /(Angle a, float v)
        {
            return a.Divide(v);
        }

        /// <summary>
        /// Compares two angles to see if they represent the same position on a circle.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Angle left, Angle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two angles to see if they do not represent the same position on a circle.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Angle left, Angle right)
        {
            return !(left == right);
        }

        private static float Repeat(float v)
        {
            while (v >= 360)
            {
                v -= 360;
            }
            while (v < 0)
            {
                v += 360;
            }
            return v;
        }

        /// <summary>
        /// Compares two angles to see if they represent the same position on a circle.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Angle angle2)
            {
                var f1 = Repeat(currentValue);
                var f2 = Repeat(angle2.currentValue);
                return f1.Equals(f2);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="T:UnityEngine.Angle"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data
        /// structures such as a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Repeat(currentValue).GetHashCode();
        }

        /// <summary>
        /// The current value of the angle,
        /// </summary>
        private readonly float currentValue;

        /// <summary>
        /// The number of whole-circle rotations, times 360, we had to take to get to the current value.
        /// </summary>
        private readonly float rotations;

        /// <summary>
        /// Performs the min-distance-edit operation, given the new, desired angle value.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="v1">V1.</param>
        private Angle Update(float v1)
        {
            float r = rotations,
                v0 = currentValue;

            v1 = Repeat(v1);

            float d1, d2, d3;
            do
            {
                // figure out if it is adding the raw value, or whole rotations of the value, that
                // results in a smaller magnitude of change.
                d1 = v1 + r - v0;
                d2 = Math.Abs(d1 + 360);
                d3 = Math.Abs(d1 - 360);
                d1 = Math.Abs(d1);
                if (d2 < d1 && d2 < d3)
                {
                    r += 360;
                }
                else if (d3 < d1)
                {
                    r -= 360;
                }
            } while (d1 > d2 || d1 > d3);

            return new Angle(v1 + r, r);
        }

        public override string ToString()
        {
            return Converter.Label(currentValue, UnitOfMeasure.Degrees);
        }

        public static explicit operator string(Angle value)
        {
            return value.ToString();
        }
    }
}
