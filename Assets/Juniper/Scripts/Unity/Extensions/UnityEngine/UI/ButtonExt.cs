using UnityEngine.Events;

namespace UnityEngine.UI
{
    /// <summary>
    /// Extensions to Unity's 2D UI Button object, to make it easier to use along side Juniper's
    /// Touchable widgets.
    /// </summary>
    public static class ButtonExt
    {
        /// <summary>
        /// Add a listener to the OnClick event of the button.
        /// </summary>
        /// <param name="btn">Button.</param>
        /// <param name="action">Action.</param>
        public static void AddListener(this Button btn, UnityAction action) =>
            btn.onClick.AddListener(action);

        /// <summary>
        /// Remove a listener from the OnClick event of the button.
        /// </summary>
        /// <param name="btn">Button.</param>
        /// <param name="action">Action.</param>
        public static void RemoveListener(this Button btn, UnityAction action) =>
            btn.onClick.RemoveListener(action);

        /// <summary>
        /// Remove all listeners from the OnClick event of the button.
        /// </summary>
        /// <param name="btn">Button.</param>
        public static void RemoveAllListeners(this Button btn) =>
            btn.onClick.RemoveAllListeners();

        /// <summary>
        /// The SetDisabled function is useful for inverting the meaning of "enabled".
        /// </summary>
        /// <param name="btn">Button.</param>
        /// <param name="v">If set to <c>true</c> v.</param>
        public static void SetDisabled(this Button btn, bool v) =>
            btn.enabled = !v;
    }
}
