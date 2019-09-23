using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.SceneManagement;

using Juniper.Animation;
using Juniper.Audio;
using Juniper.Display;
using Juniper.Widgets;
using Juniper.Progress;

using UnityImage = UnityEngine.UI.Image;

using Juniper.Units;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace Juniper
{
    /// <summary>
    /// Manages loading <see cref="SubSceneController"/> components (scenes that are additively
    /// loaded into a "main" scene), as well as transitioning between them.
    /// </summary>
    [DisallowMultipleComponent]
    public class MasterSceneController : MonoBehaviour, IInstallable
    {
        /// <summary>
        /// Parse the scene name.
        /// </summary>
        private static readonly Regex SceneNamePattern = new Regex("([^/]+)\\.unity$", RegexOptions.Compiled);

        private static string GetSceneNameFromPath(string path)
        {
            return SceneNamePattern.Match(path).Groups[1].Value;
        }

        /// <summary>
        /// Get a scene by name (or path, if we're running in the editor).
        /// </summary>
        /// <returns>The scene.</returns>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="path">     Path.</param>
        private static IEnumerator LoadScene(string scenePath, string sceneName)
        {
            if (Application.isPlaying)
            {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).AsCoroutine();
            }
#if UNITY_EDITOR
            else
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
#endif
        }

        private static bool IsScenePathLoaded(string path)
        {
            var scene = SceneManager.GetSceneByPath(path);
            return scene.IsValid() && scene.isLoaded;
        }

        /// <summary>
        /// Quit out of the application, making sure any exit transitions for the current scene are
        /// ran first.
        /// </summary>
        public static void QuitApp()
        {
            var master = ComponentExt.FindAny<MasterSceneController>();
            if (master == null)
            {
                Exit();
            }
            else
            {
                master.Quit();
            }
        }

        public LoadingBar loadingBar;

        public UnityImage splash;

        /// <summary>
        /// The material to apply to Darth Fader.
        /// </summary>
        [Header("Scene fader")]
        public Material sceneFaderMaterial;

        /// <summary>
        /// All of the scene files to load. Use the "truncated scene path" format for these paths.
        /// Truncated scene paths do not need to reference the "Assets/" (it is assumed) and do not
        /// need the ".unity" file extension.
        /// </summary>
        public string[] subSceneNames;

        private FadeTransition fader;
        private InteractionAudio interaction;
        private float originalFadeVolume;
        private AudioClip originalFadeInSound;

        /// <summary>
        /// All of the active SubSceneControllers
        /// </summary>
        /// <value>The current sub scenes.</value>
        public IEnumerable CurrentSubScenes
        {
            get
            {
                return from scene in ComponentExt.FindAll<SubSceneController>()
                       where scene.isActiveAndEnabled
                       select scene;
            }
        }

#if UNITY_EDITOR

        private void SetBuildSettings()
        {
            if (!Application.isPlaying && !string.IsNullOrEmpty(gameObject?.scene.path))
            {
                var s = (from path in subSceneNames
                         where File.Exists(path)
                         select new EditorBuildSettingsScene(path, true))
                    .ToList();
                s.Insert(0, new EditorBuildSettingsScene(gameObject.scene.path, true));
                EditorBuildSettings.scenes = s.ToArray();
            }
        }

        /// <summary>
        /// Use this function by right-clicking in the editor to open up all the scenes in additive mode.
        /// </summary>
        [ContextMenu("Load Scenes")]
        private void LoadScenes_MenuItem()
        {
            using (var prog = new UnityEditorProgressDialog("Loading scenes"))
            {
                var iters = new Stack<IEnumerator>();
                iters.Push(LoadAllScenesCoroutine(prog));
                while (iters.Count > 0)
                {
                    var iter = iters.Pop();
                    while (iter.MoveNext())
                    {
                        var obj = iter.Current;
                        if (obj is IEnumerator subIter)
                        {
                            iters.Push(iter);
                            iter = subIter;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Copy all of the root scene objects out of the sub scenes and into the master scene,
        /// removing the subscenes along the way. This can also be executed from the editor by
        /// accessing the component's context menu and selecting "Flatten".
        /// </summary>
        [ContextMenu("Flatten")]
        private void Flatten()
        {
            var curScene = gameObject.scene;

            if (subSceneNames.Length > 0)
            {
                var newPath = curScene.path.Replace(".unity", ".original.unity");
                FileExt.Copy(curScene.path, newPath, true);

                foreach (var path in subSceneNames)
                {
                    var sceneName = SceneNamePattern.Match(path).Groups[1].Value;
                    var scene = SceneManager.GetSceneByPath(path);
                    if (!scene.IsValid())
                    {
                        scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    }
                    SceneManager.MergeScenes(scene, curScene);
                }

                subSceneNames = new string[0];
                EditorSceneManager.SaveScene(curScene);
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(curScene.path, true)
            };
        }

        public void OnValidate()
        {
            Invoke(nameof(SetBuildSettings), 100);
        }

        public void Reset()
        {
            Reinstall();
        }

#endif

        public void Install(bool reset)
        {
            var qualityDegrader = ComponentExt.FindAny<QualityDegrader>();
            var aud = ComponentExt.FindAny<InteractionAudio>();

            if (reset && (subSceneNames == null || subSceneNames.Length == 0))
            {
                subSceneNames = new string[] { "Assets/Juniper/Scenes/Examples/Content.unity" };
            }

            SetupFader(reset);
            SetupSystemInterface(qualityDegrader, aud);
        }

        public virtual void Reinstall()
        {
            Install(true);
        }

        public void Uninstall()
        {
        }

        private void SetupFader(bool reset)
        {
#if UNITY_EDITOR
            if (sceneFaderMaterial == null || reset)
            {
                sceneFaderMaterial = ResourceExt.EditorLoadAsset<Material>("Assets/Juniper/Materials/FaderMaterial.mat");
            }
#endif

            var fader = FadeTransition.Ensure(DisplayManager.MainCamera.transform);
            if (fader.IsNew || reset)
            {
                fader.GetComponent<Renderer>()
                    .SetMaterial(sceneFaderMaterial);
                fader.transform.localPosition = Mathf.Max(0.5f, 1.2f * DisplayManager.MainCamera.nearClipPlane) * Vector3.forward;
                fader.transform.localScale = 2 * Vector3.one;
            }
        }

        private void SetupSystemInterface(QualityDegrader qualityDegrader, InteractionAudio aud)
        {
            var sys = transform.Query("/SystemUserInterface");
#if UNITY_EDITOR
            if (sys == null)
            {
                var prefab = ResourceExt.EditorLoadAsset<GameObject>("Assets/Juniper/Prefabs/UI/SystemUserInterface.prefab");
                sys = Instantiate(prefab).transform;
                sys.name = "SystemUserInterface";
            }
#endif

            loadingBar = sys.Query<LoadingBar>("LoadingBar");

            splash = sys.Query<UnityImage>("Canvas/SplashImage");
        }

        /// <summary>
        /// Use this function by right-clicking in the editor to open up all the scenes in additive mode.
        /// </summary>
        private IEnumerator LoadAllScenesCoroutine(IProgress prog)
        {
            if (subSceneNames?.Length > 0)
            {
                for (var i = 0; i < subSceneNames.Length; ++i)
                {
                    yield return LoadScenePathCoroutine(
                        subSceneNames[i],
                        prog.Subdivide(i, subSceneNames.Length));
                }
            }
        }

        private void LoadFirstScene()
        {
            StartCoroutine(LoadFirstSceneCoroutine());
        }

        private IEnumerator LoadFirstSceneCoroutine()
        {
            var path = subSceneNames.FirstOrDefault();
            if (string.IsNullOrEmpty(path))
            {
                throw new InvalidOperationException("[Juniper.MasterSceneController::LoadFirstScene] No subscenes defined.");
            }
            else
            {
                var name = GetSceneNameFromPath(path);
                if (string.IsNullOrEmpty(name))
                {
                    throw new InvalidOperationException("[Juniper.MasterSceneController::LoadFirstScene] Could not find scene " + path);
                }
                else
                {
                    yield return SwitchToSceneCoroutine(name, true, true);
                }
            }
        }

        /// <summary>
        /// A subscene is a root game object loaded from another scene. The scenes all get loaded at
        /// runtime and then you can make different parts of it visible on the fly. This procedure
        /// deactivates any subscenes that are not the desired subscene, calling any Exit functions
        /// along the way. In the new scene, TransitionController Enter functions are called as well.
        /// It is suitable for running in a coroutine to track when the end of the switching process occurs.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public void SwitchToScene(string sceneName)
        {
            SwitchToScene(sceneName, false, true);
        }

        public void ShowScene(string sceneName)
        {
            SwitchToScene(sceneName, false, false);
        }

        /// <summary>
        /// A subscene is a root game object loaded from another scene. The scenes all get loaded at
        /// runtime and then you can make different parts of it visible on the fly. This procedure
        /// deactivates any subscenes that are not the desired subscene, calling any Exit functions
        /// along the way. In the new scene, TransitionController Enter functions are called as well.
        /// It is suitable for running in a coroutine to track when the end of the switching process occurs.
        /// </summary>
        /// <param name="sceneName">  </param>
        /// <param name="skipFadeOut">
        /// Whether or not to fade the screen out (true) or start with a black screen (false) before
        /// loading the scene.
        /// </param>
        /// <param name="unloadOtherScenes">Set to true to unload the scene file for any sub scene that
        /// indicates it shouldn't be kept around for long.</param>
        /// <returns></returns>
        private void SwitchToScene(string sceneName, bool skipFadeOut, bool unloadOtherScenes)
        {
            StartCoroutine(SwitchToSceneCoroutine(sceneName, skipFadeOut, unloadOtherScenes));
        }

        private IEnumerator SwitchToSceneCoroutine(string subSceneName, bool skipFadeOut, bool unloadOtherScenes)
        {
            if (!skipFadeOut && fader != null && !fader.IsEntered && !fader.IsEntering)
            {
                yield return fader.EnterCoroutine();
            }

            loadingBar?.Activate();
            loadingBar?.Report(0, subSceneName);

            var sceneFileName = subSceneName + ".unity";
            string scenePath = null;
            foreach (var s in subSceneNames)
            {
                if (s.EndsWith(sceneFileName))
                {
                    scenePath = s;
                    break;
                }
            }

            if (scenePath == null)
            {
                throw new Exception("Couldn't find scene: " + subSceneName);
            }

            if (unloadOtherScenes)
            {
                yield return UnloadAllScenesExcept(scenePath);
            }
            yield return LoadScenePathCoroutine(scenePath, loadingBar);
            yield return LoadingCompleteCoroutine();
        }

        private IEnumerator LoadingCompleteCoroutine()
        {
            for (var i = 1; i < SceneManager.sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);

#if UNITY_MODULES_UI
                var canvases = scene.FindAll<Canvas>((c) =>
                    c.renderMode == RenderMode.WorldSpace
                        && (c.worldCamera == null
                            || c.worldCamera != DisplayManager.MainCamera));
                foreach (var canvas in canvases)
                {
                    canvas.worldCamera = DisplayManager.EventCamera;
                }
#endif

#if UNITY_MODULES_AUDIO
                var audioSources = scene.FindAll<AudioSource>((a) => a.spatialize);
                foreach (var audioSource in audioSources)
                {
                    interaction.Spatialize(audioSource);
                }
#endif
            }

            var start = DateTime.Now;
            var ts = TimeSpan.FromSeconds(1);
            while ((DateTime.Now - start) < ts || loadingBar?.Progress >= 1)
            {
                yield return null;
            }

            splash?.Deactivate();
            loadingBar?.Deactivate();
            if (fader != null)
            {
                yield return fader.ExitCoroutine();
                fader.volume = originalFadeVolume;
                fader.fadeInSound = originalFadeInSound;
            }
        }

        /// <summary>
        /// Disable any subScenes that are alive in the main scene.
        /// </summary>
        public virtual void Awake()
        {
            // Unity hacks the sense of null, creating a value reference that compares to null, but
            // doesn't work with the null coalescing operator. So we make it actually, really null here.
            if (splash == null)
            {
                splash = null;
            }

            splash?.Activate();

            fader = ComponentExt.FindAny<FadeTransition>();
            interaction = ComponentExt.FindAny<InteractionAudio>();

            if (fader != null)
            {
                originalFadeInSound = fader.fadeInSound;
                originalFadeVolume = fader.volume;
                if (interaction != null)
                {
                    fader.volume = 0.5f;
                    fader.fadeInSound = interaction.soundOnStartUp;
                }
            }

            foreach (var subScene in FindObjectsOfType<SubSceneController>())
            {
                subScene.Deactivate();
            }
        }

        /// <summary>
        /// Wait until the Start method to load the scenes so the MasterSceneController or any child
        /// class of it can be found by the newly loaded scenes more reliably.
        /// </summary>
        protected virtual void Start()
        {
            if (fader != null)
            {
                fader.SkipEnter();
            }

            Invoke(nameof(LoadFirstScene), 0.5f);
        }

        /// <summary>
        /// Close the application.
        /// </summary>
        private static void Exit()
        {
            if (Application.isPlaying)
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        /// <summary>
        /// Get a scene by name (or path, if we're running in the editor).
        /// </summary>
        /// <returns>The scene.</returns>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="path">     Path.</param>
        private static Scene? GetScene(string sceneName, string path)

        {
            if (IsScenePathLoaded(path))
            {
                return SceneManager.GetSceneByPath(path);
            }
            else
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
#endif
                    SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                    return SceneManager.GetSceneByPath(path);
#if UNITY_EDITOR
                }
                else
                {
                    return EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                }
#endif
            }
        }

        private IEnumerator UnloadAllScenesExcept(string path)
        {
            foreach (SubSceneController subScene in CurrentSubScenes)
            {
                subScene.Deactivate();
                if (subScene.gameObject.scene.path != path
                    && subScene.unloadSceneOnExit)
                {
                    yield return SceneManager.UnloadSceneAsync(subScene.gameObject.scene).AsCoroutine();
                }
            }
        }

        private IEnumerator LoadScenePathCoroutine(string path, IProgress prog)
        {
            var sceneName = GetSceneNameFromPath(path);
            var sceneLoadProg = prog.Subdivide(0, 0.25f, sceneName + ": loading...");
            var subSceneLoadProg = prog.Subdivide(0.25f, 0.75f, sceneName + ": loading components...");

            if (IsScenePathLoaded(path))
            {
                sceneLoadProg?.Report(1, "100%");
                yield return sceneName + " already loaded.";
            }
            else
            {
                yield return LoadScene(path, sceneName);
            }

            Scene? scene = null;
            while (scene == null)
            {
                scene = GetScene(sceneName, path);
                yield return sceneName + " " + (prog?.Progress).Label(UnitOfMeasure.Percent, 1);
            }

            if (Application.isPlaying)
            {
                var toLoad = (from root in scene.Value.GetRootGameObjects()
                              from subScene in root.GetComponentsInChildren<SubSceneController>(true)
                              select subScene)
                        .ToArray();
                for (var i = 0; i < toLoad.Length; ++i)
                {
                    var ss = toLoad[i];
                    yield return ss.EnterCoroutine(subSceneLoadProg.Subdivide(i, toLoad.Length, ss.name));
                }
            }
        }

        /// <summary>
        /// Quit out of the application, making sure any exit transitions for the current scene are
        /// ran first.
        /// </summary>
        public void Quit()
        {
            StartCoroutine(QuitCoroutine());
        }

        /// <summary>
        /// Quit out of the application, making sure any exit transitions for the current scene are
        /// ran first.
        /// </summary>
        private IEnumerator QuitCoroutine()
        {
            foreach (SubSceneController subScene in CurrentSubScenes)
            {
                subScene.Exit();
            }

            bool anyIncomplete;
            do
            {
                anyIncomplete = false;
                foreach (SubSceneController subScene in CurrentSubScenes)
                {
                    anyIncomplete |= !subScene.IsComplete;
                }
                if (anyIncomplete)
                {
                    yield return null;
                }
            } while (anyIncomplete);

            if (fader != null)
            {
                fader.fadeOutSound = interaction.soundOnShutDown;
                fader.volume = 0.5f;
                yield return fader.EnterCoroutine();
            }

            Exit();
        }
    }
}