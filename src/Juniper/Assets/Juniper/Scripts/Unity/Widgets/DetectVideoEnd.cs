using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Juniper.Widgets
{
    /// <summary>
    /// Detects when a video player has played past the end of a video clip, either terminally or looping.
    /// </summary>
    [DisallowMultipleComponent]
#if UNITY_MODULES_VIDEO
    [RequireComponent(typeof(VideoPlayer))]
#endif
    public class DetectVideoEnd : MonoBehaviour
    {
#if UNITY_MODULES_VIDEO

        /// <summary>
        /// The video player on which we are attempting to detect an end.
        /// </summary>
        private VideoPlayer player;

        /// <summary>
        /// The isPlaying status of the video from the previous frame, to be able to detect changes
        /// between frames.
        /// </summary>
        private bool wasPlaying;

        /// <summary>
        /// A Unity Event that is fired when the video has reached its end.
        /// </summary>
        public UnityEvent onVideoEnd = new UnityEvent();

        /// <summary>
        /// A standard .NET event that is fired when the video has reached its end.
        /// </summary>
        public event EventHandler VideoEnd;

        /// <summary>
        /// Fires the <see cref="onVideoEnd"/> and <see cref="VideoEnd"/> events.
        /// </summary>
        private void OnVideoEnd()
        {
            onVideoEnd?.Invoke();
            VideoEnd?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Retrieves the video player component on this game object.
        /// </summary>
        public void Awake()
        {
            player = GetComponent<VideoPlayer>();
            player.loopPointReached += Player_LoopPointReached;
        }

        /// <summary>
        /// Triggers <see cref="OnVideoEnd"/> when the player has looped.
        /// </summary>
        /// <param name="source">Source.</param>
        private void Player_LoopPointReached(VideoPlayer source)
        {
            OnVideoEnd();
        }

        /// <summary>
        /// Resets the playing status to false.
        /// </summary>
        public void OnEnable()
        {
            wasPlaying = false;
        }

        /// <summary>
        /// Checks to see if the video has reached the last frame.
        /// </summary>
        public void Update()
        {
            if (player.isPlaying && !player.isLooping)
            {
                var isPlaying = player.frame < (long)player.frameCount;
                if (!isPlaying && wasPlaying)
                {
                    OnVideoEnd();
                }
                wasPlaying = isPlaying;
            }
        }

#endif
    }
}