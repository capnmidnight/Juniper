using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    /// <summary>
    /// An abstraction of storing values on disk in the Assets folder. A sort of drag-and-drop means
    /// of settings certain common values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ComVisible(false)]
    [Serializable]
    public abstract class AbstractVariable<T> : ScriptableObject, IValued<T>
    {
        /// <summary>
        /// The underlying saved value.
        /// </summary>
        [SerializeField]
        private T _value;

        /// <summary>
        /// The raw value encapsulated by this Variable.
        /// </summary>
        public T Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Cast the raw value encapsulated in this variable out to values of its own type.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator T(AbstractVariable<T> value) =>
            value.Value;

        /// <summary>
        /// Print the encapsulated value, rather than a junky Object reference.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            Value.ToString();
    }
}
