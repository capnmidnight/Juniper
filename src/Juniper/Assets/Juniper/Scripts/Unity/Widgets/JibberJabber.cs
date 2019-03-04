#if UNITY_MODULES_AUDIO

using Juniper.Audio;

using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// Plays clips out of an AudioClipCollection at set intervals, making for a point source of
    /// ambient sound. An <see cref="AudioSource"/> is required to use this component. Only one
    /// JibberJabber component is allowed on a gameObject at a time.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class JibberJabber : MonoBehaviour
    {
        /// <summary>
        /// A set of audio clips, which may be played at random.
        /// </summary>
        public AudioClipCollection clips;

        /// <summary>
        /// The number of seconds between each play of an audio clip.
        /// </summary>
        public float timeBetweenClips = 1;

        /// <summary>
        /// Get the audioSource, and if it's set to playOnAwake, start the JibberJabber.
        /// </summary>
        public void Awake()
        {
            audioSource = GetComponent<AudioSource>()
                .Spatialize();

            if (audioSource.playOnAwake)
            {
                Play();
            }
        }

        /// <summary>
        /// Check to see if the timeout has expired, and play a new, random audioClip if it has,
        /// resetting the timeout.
        /// </summary>
        public void Update()
        {
            if (playing)
            {
                if (time <= 0)
                {
                    audioSource.clip = clips.Random();
                    audioSource.Play();
                    time += timeBetweenClips + audioSource.clip.length;
                }

                time -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Start the JibberJabber play loop.
        /// </summary>
        public void Play()
        {
            playing = true;
            time = 0;
        }

        /// <summary>
        /// Stop the JibberJabber timeout from decreasing, thereby stopping the JibberJabber. The
        /// currently playing audioClip will not be stopped.
        /// </summary>
        public void Stop()
        {
            playing = false;
        }

        /// <summary>
        /// The audioSource through which to play the audio clips.
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// The number of seconds before the next audioClip should be played.
        /// </summary>
        private float time;

        /// <summary>
        /// Whether or not to keep playing.
        /// </summary>
        private bool playing;
    }
}

#endif