using System;
using System.Collections.Generic;

using Juniper.Display;
using Juniper.Input;
using Juniper.IO;
using Juniper.XR;

using UnityEngine;
using UnityEngine.Rendering;

namespace Juniper.Animation
{
    /// <summary>
    /// Performs a fade-in/fade-out animation on scene transitions.
    /// </summary>
    public class FadeTransition : AbstractTransitionController, IInstallable
    {
#if UNITY_MODULES_AUDIO

        /// <summary>
        /// The sound to play when the view fades out.
        /// </summary>
        public AudioClip fadeOutSound;

        /// <summary>
        /// The audio source on which to play the fade-in/fade-out sounds.
        /// </summary>
        private AudioSource aud;

        public float volume = 0.5f;

#endif

        /// <summary>
        /// If Unity's audio subsystem is not available, or the fade-in/fade-out sounds are not
        /// configured, this is the default length of time to run the fade-in/fade-out transition.
        /// </summary>
        public float fadeLength = 0.5f;

        /// <summary>
        /// The amount of time the fade transition takes to complete. If a fade sound is provided,
        /// this will be based on the length of the sound clip. Otherwise, it's 1/4th of a second.
        /// </summary>
        public override float TransitionLength
        {
            get
            {
                return fadeLength / 2;
            }
        }

        /// <summary>
        /// The input module that will tell use what layer the controllers are on, so we don't fade
        /// them out.
        /// </summary>
        private UnifiedInputModule input;

        private Color fadeOutColor;

        /// <summary>
        /// Setup the necessary gameObjects and components to make a fader box appears in front of
        /// the camera.
        /// </summary>
        /// <returns></returns>
        public static PooledComponent<FadeTransition> Ensure(Transform parent)
        {
            var fader = parent.Ensure<Transform>("Fader", () =>
                GameObject.CreatePrimitive(PrimitiveType.Quad))
                .Ensure<FadeTransition>();

#if UNITY_MODULES_PHYSICS
            if (fader.IsNew)
            {
                fader.Value.Remove<Collider>();
            }
#endif

            return fader;
        }

        /// <summary>
        /// Retrieves component references and sets up event handlers.
        /// </summary>
        public void Awake()
        {
            Install(false);

            Entering += FadeTransition_Enter_Exiting;
            Exiting += FadeTransition_Enter_Exiting;

            r = GetComponent<Renderer>();
            props = new MaterialPropertyBlock();

            var material = r.GetMaterial();
            fadeOutColor = material.GetColor("_Color");

            Find.Any(out input);
        }

        /// <summary>
        /// Resets the object to its initial state.
        /// </summary>
        public virtual void Reinstall()
        {
            Install(true);
        }

#if UNITY_EDITOR

        /// <summary>
        /// Resets the object to its initial state.
        /// </summary>
        public void Reset()
        {
            Reinstall();
        }

#endif

        /// <summary>
        /// Installs necessary components and initializes their default values.
        /// </summary>
        public void Install(bool reset)
        {
#if UNITY_MODULES_AUDIO
#if UNITY_EDITOR
            if (reset)
            {
                fadeOutSound = ResourceExt.EditorLoadAsset<AudioClip>("Assets/Juniper/Assets/Audio/Star Trek/hologram_off_2.mp3");
            }
#endif

            aud = this.Ensure<AudioSource>();
            aud.playOnAwake = false;
            aud.clip = fadeOutSound;
#endif
        }

        /// <summary>
        /// There are no components to uninstall from this component.
        /// </summary>
        public void Uninstall()
        {
        }

        /// <summary>
        /// As the fade transition updates, sets the opacity of the view blocker graphic.
        /// </summary>
        /// <param name="value"></param>
        protected override void RenderValue(float value)
        {
            if (props != null)
            {
                props.SetFloat("_Alpha", Mathf.Clamp01(value));
                r.SetPropertyBlock(props);
            }
        }

        /// <summary>
        /// The renderer for the current object.
        /// </summary>
        private Renderer r;

        /// <summary>
        /// Used to change values in the material, without globally changing the material.
        /// </summary>
        private MaterialPropertyBlock props;

        /// <summary>
        /// A queue of actions to perform when the fader has finished fading out, before it fades
        /// back in.
        /// </summary>
        private readonly List<Action> actions = new List<Action>(5);

        /// <summary>
        /// Adds the provided action to a queue of actions that will be executed in between a
        /// fade-out and fade-in cycle. This is useful for hiding long processing actions that would
        /// cause the VR view to stutter.
        /// </summary>
        /// <param name="act">The action lambda expression to run</param>
        public void Fader(Action act)
        {
            if (actions.Count == 0)
            {
                actions.Add(new Action(Exit));
            }

            actions.Insert(actions.Count - 1, act);
            Enter();
        }

        /// <summary>
        /// The clear-color that was used before the fade-out transition started.
        /// </summary>
        private Color lastColor;

        /// <summary>
        /// The culling-mask that was used before the fade-out transition started.
        /// </summary>
        private int lastCameraCullingMask;

        /// <summary>
        /// The layers the controllers are rendered on. This is used to make sure the controllers
        /// are still visible while the scene is faded out.
        /// </summary>
        private int lastControllerLayer;

        /// <summary>
        /// The ambient light mode that was used before the fade-out transition was started.
        /// </summary>
        private AmbientMode lastAmbientMode;

        /// <summary>
        /// Play the fade-in/fade-out sound
        /// </summary>
        private void FadeTransition_Enter_Exiting(object sender, EventArgs e)
        {
#if UNITY_MODULES_AUDIO
            if (State == Direction.Forward)
            {
                aud.volume = volume;
                aud.Play();
            }
#endif
        }

        /// <summary>
        /// Saves the last clear color, culling mask, and ambient light mode, then prepares the
        /// camera to render the special "faded out" view, which allows TransparentFX objects to
        /// still be visible, so we can have simple UI elements still visible while the view is faded
        /// out. If Unity's audio subsystem is available, switches the audio source to use the
        /// fade-in sound for the next transition. Also, executes and queued actions that are waiting
        /// for the fade transition to complete.
        /// </summary>
        protected override void OnEntered()
        {
            base.OnEntered();

            lastColor = DisplayManager.BackgroundColor;
            lastCameraCullingMask = DisplayManager.CullingMask;
            lastControllerLayer = input.ControllerLayer;
            lastAmbientMode = RenderSettings.ambientMode;

            Find.Any(out JuniperSystem sys);

            if (sys.m_ARMode == AugmentedRealityTypes.None)
            {
                DisplayManager.ClearFlags = CameraClearFlags.Color;
                DisplayManager.BackgroundColor = fadeOutColor;
            }

            input.ControllerLayer = LayerMask.NameToLayer("TransparentFX");
            DisplayManager.CullingMask = LayerMask.GetMask("TransparentFX");
            RenderSettings.ambientMode = AmbientMode.Flat;

            foreach (var action in actions)
            {
                try
                {
                    action();
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    Debug.LogError(exp);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            actions.Clear();
        }

        /// <summary>
        /// Restores the camera's rendering and lighting modes to what it was before the fade-out.
        /// </summary>
        protected override void OnExiting()
        {
            base.OnExiting();
            Find.Any(out JuniperSystem sys);
            if (sys.m_ARMode == AugmentedRealityTypes.None)
            {
                DisplayManager.ClearFlags = CameraClearFlags.Skybox;
                DisplayManager.BackgroundColor = lastColor;
            }

            DisplayManager.CullingMask = lastCameraCullingMask;
            input.ControllerLayer = lastControllerLayer;
            RenderSettings.ambientMode = lastAmbientMode;
        }
    }
}