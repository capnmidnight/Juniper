using Juniper.Display;
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

namespace Juniper.Animation
{
    /// <summary>
    /// Performs a fade-in/fade-out animation on scene transitions.
    /// </summary>
    public class FadeTransition : AbstractTransitionController, IInstallable
    {
        private JuniperPlatform xr;

#if UNITY_MODULES_AUDIO
        public AudioClip fadeOutSound;

        public AudioClip fadeInSound;

        private AudioSource aud;

        public override float TransitionLength =>
            aud?.clip?.length ?? 0;

#else
        public float fadeLength;

        public override float TransitionLength =>
            fadeLength;

#endif

        /// <summary>
        /// Setup the necessary gameObjects and components to make a fader box appear in front of the camera.
        /// </summary>
        /// <returns></returns>
        public static PooledComponent<FadeTransition> Ensure(Transform parent) =>
            parent.EnsureTransform("Fader", () =>
                GameObject.CreatePrimitive(PrimitiveType.Quad))
                .Value
                .EnsureComponent<FadeTransition>(
#if UNITY_MODULES_PHYSICS
                    (fader) => fader.RemoveComponent<Collider>()
#endif
                );

        /// <summary>
        /// When the object starts up, it looks for the master system object.
        /// </summary>
        public void Awake()
        {
            Install(false);

#if UNITY_MODULES_AUDIO

#if UNITY_EDITOR
            fadeOutSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(System.IO.PathExt.FixPath("Assets/Juniper/Audio/Star Trek/hologram_off_2.mp3"));
            fadeInSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(System.IO.PathExt.FixPath("Assets/Juniper/Audio/Star Trek/hologrid_online.mp3"));
#endif

            aud.playOnAwake = false;
            aud.clip = fadeOutSound;
            Exited += FadeTransition_Exited;
#endif

            r = GetComponent<Renderer>();
            props = new MaterialPropertyBlock();

            Entered += FadeTransition_Entered;

            xr = ComponentExt.FindAny<JuniperPlatform>();
        }

        public virtual void Reinstall() =>
            Install(true);

#if UNITY_EDITOR
        public void Reset() =>
            Reinstall();
#endif

        public void Install(bool reset)
        {
            reset &= Application.isEditor;

#if UNITY_MODULES_AUDIO
            aud = this.EnsureComponent<AudioSource>();
#endif
        }

        public void Uninstall() { }

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
        /// Start the fade in transition.
        /// </summary>
        /// <returns></returns>
        public override void Enter()
        {
#if UNITY_MODULES_AUDIO
            aud.Play();
#endif
            base.Enter();
        }

        /// <summary>
        /// Start the fade out transition.
        /// </summary>
        /// <returns></returns>
        public override void Exit()
        {
#if UNITY_MODULES_AUDIO
            aud.Play();
#endif
            base.Exit();
        }

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

        private readonly List<Action> actions = new List<Action>();

#if UNITY_MODULES_AUDIO
        private void FadeTransition_Exited(object sender, EventArgs e) =>
            aud.clip = fadeOutSound;
#endif

        private void FadeTransition_Entered(object sender, EventArgs e)
        {
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

        private Color lastColor;
        private int lastCullingMask;
        private AmbientMode lastAmbientMode;

        protected override void OnEntered()
        {
            base.OnEntered();

            lastColor = DisplayManager.MainCamera.backgroundColor;
            lastCullingMask = DisplayManager.MainCamera.cullingMask;
            lastAmbientMode = RenderSettings.ambientMode;

            if (xr.ARMode == AugmentedRealityTypes.None)
            {
                DisplayManager.MainCamera.clearFlags = CameraClearFlags.Color;
                DisplayManager.MainCamera.backgroundColor = ColorExt.TransparentBlack;
            }

            DisplayManager.MainCamera.cullingMask = LayerMask.GetMask("TransparentFX");
            RenderSettings.ambientMode = AmbientMode.Flat;
        }

        protected override void OnExiting()
        {
            base.OnExiting();

            if (xr.ARMode == AugmentedRealityTypes.None)
            {
                DisplayManager.MainCamera.clearFlags = CameraClearFlags.Skybox;
                DisplayManager.MainCamera.backgroundColor = lastColor;
            }

            DisplayManager.MainCamera.cullingMask = lastCullingMask;
            RenderSettings.ambientMode = lastAmbientMode;
        }
    }
}
