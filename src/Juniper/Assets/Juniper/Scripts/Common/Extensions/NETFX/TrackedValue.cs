namespace System
{
    /// <summary>
    /// An object that announces changes to an encapsulated value.
    /// </summary>
    /// <typeparam name="T">The type of the value to track.</typeparam>
    public class TrackedValue<T>
    {
        /// <summary>
        /// The tracked value.
        /// </summary>
        private T backer;

        /// <summary>
        /// The value of the tracked value before it was changed.
        /// </summary>
        private T lastValue;

        /// <summary>
        /// Track a value for changes.
        /// </summary>
        /// <param name="v">The value to track.</param>
        public TrackedValue(T v)
        {
            lastValue = backer = v;
            ValueChanged = null;
        }

        /// <summary>
        /// An event that fires when the tracked value changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// An accessor for the tracked value, to enable checking when
        /// the value changes.
        /// </summary>
        public T Value
        {
            get
            {
                return backer;
            }
            set
            {
                if (IsDifferent(value))
                {
                    backer = value;
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Returns true when backer differs from lastValue.
        /// </summary>
        public bool Changed
        {
            get
            {
                return IsDifferent(lastValue);
            }
        }

        /// <summary>
        /// Unwraps a tracked value.
        /// </summary>
        /// <param name="wrapped">The tracked value to unwrap</param>
        public static implicit operator T(TrackedValue<T> wrapped)
        {
            return wrapped.Value;
        }

        /// <summary>
        /// Save the changed value as the current value.
        /// </summary>
        /// <returns>The value that was saved.</returns>
        public T Commit()
        {
            return lastValue = backer;
        }

        /// <summary>
        /// Undo a change to the tracked value. Only one undo is available.
        /// </summary>
        public void Revert()
        {
            Value = lastValue;
        }

        /// <summary>
        /// Execute the ValueChanged event.
        /// </summary>
        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Compares the current value to another value to see if they reference the same value.
        /// </summary>
        /// <param name="value">The value to compare against</param>
        /// <returns>True, if the values are not the same, false otherwise.</returns>
        private bool IsDifferent(T value)
        {
            var backerNull = ReferenceEquals(backer, default(T));
            var valueNull = ReferenceEquals(value, default(T));
            return backerNull != valueNull || (!backerNull && !backer.Equals(value));
        }
    }
}
