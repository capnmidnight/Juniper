using System;
using System.Collections;

using Juniper.Haptics;
using Juniper.Widgets;
using Juniper.Display;
using Juniper.Input;

using UnityEngine;

#if UNITY_MODULES_AUDIO

using System.Collections.Generic;
using System.Linq;

using UnityEngine.Audio;

#endif

namespace Juniper.Audio
{
    /// <summary>
    /// The audio portion of the interaction system.
    /// </summary>
    public class InteractionAudio :
#if UNITY_GOOGLE_RESONANCE_AUDIO
        ResonanceInteractionAudio
#elif UNITY_XR_OCULUS
        OculusInteractionAudio
#elif UNITY_XR_WINDOWSMR_METRO
        WindowsMRInteractionAudio
#elif UNITY_XR_MAGICLEAP
        MagicLeapInteractionAudio
#else
        DefaultInteractionAudio
#endif
    {
    }

    /// <summary>
    /// The audio portion of the interaction system.
    /// </summary>
    public abstract class AbstractInteractionAudio : MonoBehaviour, IInstallable
    {
        /// <summary>
        /// Converts an Interaction into a haptic expression to be able to play.
        /// </summary>
        /// <returns>The haptic expressions.</returns>
        /// <param name="action">Action.</param>
        public static HapticExpression GetHapticExpressions(Interaction action)
        {
            switch (action)
            {
                case Interaction.Closed:
                return HapticExpression.Heavy;

                case Interaction.Disabled:
                return HapticExpression.Warning;

                case Interaction.DraggingStarted:
                case Interaction.Dragged:
                case Interaction.DraggingEnded:
                return HapticExpression.SelectionChange;

                case Interaction.Error:
                return HapticExpression.Error;

                case Interaction.Opened:
                return HapticExpression.Medium;

                case Interaction.Clicked:
                return HapticExpression.Light;

                case Interaction.Success:
                return HapticExpression.Success;

                default:
                return HapticExpression.None;
            }
        }

        /// <summary>
        /// Play the specified action at the camera position using the specified haptics, with no callback.
        /// </summary>
        /// <returns>The play.</returns>
        /// <param name="action"> Action.</param>
        /// <param name="haptics">Haptics.</param>
        public static float Play(Interaction action, AbstractHapticDevice haptics)
        {
            return instance?.Play_Internal(action, haptics) ?? 0;
        }

        /// <summary>
        /// Play the specified action using the specified pose and haptic subsystem, with no callback.
        /// </summary>
        /// <returns>The play.</returns>
        /// <param name="action"> Action.</param>
        /// <param name="pose">   Pose.</param>
        /// <param name="haptics">Haptics.</param>
        public static float Play(Interaction action, Transform pose, AbstractHapticDevice haptics)
        {
            return instance?.Play_Internal(action, pose, haptics) ?? 0;
        }

        /// <summary>
        /// Play the specified action at the camera position using the specified haptics, with a
        /// callback on completion.
        /// </summary>
        /// <returns>The play.</returns>
        /// <param name="action">    Action.</param>
        /// <param name="haptics">   Haptics.</param>
        /// <param name="onComplete"></param>
        public static float Play(Interaction action, AbstractHapticDevice haptics, Action onComplete)
        {
            return instance?.Play_Internal(action, haptics, onComplete) ?? 0;
        }

        /// <summary>
        /// Play the specified action using the specified pose and haptic subsystem, with a callback
        /// on completion.
        /// </summary>
        /// <returns>The play.</returns>
        /// <param name="action">    Action.</param>
        /// <param name="pose">      Pose.</param>
        /// <param name="haptics">   Haptics.</param>
        /// <param name="onComplete"></param>
        public static float Play(Interaction action, Transform pose, AbstractHapticDevice haptics, Action onComplete)
        {
            return instance?.Play_Internal(action, pose, haptics, onComplete) ?? 0;
        }

#if UNITY_MODULES_AUDIO

        public AudioSource Spatialize(AudioSource audioSource, bool loop, AudioMixerGroup group)
        {
            return InternalSpatialize(audioSource, loop, group);
        }

        public AudioSource Spatialize(AudioSource audioSource, bool loop)
        {
            return InternalSpatialize(audioSource, loop, defaultMixerGroup);
        }

        public AudioSource Spatialize(AudioSource audioSource, AudioMixerGroup group)
        {
            return InternalSpatialize(audioSource, false, group);
        }

        public AudioSource Spatialize(AudioSource audioSource)
        {
            return InternalSpatialize(audioSource, false, defaultMixerGroup);
        }

        /// <summary>
        /// The audio mixer to use with ResonanceAudio
        /// </summary>
        [Header("Configuration")]
        public AudioMixerGroup defaultMixerGroup;

#endif

        public GameObject volumeSlider;

        /// <summary>
        /// The sound to play when an <see cref="Widgets.Openable"/> object has been closed.
        /// </summary>
        [Header("Interactions")]
        public AudioClipCollection soundOnClosed;

        /// <summary>
        /// The sound to play when any <see cref="Widgets.Touchable"/> that has been set to disabled
        /// has been clicked.
        /// </summary>
        public AudioClipCollection soundOnDisabled;

        /// <summary>
        /// The sound to play when a <see cref="Widgets.Draggable"/> has been moved more than 10cm.
        /// </summary>
        public AudioClipCollection soundOnDragged;

        /// <summary>
        /// The sound to play when a cursor first hovers over any <see cref="Widgets.Touchable"/>.
        /// </summary>
        public AudioClipCollection soundOnEntered;

        /// <summary>
        /// The sound to play when an operation needs to indicate an error.
        /// </summary>
        public AudioClipCollection soundOnError;

        /// <summary>
        /// The sound to play when a cursor leaves a <see cref="Widgets.Touchable"/> and is no longer
        /// hovering over it.
        /// </summary>
        public AudioClipCollection soundOnExited;

        /// <summary>
        /// The sound to play when an <see cref="Widgets.Openable"/> object has been opened.
        /// </summary>
        public AudioClipCollection soundOnOpened;

        /// <summary>
        /// The sound to play when the primary selection button is no longer being pressed on a <see cref="Widgets.Clickable"/>.
        /// </summary>
        public AudioClipCollection soundOnReleased;

        /// <summary>
        /// The sound to play when a scrollable section is moved a certain distance.
        /// </summary>
        public AudioClipCollection soundOnScrolled;

        /// <summary>
        /// The sound to play when the primary selection button is first being pressed on a <see cref="Widgets.Clickable"/>.
        /// </summary>
        public AudioClipCollection soundOnSelected;

        /// <summary>
        /// The sound to play when a long-running operating needs to indicate success.
        /// </summary>
        public AudioClipCollection soundOnSuccess;

#if UNITY_MODULES_AUDIO

        /// <summary>
        /// The sound to play when the application first starts up.
        /// </summary>
        [Header("Transitions")]
        public AudioClip soundOnStartUp;

        /// <summary>
        /// The sound to play right before the application shuts down.
        /// </summary>
        public AudioClip soundOnShutDown;

        private const string MASTER_VOLUME_KEY = "MasterVolume";

        private IValuedControl<float> volume;

        protected AudioListener listener;
#endif

        public virtual void Install(bool reset)
        {
#if UNITY_MODULES_AUDIO
            listener = DisplayManager.MainCamera.Ensure<AudioListener>();
#endif
        }

        public void Reinstall()
        {
            Install(true);
        }

#if UNITY_EDITOR

        public void Reset()
        {
            Reinstall();
        }

#endif

        public virtual void Uninstall()
        {
#if UNITY_MODULES_AUDIO
            DisplayManager.MainCamera.Ensure<AudioListener>();
#endif
        }

        /// <summary>
        /// Gets the system configuration, sets up the default haptics, provisions the interaction
        /// sound library, and initializes the spatial audio engine (if one was selected).
        /// </summary>
        public virtual void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("Juniper: There can be only one! (R2D2)");
                this.Destroy();
            }
            else
            {
                Install(false);

                instance = this;
                camT = DisplayManager.MainCamera.transform;

#if UNITY_MODULES_AUDIO
                startUp = new SingleAudioClipCollection(soundOnStartUp, false);
                shutDown = new SingleAudioClipCollection(soundOnShutDown, false);

                audioSources = new List<AudioSource>(10);

                if (volumeSlider != null)
                {
                    volume = volumeSlider.GetSlider();
                    volume.ValueChange += Volume_ValueChange;
                }
#endif
            }
        }

        private void Volume_ValueChange(object sender, float value)
        {
            Volume = value;
        }

        private static AbstractInteractionAudio instance;

#if UNITY_MODULES_AUDIO

        /// <summary>
        /// The sound to play on application startup.
        /// </summary>
        private IAudioClipCollection startUp;

        /// <summary>
        /// The sound to play on application shutdown.
        /// </summary>
        private IAudioClipCollection shutDown;

        /// <summary>
        /// A pool of audio sources that can be moved around to play sounds wherever they are needed.
        /// The <see cref="Play(Interaction, Transform, AbstractHapticDevice, Action)"/> method will
        /// find whichever one is currently not playing and use it, or create a new one if all audio
        /// sources are currently playing. It starts out with 5 audio sources by default so that
        /// applications don't have to grow the collection right away.
        /// </summary>
        private List<AudioSource> audioSources;

#endif

        /// <summary>
        /// The main camera, which also has the user's ears.
        /// </summary>
        private Transform camT;

        /// <summary>
        /// Converts an Interaction into a set of audio clips to be able to play.
        /// </summary>
        /// <returns>The audio clip.</returns>
        /// <param name="action">Action.</param>
        private IAudioClipCollection GetClipCollection(Interaction action)
        {
            switch (action)
            {
                case Interaction.Closed:
                return soundOnClosed;

                case Interaction.Disabled:
                return soundOnDisabled;

                case Interaction.DraggingStarted:
                case Interaction.Dragged:
                case Interaction.DraggingEnded:
                return soundOnDragged;

                case Interaction.Scrolled:
                return soundOnScrolled;

                case Interaction.Entered:
                return soundOnEntered;

                case Interaction.Error:
                return soundOnError;

                case Interaction.Exited:
                return soundOnExited;

                case Interaction.Opened:
                return soundOnOpened;

                case Interaction.Released:
                return soundOnReleased;

                case Interaction.Clicked:
                return soundOnSelected;

                case Interaction.Success:
                return soundOnSuccess;

#if UNITY_MODULES_AUDIO
                case Interaction.StartUp:
                return startUp;

                case Interaction.ShutDown:
                return shutDown;
#endif

                default:
                return null;
            }
        }

        /// <summary>
        /// Waits a set number of seconds and then executes a callback function.
        /// </summary>
        /// <returns>The wait.</returns>
        /// <param name="seconds">   Seconds.</param>
        /// <param name="onComplete">On complete.</param>
        private static IEnumerator Wait(float seconds, Action onComplete)
        {
            yield return new WaitForSeconds(seconds);
            onComplete();
        }

        private float Play_Internal(Interaction action, AbstractHapticDevice haptics)
        {
            return Play_Internal(action, camT, haptics, null);
        }

        private float Play_Internal(Interaction action, Transform pose, AbstractHapticDevice haptics)
        {
            return Play_Internal(action, pose, haptics, null);
        }

        private float Play_Internal(Interaction action, AbstractHapticDevice haptics, Action onComplete)
        {
            return Play_Internal(action, camT, haptics, onComplete);
        }

        private float Play_Internal(Interaction action, Transform pose, AbstractHapticDevice haptics, Action onComplete)
        {
            if (action == Interaction.None)
            {
                return 0;
            }
            else
            {
                PlayHaptics(action, haptics);
                var len = PlayClip(action, pose);
                if (onComplete != null)
                {
                    StartCoroutine(Wait(len, onComplete));
                }
                return len;
            }
        }

        /// <summary>
        /// Executes the haptic feedback portion of the interaction.
        /// </summary>
        /// <param name="action"> Action.</param>
        /// <param name="haptics">Haptics.</param>
        private static void PlayHaptics(Interaction action, AbstractHapticDevice haptics)
        {
            var expr = GetHapticExpressions(action);
            if (expr != HapticExpression.None)
            {
                haptics?.Play(expr);
            }
        }

        /// <summary>
        /// Plays the audio portion of the interaction.
        /// </summary>
        /// <returns>The clip.</returns>
        /// <param name="action">Action.</param>
        /// <param name="pose">  Pose.</param>
        private float PlayClip(Interaction action, Transform pose)
        {
#if UNITY_MODULES_AUDIO
            var clips = GetClipCollection(action);
            var clip = clips?.Random();
            if (clip != null)
            {
                var audioSource = GetAudioSource(clip);
                if (clips.RandomizeClipPitch)
                {
                    audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                }
                audioSource.transform.position = pose.position;
                audioSource.transform.rotation = pose.rotation;
                audioSource.Play();

                return audioSource.clip.length;
            }
#endif
            return 0;
        }

#if UNITY_MODULES_AUDIO

        private const float E = (float)Math.E;

        public void Start()
        {
            for (var i = 0; i < 5; ++i)
            {
                CreateNewAudioSource();
            }

            Volume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1);

            if (volume != null)
            {
                volume.value = Volume;
            }
        }

        protected virtual string DefaultAudioMixer
        {
            get
            {
                return "defaultAudioMixer";
            }
        }

        public void OnValidate()
        {
            if (defaultMixerGroup == null)
            {
                defaultMixerGroup =
                    (from g in Resources.FindObjectsOfTypeAll<AudioMixerGroup>()
                     where g.audioMixer.name == DefaultAudioMixer
                     orderby g.name == "Master" descending
                     select g)
                        .FirstOrDefault();
            }

            if (defaultMixerGroup != null)
            {
                Volume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1);
            }
        }

        /// <summary>
        /// Find or create a free Audio Source out of the pool.
        /// </summary>
        /// <returns>The audio source.</returns>
        private AudioSource GetAudioSource(AudioClip clip)
        {
            var source = audioSources
                .Where(a => !a.isPlaying)
                .OrderByDescending(a => a.clip == clip)
                .FirstOrDefault()
                ?? CreateNewAudioSource();
            source.clip = clip;
            return source;
        }

        private AudioSource CreateNewAudioSource()
        {
            var audioSource = Spatialize(
                new GameObject().AddComponent<AudioSource>(),
                false,
                defaultMixerGroup);
            audioSources.Add(audioSource);
            audioSource.name = "AudioSource" + audioSources.Count.ToString("00");
            return audioSource;
        }

#endif

        public float Volume
        {
            set
            {
#if UNITY_MODULES_AUDIO
                PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, value);
                PlayerPrefs.Save();

                var log = Mathf.Log(value) * 11.5f;
                if (float.IsInfinity(log))
                {
                    log = -1000;
                }

                defaultMixerGroup
                    ?.audioMixer
                    ?.SetFloat("Volume", log);
#endif
            }

            get
            {
                float value = 0;
#if UNITY_MODULES_AUDIO
                if (defaultMixerGroup?.audioMixer?.GetFloat("Volume", out value) == true)
                {
                    value = Mathf.Pow(E, value / 11.5f);
                }
#endif
                return value;
            }
        }

#if UNITY_MODULES_AUDIO

        /// <summary>
        /// Set the common spatialization parameters for an AudioSource, including adding the
        /// necessary spatialization component for the platforms spatialization plugin.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="loop">       </param>
        /// <param name="group">      </param>
        /// <returns></returns>
        protected virtual AudioSource InternalSpatialize(AudioSource audioSource, bool loop, AudioMixerGroup group)
        {
            audioSource.loop = loop;
            audioSource.outputAudioMixerGroup = group ?? defaultMixerGroup;
            audioSource.spatialBlend = 1;
            audioSource.spatialize = true;
            audioSource.spatializePostEffects = true;

            return audioSource;
        }

#endif
    }
}