using System.Reflection;

namespace Juniper
{
    /// <summary>
    /// A base class for all of the Settings extender classes to gain access to helper methods that
    /// do the hard work of finding the hidden settings.
    /// </summary>
    public class HiddenSettingsAccessor
    {
        /// <summary>
        /// Sets a static property value that is hidden in a class.
        /// </summary>
        /// <returns><c>true</c>, if hidden static was set, <c>false</c> otherwise.</returns>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value to set.</param>
        /// <typeparam name="T">The class in which the property is hidden.</typeparam>
        public static bool SetHiddenStaticProperty<T>(string name, object value)
        {
            var t = typeof(T);
            var p = t.GetProperty(name, HIDDEN_STATIC);
            var isGood = p != null;
            if (isGood)
            {
                p.SetValue(null, value);
            }
            return isGood;
        }

        /// <summary>
        /// Gets a static property value that is hidden in a class.
        /// </summary>
        /// <returns>
        /// The value contained in the property, if it exists. The default value for <typeparamref
        /// name="U"/>, if the property does not exist.
        /// </returns>
        /// <param name="name">The name of the property.</param>
        /// <typeparam name="T">The class in which the property is hidden.</typeparam>
        /// <typeparam name="U">The return type of the property.</typeparam>
        public static U GetHiddenStaticProperty<T, U>(string name)
        {
            var t = typeof(T);
            var p = t.GetProperty(name, HIDDEN_STATIC);
            if (p != null)
            {
                return (U)p.GetValue(null);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Calls the hidden static method on the provided type <typeparamref name="T"/>..
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="args">Arguments to pass to the method.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void CallHiddenStaticMethod<T>(string name, params object[] args)
        {
            var t = typeof(T);
            var m = t.GetMethod(name, HIDDEN_STATIC);
            m?.Invoke(null, args);
        }

        /// <summary>
        /// Standard binding flags for finding hidden properties in these settings classes.
        /// </summary>
        private const BindingFlags HIDDEN_STATIC = BindingFlags.Static | BindingFlags.NonPublic;
    }
}
