using Juniper.Animation;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Juniper.Widgets
{
    /// <summary>
    /// Manages the life-cycle of a video that can be faded out or faded in.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class VideoVader : AlphaTransition
    {
#if UNITY_MODULES_VIDEO

        /// <summary>
        /// The video player that is to be faded in and out.
        /// </summary>
        private VideoPlayer player;

#endif

        /// <summary>
        /// The number of seconds a video may remain hidden until it resets to the beginning.
        /// </summary>
        public float restartTimeout = 10;

        /// <summary>
        /// The number of second to wait after the video has ended to trigger the next state.
        /// </summary>
        public float lingerTimeout = 0;

        /// <summary>
        /// A Unity Event that fires when the video has finished playing.
        /// </summary>
        public UnityEvent onComplete = new UnityEvent();

        /// <summary>
        /// Retrieves the video player and its fader.
        /// </summary>
        public void Awake()
        {
#if UNITY_MODULES_VIDEO
            player = GetComponent<VideoPlayer>();
            this.Ensure<DetectVideoEnd>()
                .Value
                .onVideoEnd
                .AddListener(Finish);
#endif
        }

        /// <summary>
        /// Reset the video to the beginning if the user lost tracking for a long time.
        /// </summary>
        private void RestartVideo()
        {
#if UNITY_MODULES_VIDEO
            player.Stop();
#endif
        }

        protected override void OnEntering()
        {
            base.OnEntering();
            CancelInvoke(nameof(RestartVideo));
#if UNITY_MODULES_VIDEO
            player.Stop();
            player.Play();
#endif
        }

        protected override void OnExiting()
        {
            Invoke(nameof(RestartVideo), restartTimeout);
        }

        public void FireComplete()
        {
            Finish();
        }

        /// <summary>
        /// Trigger the timeout for the finisher.
        /// </summary>
        private void Finish()
        {
            Invoke(nameof(OnFinish), lingerTimeout);
        }

        /// <summary>
        /// Finish out the video state.
        /// </summary>
        private void OnFinish()
        {
            onComplete?.Invoke();
        }

        protected override void RenderValue(float value)
        {
#if UNITY_MODULES_VIDEO
            if (player.renderMode == VideoRenderMode.CameraNearPlane
               || player.renderMode == VideoRenderMode.CameraFarPlane)
            {
                var o = Mathf.Clamp01(minAlpha + value * DeltaAlpha);
                player.targetCameraAlpha = o;
            }
            else
            {
#endif
                base.RenderValue(value);
#if UNITY_MODULES_VIDEO
            }
#endif
        }
    }
}
