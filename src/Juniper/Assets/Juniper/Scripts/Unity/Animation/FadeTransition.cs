using Juniper.Unity.Display;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

namespace Juniper.Unity.Animation
{
    /// <summary>
    /// Performs a fade-in/fade-out animation on scene transitions.
    /// </summary>
    public class FadeTransition : AbstractTransitionController, IInstallable
    {
        /// <summary>
        /// Used to check the current ARMode to determine how to restore the camera clear mode.
        /// </summary>
        private DisplayManager display;

#if UNITY_MODULES_AUDIO

        /// <summary>
        /// The sound to play when the view fades out.
        /// </summary>
        public AudioClip fadeOutSound;

        /// <summary>
        /// The sound to play when the view fades back in.
        /// </summary>
        public AudioClip fadeInSound;

        /// <summary>
        /// The audio source on which to play the fade-in/fade-out sounds.
        /// </summary>
        private AudioSource aud;

#endif

        /// <summary>
        /// If Unity's audio subsystem is not available, or the fade-in/fade-out sounds are not
        /// configured, this is the default length of time to run the fade-in/fade-out transition.
        /// </summary>
        public float fadeLength = 0.25f;

        /// <summary>
        /// The amount of time the fade transition takes to complete. If a fade sound is provided,
        /// this will be based on the length of the sound clip. Otherwise, it's 1/4th of a second.
        /// </summary>
        public override float TransitionLength
        {
            get
            {
                return
#if UNITY_MODULES_AUDIO
                    aud?.clip?.length ??
#endif
                    fadeLength;
            }
        }

        /// <summary>
        /// Setup the necessary gameObjects and components to make a fader box appears in front of
        /// the camera.
        /// </summary>
        /// <returns></returns>
        public static PooledComponent<FadeTransition> Ensure(Transform parent)
        {
            return parent.EnsureTransform("Fader", () =>
                GameObject.CreatePrimitive(PrimitiveType.Quad))
                    .Value
                    .EnsureComponent<FadeTransition>(
#if UNITY_MODULES_PHYSICS
                    (fader) => fader.RemoveComponent<Collider>()
#endif
                    );
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

            display = ComponentExt.FindAny<DisplayManager>();
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
        public bool Install(bool reset)
        {
            reset &= Application.isEditor;

#if UNITY_MODULES_AUDIO
#if UNITY_EDITOR
            if (reset)
            {
                fadeOutSound = ComponentExt.EditorLoadAsset<AudioClip>("Assets/Juniper/Audio/Star Trek/hologram_off_2.mp3");
                fadeInSound = ComponentExt.EditorLoadAsset<AudioClip>("Assets/Juniper/Audio/Star Trek/hologrid_online.mp3");
            }
#endif

            aud = this.EnsureComponent<AudioSource>();
            aud.playOnAwake = false;
            aud.clip = fadeOutSound;
#endif

            return true;
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
        private readonly List<Action> actions = new List<Action>();

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
                actions.Add(() =>
                {
                    act();
                    Exit();
                });
            }
            else
            {
                actions.Add(act);
            }
            Enter();
        }

        /// <summary>
        /// The clear-color that was used before the fade-out transition started.
        /// </summary>
        private Color lastColor;

        /// <summary>
        /// The culling-mask that was used before the fade-out transition started.
        /// </summary>
        private int lastCullingMask;

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
            aud.Play();
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

            lastColor = DisplayManager.MainCamera.backgroundColor;
            lastCullingMask = DisplayManager.MainCamera.cullingMask;
            lastAmbientMode = RenderSettings.ambientMode;

            if (display.ARMode == AugmentedRealityTypes.None)
            {
                DisplayManager.MainCamera.clearFlags = CameraClearFlags.Color;
                DisplayManager.MainCamera.backgroundColor = ColorExt.TransparentBlack;
            }

            DisplayManager.MainCamera.cullingMask = LayerMask.GetMask("TransparentFX");
            RenderSettings.ambientMode = AmbientMode.Flat;

#if UNITY_MODULES_AUDIO
            aud.clip = fadeInSound;
#endif

            foreach (var action in actions)
            {
                try
                {
                    action();
                }
                catch (Exception exp)
                {
                    Debug.LogError(exp);
                }
            }

            actions.Clear();
        }

        /// <summary>
        /// Restores the camera's rendering and lighting modes to what it was before the fade-out.
        /// </summary>
        protected override void OnExiting()
        {
            base.OnExiting();

            if (display.ARMode == AugmentedRealityTypes.None)
            {
                DisplayManager.MainCamera.clearFlags = CameraClearFlags.Skybox;
                DisplayManager.MainCamera.backgroundColor = lastColor;
            }

            DisplayManager.MainCamera.cullingMask = lastCullingMask;
            RenderSettings.ambientMode = lastAmbientMode;
        }

        /// <summary>
        /// If Unity's audio subsystem is available, switches the audio source to use the fade-out
        /// sound for the next transition.
        /// </summary>
        protected override void OnExited()
        {
#if UNITY_MODULES_AUDIO
            aud.clip = fadeOutSound;
#endif
        }
    }
}