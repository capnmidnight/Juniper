using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.Display;
using Juniper.Haptics;
using Juniper.Input;
using Juniper.IO;
using Juniper.Security;

using UnityEngine;

using Juniper.Speech.Azure.CognitiveServices;

#if UNITY_MODULES_AUDIO

using System.Collections.Generic;
using System.Linq;

using UnityEngine.Audio;

#endif

namespace Juniper.Sound
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
#if UNITY_EDITOR
        , ICredentialReceiver
#endif
    {
        private const string INTERACTION_SOUND_TAG = "InteractionSound";

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

                case Interaction.PressedDisabled:
                case Interaction.ClickedDisabled:
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
        /// A sub directory in StreamingAssets to which TTS audio files will be cached.
        /// </summary>
        public string CachePrefix;

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

        public bool IsTextToSpeechAvailable
        {
            get
            {
                return tts != null
                    && tts.IsAvailable == true;
            }
        }

        [SerializeField]
        [HideInNormalInspector]
        private string azureApiKey;

        [SerializeField]
        [HideInNormalInspector]
        private string azureRegion;

        [SerializeField]
        [HideInNormalInspector]
        private string azureResourceName;

        private TextToSpeechClient tts;

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

        public string CredentialFile
        {
            get
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var assetsRoot = Path.Combine(userProfile, "Box", "VR Initiatives", "Engineering", "Assets");
                var keyFile = Path.Combine(assetsRoot, "DevKeys", "azure-speech.txt");
                return keyFile;
            }
        }

        public void ReceiveCredentials(string[] args)
        {
            if (args == null)
            {
                azureApiKey = null;
                azureRegion = null;
                azureResourceName = null;
            }
            else
            {
                azureApiKey = args[0];
                azureRegion = args[1];
                azureResourceName = args[2];
            }
        }

        public void Reset()
        {
            Reinstall();
        }

#endif

        public virtual void Uninstall()
        {
#if UNITY_MODULES_AUDIO
            DisplayManager.MainCamera.Ensure<AudioListener>();

            FindAudioSources();
            foreach (var audioSource in audioSources)
            {
                UninstallSpatialization(audioSource);
            }
#endif
        }

        /// <summary>
        /// Gets the system configuration, sets up the default haptics, provisions the interaction
        /// sound library, and initializes the spatial audio engine (if one was selected).
        /// </summary>
        public virtual void Awake()
        {
#if UNITY_EDITOR
            this.ReceiveCredentials();
#endif
            Install(false);

            camT = DisplayManager.MainCamera.transform;

#if UNITY_MODULES_AUDIO
            startUp = new SingleAudioClipCollection(soundOnStartUp, false);
            shutDown = new SingleAudioClipCollection(soundOnShutDown, false);

            InitializeInteractionAudioSources();
#endif

            if (!string.IsNullOrEmpty(azureRegion)
                && !string.IsNullOrEmpty(azureApiKey)
                && !string.IsNullOrEmpty(azureResourceName))
            {
                var cache = GetCachingStrategy();

                tts = new TextToSpeechClient(
                    azureRegion,
                    azureApiKey,
                    azureResourceName,
                    new JsonFactory<Voice[]>(),
#if UNITY_STANDALONE_WIN || UNITY_WSA
                    AudioFormat.Audio24KHz160KbitrateMonoMP3,
#else
                    AudioFormat.Riff16KHz16BitMonoPCM,
#endif
                    new NAudioAudioDataDecoder(),
                    cache);
            }
        }

        public CachingStrategy GetCachingStrategy()
        {
            if (string.IsNullOrEmpty(CachePrefix))
            {
                return new CachingStrategy
                {
                    new StreamingAssetsCacheLayer(Path.Combine("Azure", "CognitiveServices"))
                };
            }
            else
            {
                return new CachingStrategy
                {
                    new StreamingAssetsCacheLayer(Path.Combine(CachePrefix, "Azure", "CognitiveServices"))
                };
            }
        }

        public async Task<AudioClip> PreloadSpeech(string text, string voiceName, float rateChange, float pitchChange)
        {
            using (var audioData = await tts.GetDecodedAudioAsync(text, voiceName, rateChange, pitchChange))
            {
                var reader = new BinaryReader(audioData.DataStream);
                var clip = AudioClip.Create(
                    text,
                    (int)audioData.Samples,
                    audioData.Format.Channels,
                    audioData.Format.SampleRate,
                    true,
                    floats =>
                    {
                        for (var i = 0; i < floats.Length; ++i)
                        {
                            floats[i] = reader.ReadSingle();
                        }
                    },
                    pos =>
                    {
                        reader.BaseStream.Position = pos;
                    });
                clip.LoadAudioData();
                return clip;
            }
        }

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

                case Interaction.DraggingStarted:
                case Interaction.Dragged:
                case Interaction.DraggingEnded:
                return soundOnDragged;

                case Interaction.Scrolled:
                return soundOnScrolled;

                case Interaction.Entered:
                case Interaction.EnteredDisabled:
                return soundOnEntered;

                case Interaction.Error:
                return soundOnError;

                case Interaction.Exited:
                return soundOnExited;

                case Interaction.PressedDisabled:
                return soundOnDisabled;

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

        public float PlayAction(Interaction action, Transform pose, AbstractHapticDevice haptics, bool randomizePitch)
        {
            if (action == Interaction.None)
            {
                return 0;
            }
            else
            {
                if (haptics != null)
                {
                    var expr = GetHapticExpressions(action);
                    if (expr != HapticExpression.None)
                    {
                        haptics.Play(expr);
                    }
                }

#if UNITY_MODULES_AUDIO
                var clips = GetClipCollection(action);
                var reallyRandomize = randomizePitch && clips?.RandomizeClipPitch == true;
                var clip = clips?.Random();
                if (clip != null)
                {
                    return PlayAudioClip(clip, pose, reallyRandomize);
                }
#endif
                return 0;
            }
        }

        public float PlayAction(Interaction action, Transform pose, AbstractHapticDevice haptics)
        {
            return PlayAction(action, pose, haptics, false);
        }

        public float PlayAction(Interaction action, Transform pose, bool randomizePitch)
        {
            return PlayAction(action, pose, null, randomizePitch);
        }

        public float PlayAction(Interaction action, Transform pose)
        {
            return PlayAction(action, pose, null, false);
        }

        public float PlayAction(Interaction action, AbstractHapticDevice haptics, bool randomizePitch)
        {
            return PlayAction(action, camT, haptics, randomizePitch);
        }

        public float PlayAction(Interaction action, AbstractHapticDevice haptics)
        {
            return PlayAction(action, camT, haptics, false);
        }

        public float PlayAction(Interaction action, bool randomizePitch)
        {
            return PlayAction(action, camT, null, randomizePitch);
        }

        public float PlayAction(Interaction action)
        {
            return PlayAction(action, camT, null, false);
        }

        public float PlayAudioClip(AudioClip clip, Transform pose, bool randomizePitch)
        {
#if UNITY_MODULES_AUDIO
            var audioSource = GetAudioSource(clip);
            audioSource.loop = false;
            if (randomizePitch)
            {
                audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            }
            audioSource.transform.position = pose.position;
            var deltaP = listener.transform.position - audioSource.transform.position;
            if (deltaP.sqrMagnitude < 0.0625f)
            {
                audioSource.transform.position +=
                    0.25f * listener.transform.forward
                    + 0.125f * listener.transform.up
                    + 0.05f * listener.transform.right;
            }
            audioSource.transform.rotation = pose.rotation;
            audioSource.Activate();
            audioSource.Play();

            return audioSource.clip.length;
#else
            return 0;
#endif
        }

        public float PlayAudioClip(AudioClip clip, Transform pose)
        {
            return PlayAudioClip(clip, pose, false);
        }
        public float PlayAudioClip(AudioClip clip, bool randomizePitch)
        {
            return PlayAudioClip(clip, camT, randomizePitch);
        }

        public float PlayAudioClip(AudioClip clip)
        {
            return PlayAudioClip(clip, camT, false);
        }

        public async Task EnableDopplerAsync(bool value)
        {
#if UNITY_MODULES_AUDIO
            await JuniperSystem.OnMainThreadAsync(() =>
            {
                var audioSources = Find.All<AudioSource>();
                var doppler = value ? dopplerLevel : 0;
                foreach (var audioSource in audioSources)
                {
                    audioSource.dopplerLevel = doppler;
                }
            });
#endif

            // burn a frame
            await Task.Yield();
        }

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

        protected AudioListener listener;

        private void InitializeInteractionAudioSources()
        {
            FindAudioSources();
            for (var i = audioSources.Count; i < 5; ++i)
            {
                CreateNewAudioSource();
            }
        }

        private void FindAudioSources()
        {
            Find.All(hasInteractionSoundTag, ref audioSources);
        }

        private static readonly Func<AudioSource, bool> hasInteractionSoundTag = a =>
            a.CompareTag(INTERACTION_SOUND_TAG);


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

        public AudioSource Spatialize(AudioSource audioSource, AudioMixerGroup group)
        {
            return InternalSpatialize(audioSource, group);
        }

        public AudioSource Spatialize(AudioSource audioSource)
        {
            return InternalSpatialize(audioSource, defaultMixerGroup);
        }

        public void SetDefaultMixerGroup(AudioSource audioSource)
        {
            audioSource.outputAudioMixerGroup = defaultMixerGroup;
        }

        protected virtual void UninstallSpatialization(AudioSource audioSource)
        { }

        /// <summary>
        /// The audio mixer to use with ResonanceAudio
        /// </summary>
        [Header("Configuration")]
        public AudioMixerGroup defaultMixerGroup;

        [Range(0, 5)]
        public float dopplerLevel = 0.68f;

        protected virtual string DefaultAudioMixer => "defaultAudioMixer";


#if UNITY_EDITOR
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

            ConfigurationManagement.TagManager.NormalizeTag(INTERACTION_SOUND_TAG);
            InitializeInteractionAudioSources();
        }
#endif

        /// <summary>
        /// Find or create a free Audio Source out of the pool.
        /// </summary>
        /// <returns>The audio source.</returns>
        private AudioSource GetAudioSource(AudioClip clip)
        {
            AudioSource source = null;
            foreach (var a in audioSources)
            {
                if (source == null && !a.isPlaying
                    || source != null && a.clip == clip && source.clip != clip)
                {
                    source = a;
                }
            }

            if (source == null)
            {
                source = CreateNewAudioSource();
            }

            source.clip = clip;

            return source;
        }

        private AudioSource CreateNewAudioSource()
        {
            var name = "AudioSource" + (audioSources.Count + 1).ToString("00");
            var audioSource = Spatialize(
                new GameObject(name).AddComponent<AudioSource>(),
                defaultMixerGroup);
            audioSources.Add(audioSource);
            audioSource.tag = INTERACTION_SOUND_TAG;
            return audioSource;
        }

        /// <summary>
        /// Set the common spatialization parameters for an AudioSource, including adding the
        /// necessary spatialization component for the platforms spatialization plugin.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="group">      </param>
        /// 
        /// <returns></returns>
        protected virtual AudioSource InternalSpatialize(AudioSource audioSource, AudioMixerGroup group)
        {
            audioSource.outputAudioMixerGroup = group == null
                ? defaultMixerGroup
                : group;
            audioSource.spatialBlend = 1;
            audioSource.spatialize = true;
            audioSource.spatializePostEffects = true;
            audioSource.dopplerLevel = dopplerLevel;

            return audioSource;
        }

        public void StopZone(string zoneName)
        {
            JuniperSystem.OnMainThread(() =>
            {
                var audioSources = Find.All<AudioSource>(a => !a.CompareTag(INTERACTION_SOUND_TAG));
                foreach (var audioSource in audioSources)
                {
                    if (audioSource.IsInZone(zoneName)
                        && audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                }
            });
        }

        public void PauseZone(string zoneName)
        {
            JuniperSystem.OnMainThread(() =>
            {
                var audioSources = Find.All<AudioSource>(a => !a.CompareTag(INTERACTION_SOUND_TAG));
                foreach (var audioSource in audioSources)
                {
                    if (audioSource.IsInZone(zoneName)
                        && audioSource.isPlaying)
                    {
                        audioSource.Pause();
                    }
                }
            });
        }

        public void PlayZone(string zoneName)
        {
            JuniperSystem.OnMainThread(() =>
            {
                var audioSources = Find.All<AudioSource>(a => !a.CompareTag(INTERACTION_SOUND_TAG));
                foreach (var audioSource in audioSources)
                {
                    if (!audioSource.IsInZone(zoneName)
                        && audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                    else if (audioSource.playOnAwake
                        && !audioSource.isPlaying)
                    {
                        audioSource.Activate();
                        audioSource.Play();
                    }
                }
            });
        }
#endif
    }
}