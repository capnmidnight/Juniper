namespace System
{
    public struct TrackedValue<T>
    {
        public TrackedValue(T v)
        {
            lastValue = backer = v;
            ValueChanged = null;
        }

        public event EventHandler ValueChanged;

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

        public bool Changed
        {
            get
            {
                return IsDifferent(lastValue);
            }
        }

        public static implicit operator T(TrackedValue<T> wrapped)
        {
            return wrapped.Value;
        }

        public T Commit()
        {
            return lastValue = backer;
        }

        public void Revert()
        {
            Value = lastValue;
        }

        private T backer, lastValue;

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool IsDifferent(T value)
        {
            var backerNull = ReferenceEquals(backer, default(T));
            var valueNull = ReferenceEquals(value, default(T));
            return backerNull != valueNull || !backerNull && !backer.Equals(value);
        }
    }
}