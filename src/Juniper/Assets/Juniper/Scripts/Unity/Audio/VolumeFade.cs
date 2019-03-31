using UnityEngine;

namespace Juniper.Unity.Audio
{
    /// <summary>
    /// An audio-filter that fades a sound out over time. An <see cref="AudioSource"/> component is
    /// required to be on the same gameObject. Only one VolumeFade component is allowed on a
    /// gameObject at a time.
    /// </summary>
    [DisallowMultipleComponent]
#if UNITY_MODULES_AUDIO
    [RequireComponent(typeof(AudioSource))]
#endif
    public class VolumeFade : MonoBehaviour
    {
        /// <summary>
        /// The number of seconds to spend fading in.
        /// </summary>
        public float attack;

        /// <summary>
        /// The number of seconds to spend fading out.
        /// </summary>
        public float release;

        /// <summary>
        /// Adjusts the fade rate. Set to 1 for linear, 2 to quadratic, 3 to cubic, etc.
        /// </summary>
        public float fadePower = 2;

#if UNITY_MODULES_AUDIO

        /// <summary>
        /// Whether or not the fader is currently running.
        /// </summary>
        private bool running = false;

        /// <summary>
        /// Whether or not the audioSource was playing on the last frame.
        /// </summary>
        private bool wasPlaying = false;

        /// <summary>
        /// The current audio volume.
        /// </summary>
        private float volume = 1;

        /// <summary>
        /// The total amount of time that has passed during the current iteration of the fade transition.
        /// </summary>
        private float time;

        /// <summary>
        /// The audioSource to fade.
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// Get the audioSource and start fading right away.
        /// </summary>
        public void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            running = true;
        }

        /// <summary>
        /// Read the raw audio data and scale it according to the master volume setting for the
        /// current frame.
        /// </summary>
        /// <param name="data">    </param>
        /// <param name="channels"></param>
        public void OnAudioFilterRead(float[] data, int channels)
        {
            if (running)
            {
                for (var i = 0; i < data.Length; ++i)
                {
                    data[i] = volume * data[i];
                }
            }
        }

        /// <summary>
        /// If an audioClip is playing, set the volume for this frame, to create a fade effect over time.
        /// </summary>
        public void Update()
        {
            if (audioSource.isPlaying && audioSource.clip != null)
            {
                float releaseThreshold;
                if (audioSource.loop)
                {
                    attack = Mathf.Clamp(attack, 0, audioSource.clip.length);
                    release = Mathf.Clamp(release, 0, audioSource.clip.length);
                    releaseThreshold = float.MaxValue;
                }
                else
                {
                    attack = Mathf.Max(0, attack);
                    release = Mathf.Max(0, release);
                    releaseThreshold = audioSource.clip.length - release;
                }

                if (wasPlaying && audioSource.loop)
                {
                    time += Time.deltaTime;
                }
                else
                {
                    time = audioSource.time;
                }

                if (time < attack)
                {
                    var p = time / attack;
                    volume = Mathf.Pow(p, fadePower);
                }
                else if (time <= releaseThreshold)
                {
                    volume = 1;
                }
                else
                {
                    var p = (time - releaseThreshold) / release;
                    volume = Mathf.Pow(1 - p, fadePower);
                }
            }

            wasPlaying = audioSource.isPlaying;
        }

#endif
    }
}
