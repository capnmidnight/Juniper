#if UNITY_IOS
using System.Runtime.InteropServices;

namespace Juniper.Haptics
{
    /// <summary>
    /// iOS 10 Taptic Feedback. /// <a
    /// href="https://www.hackingwithswift.com/example-code/uikit/how-to-generate-haptic-feedback-with-uifeedbackgenerator">How
    /// to Generate Haptic Feedback with UIFeedbackGenerator</a>
    /// </summary>
    public class iOS10Haptics : iOS9Haptics
    {
        public static class NativeMethods
        {
            /// <summary>
            /// Call out to the Taptic interface through the Taptic feedback plugin to prepare the
            /// Taptic engine.
            /// </summary>
            [DllImport("__Internal")]
            public static extern void Prepare();

            /// <summary>
            /// Plays one of the Taptic engine's canned feedback patterns.
            /// </summary>
            /// <param name="expr">Expr.</param>
            [DllImport("__Internal")]
            public static extern void PlayTaptic(uint expr);

            /// <summary>
            /// Turn the Taptic engine off.
            /// </summary>
            [DllImport("__Internal")]
            public static extern void Shutdown();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Haptics.iOS10Haptics"/> class.
        /// </summary>
        public void Awake() =>
            NativeMethods.Prepare();

        /// <summary>
        /// Play a canned haptic pattern.
        /// </summary>
        /// <param name="expr"></param>
        public override void Play(HapticExpression expr)
        {
            if (expr != HapticExpression.None)
            {
                NativeMethods.PlayTaptic((uint)expr);
            }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:Juniper.Haptics.iOS10Haptics"/> object.
        /// </summary>
        void OnDestroy() =>
            NativeMethods.Shutdown();
    }
}
#endif
