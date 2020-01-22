namespace Juniper.Haptics
{
    public interface IHapticDevice
    {
        /// <summary>
        /// Cancel the current vibration, whatever it is.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Play a canned haptic pattern.
        /// </summary>
        /// <param name="expr"></param>
        void Play(HapticExpression expr);

        /// <summary>
        /// Play a single element of a haptic pattern.
        /// </summary>
        /// <param name="point"></param>
        void Vibrate(HapticPatternElement point);

        /// <summary>
        /// Play a haptic pattern.
        /// </summary>
        /// <param name="points"></param>
        void Play(params HapticPatternElement[] points);

        /// <summary>
        /// Play a patterned vibration, where all of the vibrations are the same amplitude, the
        /// default amplitude.
        /// </summary>
        /// <param name="pattern">Pattern.</param>
        void Play(long[] pattern);

        /// <summary>
        /// Play a patterned vibration with amplitude modulation.
        /// </summary>
        /// <param name="pattern">   Pattern.</param>
        /// <param name="amplitudes">Amplitudes.</param>
        void Play(long[] pattern, float[] amplitudes);

        /// <summary>
        /// Play a single vibration of a set length of time.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        void Vibrate(long milliseconds);

        /// <summary>
        /// Play a single vibration, of a set length of time, at a set strength.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        /// <param name="amplitude">   Amplitude values should be on the range [0, 1].</param>
        void Vibrate(long milliseconds, float amplitude);
    }
}
