using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    /// <summary>
    /// A reference to either a savable *Variable or a static value set in the editor.
    /// </summary>
    [ComVisible(false)]
    [Serializable]
    public abstract class AbstractReference<T, U> : IValued<T>
        where U : AbstractVariable<T>
    {
        /// <summary>
        /// When True, the Unity Editor will display text box for entering a value manually, rather
        /// than using a shared value out of the assets folder.
        /// </summary>
        public bool UseEphemeral;

        /// <summary>
        /// When <see cref="UseEphemeral"/> is true, the value that is to be used as the value of
        /// this reference.
        /// </summary>
        public T EphemeralValue;

        /// <summary>
        /// When <see cref="UseEphemeral"/> is false, the value to load from disk to use as the value
        /// of this reference.
        /// </summary>
        public U Variable;

        /// <summary>
        /// Returns either the EphemeralValue or the value encapsulated in the Variable that this
        /// reference encapsulates, based on the current value of <see cref="UseEphemeral"/>.
        /// </summary>
        public T Value
        {
            get
            {
                if (UseEphemeral)
                {
                    return EphemeralValue;
                }
                else if (Variable == null)
                {
                    return default(T);
                }
                else
                {
                    return Variable.Value;
                }
            }
            set
            {
                if (UseEphemeral)
                {
                    EphemeralValue = value;
                }
                else
                {
                    if (Variable == null)
                    {
                        Variable = ScriptableObject.CreateInstance<U>();
                    }
                    Variable.Value = value;
                }
            }
        }

        /// <summary>
        /// Cast the value encapsulated in this reference out to its raw type.
        /// </summary>
        /// <param name="obj"></param>
        public static implicit operator T(AbstractReference<T, U> obj) =>
            obj.Value;

        /// <summary>
        /// Print the encapsulated value.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            Value.ToString();
    }
}
