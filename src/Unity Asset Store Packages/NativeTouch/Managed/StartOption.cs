namespace E7.Native
{
    public static partial class NativeTouch
    {
        /// <summary>
        /// An input parameter for the <see cref="NativeTouch.Start(StartOption)"> `static` method.
        /// </summary>
        public class StartOption
        {
            /// <summary>
            /// **SETTING THIS TO TRUE IS EXPERIMENTAL**
            /// If `false`, The touch data returned will only contains x, y, previous x, previous y, phase, and timestamp. It *might* be faster since there are way fewer parameters, and the static callback is easier to define. I believe most of us will not use full mode.
            /// If `true`, it returns various other touch data. This mode is in BETA and not tested extensively yet, since it would require various devices that can handle pressure, tilt, angle, etc. that I don't currently have. (Apple Pencil 2, iPad Pro, iPhone X, etc.)
            /// </summary>
            public bool fullMode = false;

            /// <summary>
            /// Also disable sending touch to normal Unity input path. Disable all UGUI and Event System functionality consequently.
            /// Not sure if there are meaningful gain in using this since with this as false I could not benchmark the Unity touch anymore.
            /// But *certainly* it free up Unity from processing touch at all.
            /// 
            /// If you decided to turn this on, be sure to have a way of calling <see cref="Stop"> without relying on Unity's touch or you might stuck with Native Touch forever..
            /// </summary>
            public bool disableUnityTouch = false;

            /// <summary>
            /// If `true`, it does not matter if you registered any callbacks or not before using <see cref="Start(StartOption)">, Native Touch will **let** you start.
            /// 
            /// In the native side, it will not try to invoke any callback.
            /// 
            /// This is for use with ring buffer iteration based API. If you want to exclusively iterate through ring buffer with <see cref="NativeTouch.touches"> 
            /// and not want the callback, it is good to disable the callback completely with this.
            /// (On platform like Android IL2CPP and not Mono, the callback is very expensive. Not sure if it is a bug or not.)
            /// 
            /// However you can still use both callback and ring buffer iteration API at the same time.
            /// Just that previously without any callbacks, Native Touch will not let you <see cref="Start(StartOption)">.
            /// </summary>
            public bool noCallback = false;

            /// <summary>
            /// This size will be told to native side where it uses the ring buffer.
            /// The memory is only deallocated after a successful <see cref="Stop">.
            /// </summary>
            public int ringBufferSize = 150;
        }
    }
}