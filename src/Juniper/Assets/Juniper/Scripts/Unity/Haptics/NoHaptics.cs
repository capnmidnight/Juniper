namespace Juniper.Unity.Haptics
{
    /// <summary>
    /// When we are on a system that we know for a fact does not have haptics, we can initialize the
    /// haptic subsystem with one that does nothing.
    /// </summary>
    public class NoHaptics : AbstractHapticDevice
    {
        /// <summary>
        /// Performs no operation.
        /// </summary>
        public override void Cancel()
        {
        }

        /// <summary>
        /// Performs no operation.
        /// </summary>
        public override void Play(HapticExpression expr)
        {
        }

        /// <summary>
        /// Performs no operation.
        /// </summary>
        public override void Play(params HapticPatternElement[] points)
        {
        }

        /// <summary>
        /// Performs no operation.
        /// </summary>
        public override void Play(long[] pattern)
        {
        }

        /// <summary>
        /// Performs no operation.
        /// </summary>
        public override void Play(long[] pattern, float[] amplitudes)
        {
        }

        /// <summary>
        /// Performs no operation.
        /// </summary>
        public override void Vibrate(HapticPatternElement point)
        {
        }

        /// <summary>
        /// Performs no operation.
        /// </summary>
        public override void Vibrate(long milliseconds)
        {
        }

        /// <summary>
        /// Performs no operation.
        /// </summary>
        public override void Vibrate(long milliseconds, float amplitude)
        {
        }
    }
}
