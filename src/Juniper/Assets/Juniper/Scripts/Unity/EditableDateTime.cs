using System;

using UnityEngine;

namespace Juniper
{
    /// <summary>
    /// A wrapper around System.DateTime that makes it possible to edit values in the Unity Editor.
    /// </summary>
    [Serializable]
    public struct EditableDateTime
    {
        /// <summary>
        /// When testing applications running at different times, you can set this value to change
        /// the time the application thinks it is currently.
        /// </summary>
        public static TimeSpan TestOffset = new TimeSpan(0, 0, 0);

        /// <summary>
        /// Unspecified, UTC, or Local.
        /// </summary>
        public DateTimeKind Kind;

        /// <summary>
        /// The year.
        /// </summary>
        [Range(1, 9999)]
        public int Year;

        /// <summary>
        /// The month.
        /// </summary>
        public Month Month;

        /// <summary>
        /// The day. If the day is not valid for the month, it will be reset to the next previous
        /// valid day.
        /// </summary>
        [Range(1, 31)]
        public int Day;

        /// <summary>
        /// The hour.
        /// </summary>
        [Range(0, 23)]
        public int Hour;

        /// <summary>
        /// The minute.
        /// </summary>
        [Range(0, 59)]
        public int Minute;

        /// <summary>
        /// The second.
        /// </summary>
        [Range(0, 59)]
        public int Second;

        /// <summary>
        /// The millisecond.
        /// </summary>
        [Range(0, 999)]
        public int Millisecond;

        /// <summary>
        /// Initializes a new structure for editing DateTime values in the Unity Editor.
        /// </summary>
        /// <param name="time">Time.</param>
        public EditableDateTime(DateTime time)
        {
            lastValue = time;
            Year = lastValue.Year;
            Month = (Month)lastValue.Month;
            Day = lastValue.Day;
            Hour = lastValue.Hour;
            Minute = lastValue.Minute;
            Second = lastValue.Second;
            Millisecond = lastValue.Millisecond;
            Kind = lastValue.Kind;
        }

        /// <summary>
        /// Gets the current system time, plus any <see cref="TestOffset"/>.
        /// </summary>
        /// <value>The now.</value>
        public static DateTime Now
        {
            get
            {
                return DateTime.Now + TestOffset;
            }
        }

        /// <summary>
        /// Gets or sets the underlying DateTime value.
        /// </summary>
        /// <value>The value.</value>
        public DateTime Value
        {
            get
            {
                if (Changed)
                {
                    while (Day > DateTime.DaysInMonth(Year, (int)Month))
                    {
                        --Day;
                    }

                    lastValue = new DateTime(Year, (int)Month, Day, Hour, Minute, Second, Millisecond, Kind);
                }

                return lastValue;
            }

            set
            {
                lastValue = value;
                Year = value.Year;
                Month = (Month)value.Month;
                Day = value.Day;
                Hour = value.Hour;
                Minute = value.Minute;
                Second = value.Second;
                Millisecond = value.Millisecond;
                Kind = value.Kind;
            }
        }

        /// <summary>
        /// Automatically convert the EditableDateTime to a DateTime, meaning you can use an
        /// EditableDateTime anywhere you can use a DateTime.
        /// </summary>
        /// <returns>The unwrapped DateTime.</returns>
        /// <param name="val">Value.</param>
        public static implicit operator DateTime(EditableDateTime val)
        {
            return val.Value;
        }

        /// <summary>
        /// Automatically convert the DateTime to an EditableDateTime, meaning you can use an
        /// DateTime anywhere you can use a EditableDateTime.
        /// </summary>
        /// <returns>The DateTime, wrapped in an EditableDateTime</returns>
        /// <param name="val">Value.</param>
        public static implicit operator EditableDateTime(DateTime val)
        {
            return new EditableDateTime(val);
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="UnityEngine.EditableDateTime"/> is
        /// equal to another specified <see cref="UnityEngine.EditableDateTime"/>.
        /// </summary>
        /// <param name="a">The first <see cref="UnityEngine.EditableDateTime"/> to compare.</param>
        /// <param name="b">The second <see cref="UnityEngine.EditableDateTime"/> to compare.</param>
        /// <returns><c>true</c> if <c>a</c> and <c>b</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(EditableDateTime a, EditableDateTime b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="UnityEngine.EditableDateTime"/> is
        /// not equal to another specified <see cref="UnityEngine.EditableDateTime"/>.
        /// </summary>
        /// <param name="a">The first <see cref="UnityEngine.EditableDateTime"/> to compare.</param>
        /// <param name="b">The second <see cref="UnityEngine.EditableDateTime"/> to compare.</param>
        /// <returns><c>true</c> if <c>a</c> and <c>b</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(EditableDateTime a, EditableDateTime b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="UnityEngine.EditableDateTime"/> is
        /// equal to another specified <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="a">The first <see cref="UnityEngine.EditableDateTime"/> to compare.</param>
        /// <param name="b">The second <see cref="System.DateTime"/> to compare.</param>
        /// <returns><c>true</c> if <c>a</c> and <c>b</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(EditableDateTime a, DateTime b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="UnityEngine.EditableDateTime"/> is
        /// not equal to another specified <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="a">The first <see cref="UnityEngine.EditableDateTime"/> to compare.</param>
        /// <param name="b">The second <see cref="System.DateTime"/> to compare.</param>
        /// <returns><c>true</c> if <c>a</c> and <c>b</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(EditableDateTime a, DateTime b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="System.DateTime"/> is equal to
        /// another specified <see cref="UnityEngine.EditableDateTime"/>.
        /// </summary>
        /// <param name="a">The first <see cref="System.DateTime"/> to compare.</param>
        /// <param name="b">The second <see cref="UnityEngine.EditableDateTime"/> to compare.</param>
        /// <returns><c>true</c> if <c>a</c> and <c>b</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(DateTime a, EditableDateTime b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="System.DateTime"/> is not equal to
        /// another specified <see cref="UnityEngine.EditableDateTime"/>.
        /// </summary>
        /// <param name="a">The first <see cref="System.DateTime"/> to compare.</param>
        /// <param name="b">The second <see cref="UnityEngine.EditableDateTime"/> to compare.</param>
        /// <returns><c>true</c> if <c>a</c> and <c>b</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(DateTime a, EditableDateTime b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:UnityEngine.EditableDateTime"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:UnityEngine.EditableDateTime"/>.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to the current <see
        /// cref="T:UnityEngine.EditableDateTime"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is EditableDateTime edt)
            {
                return edt.Value == Value;
            }
            else if (obj is DateTime dt)
            {
                return dt == Value;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="T:UnityEngine.EditableDateTime"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data
        /// structures such as a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// The underlying DateTime value.
        /// </summary>
        private DateTime lastValue;

        /// <summary>
        /// Check to see if the input date values are different from the saved DateTime values.
        /// </summary>
        /// <value><c>true</c> if any of the values are different, <c>false</c> otherwise.</value>
        private bool Changed
        {
            get
            {
                return Year != lastValue.Year
                    || Month != (Month)lastValue.Month
                    || Day != lastValue.Day
                    || Hour != lastValue.Hour
                    || Minute != lastValue.Minute
                    || Second != lastValue.Second
                    || Millisecond != lastValue.Millisecond
                    || Kind != lastValue.Kind;
            }
        }
    }
}
