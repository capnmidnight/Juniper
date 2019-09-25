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
using Juniper.Input;

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

        private string GetScenePathFromName(string subSceneName)
        {
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

            return scenePath;
        }

        /// <summary>
        /// Check to see if a particular scene is already loaded.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsSceneLoaded(string path)
        {
            var scene = SceneManager.GetSceneByPath(path);
            var name = GetSceneNameFromPath(path);
            return scene.isLoaded && scene.name == name;
        }

        /// <summary>
        /// Get a scene by name (or path, if we're running in the editor).
        /// </summary>
        /// <returns>The scene.</returns>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="path">     Path.</param>
        private static IEnumerator LoadScene(string sceneName, string scenePath, IProgress sceneLoadProg)
        {
            sceneLoadProg.Report(0);
            if (!IsSceneLoaded(scenePath))
            {
                if (Application.isPlaying)
                {
                    yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).AsCoroutine(sceneLoadProg);
                }
#if UNITY_EDITOR
                else
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }
#endif
                while (!IsSceneLoaded(scenePath))
                {
                    yield return null;
                }
            }
            sceneLoadProg.Report(1);
        }

        /// <summary>
        /// Quit out of the application, making sure any exit transitions for the current scene are
        /// ran first.
        /// </summary>
        public static void QuitApp()
        {
            if (Find.Any<MasterSceneController>(out var master))
            {
                master.Quit();
            }
            else
            {
                Exit();
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
        private UnifiedInputModule input;

#if UNITY_MODULES_AUDIO
        private float originalFadeVolume;
        private AudioClip originalFadeInSound;
#endif

        /// <summary>
        /// All of the active SubSceneControllers
        /// </summary>
        /// <value>The current sub scenes.</value>
        public IEnumerable<SubSceneController> CurrentSubScenes
        {
            get
            {
                foreach (var subScene in Find.All<SubSceneController>())
                {
                    if (subScene.isActiveAndEnabled)
                    {
                        yield return subScene;
                    }
                }
            }
        }

        private IEnumerable<SubSceneController> GetOpenSubScenes(string path)
        {
            foreach (var subScene in CurrentSubScenes)
            {
                var scene = subScene.gameObject.scene;
                if (scene.isLoaded
                    && scene.path != path
                    && subScene.CanExit)
                {
                    yield return subScene;
                }
            }
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            JuniperSystem.OnEditorUpdateIn(
                $"{typeof(MasterSceneController).FullName}::{nameof(SetBuildSettings)}",
                TimeSpan.FromSeconds(1),
                SetBuildSettings);
        }


        private void SetBuildSettings()
        {
            subSceneNames = subSceneNames.Where(File.Exists).ToArray();
            if (!Application.isPlaying
                && gameObject != null
                && !string.IsNullOrEmpty(gameObject.scene.path))
            {
                var s = new List<EditorBuildSettingsScene>();
                s.Add(new EditorBuildSettingsScene(gameObject.scene.path, true));
                s.AddRange(from path in subSceneNames
                           select new EditorBuildSettingsScene(path, true));
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
        /// Use this function by right-clicking in the editor to open up all the scenes in additive mode.
        /// </summary>
        private IEnumerator LoadAllScenesCoroutine(IProgress prog)
        {
            if (subSceneNames != null && subSceneNames.Length > 0)
            {
                for (var i = 0; i < subSceneNames.Length; ++i)
                {
                    yield return LoadScenePathCoroutine(
                        subSceneNames[i],
                        prog.Subdivide(i, subSceneNames.Length));
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
                    if (File.Exists(path))
                    {
                        var sceneName = SceneNamePattern.Match(path).Groups[1].Value;
                        var iter = LoadScene(sceneName, path, null);
                        while (iter.MoveNext())
                        {
                            print(path);
                        }

                        var scene = SceneManager.GetSceneByPath(path);
                        SceneManager.MergeScenes(scene, curScene);
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid Scene Path: {path}");
                    }
                }

                subSceneNames = new string[0];
                EditorSceneManager.SaveScene(curScene);
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(curScene.path, true)
            };
        }

        public void Reset()
        {
            Reinstall();
        }

#endif

        public void Install(bool reset)
        {
            if (reset && (subSceneNames == null || subSceneNames.Length == 0))
            {
                subSceneNames = new string[] { "Assets/Juniper/Scenes/Examples/Content.unity" };
            }
            SetupFader(reset);
            SetupSystemInterface();
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

        private void SetupSystemInterface()
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
                    yield return SwitchToSceneCoroutine(name, true, false, true, false);
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
        public void SwitchToScene(string sceneName, bool fromView)
        {
            StartCoroutine(SwitchToSceneCoroutine(sceneName, false, false, true, fromView));
        }

        public void ShowScene(string sceneName)
        {
            StartCoroutine(SwitchToSceneCoroutine(sceneName, false, false, false, false));
        }

        public void ShowView(string viewName)
        {
            StartCoroutine(SwitchToSceneCoroutine(viewName, true, true, false, false));
        }

        private IEnumerator SwitchToSceneCoroutine(string subSceneName, bool skipFadeOut, bool skipLoadingScreen, bool unloadOtherScenes, bool fromView)
        {
            var scenePath = GetScenePathFromName(subSceneName);

            if (scenePath == null)
            {
                ScreenDebugger.Print($"Couldn't find scene: {subSceneName}");
            }
            else
            {
                if (unloadOtherScenes)
                {
                    yield return ExitAllSubScenesExcept(GetOpenSubScenes(scenePath), scenePath);
                }

                var showFader = !skipFadeOut
                    && fader != null
                    && fader.CanEnter;

                if (showFader && fromView)
                {
                    showFader &= !IsSceneLoaded(scenePath);
                    if (!showFader)
                    {
                        var scene = SceneManager.GetSceneByPath(scenePath);
                        var subScenes = scene.FindAll<SubSceneController>();
                        foreach (var subScene in subScenes)
                        {
                            showFader |= subScene.IsExited || !subScene.isActiveAndEnabled;
                        }
                    }
                }

                if (showFader)
                {
                    yield return JuniperSystem.Cleanup();

                    yield return fader.EnterCoroutine();

                    if (input != null)
                    {
                        input.enabled = false;
                    }

                    if (loadingBar != null && !skipLoadingScreen)
                    {
                        loadingBar.Activate();
                        loadingBar.Report(0, subSceneName);
                    }
                }

                if (unloadOtherScenes)
                {
                    foreach (var subScene in GetOpenSubScenes(scenePath))
                    {
                        if (subScene.unloadSceneOnExit)
                        {
                            var scene = subScene.gameObject.scene;
                            yield return SceneManager.UnloadSceneAsync(scene).AsCoroutine();
                        }
                    }
                }

                yield return LoadScenePathCoroutine(scenePath, loadingBar);

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

                if (loadingBar != null)
                {
                    var start = DateTime.Now;
                    var ts = TimeSpan.FromSeconds(1);
                    while ((DateTime.Now - start) < ts || loadingBar.Progress >= 1)
                    {
                        yield return null;
                    }
                    loadingBar.Deactivate();
                }

                if (splash != null)
                {
                    splash.Deactivate();
                }

                if (fader != null)
                {
                    if (fader.CanExit)
                    {
                        if (input != null)
                        {
                            input.enabled = true;
                        }

                        if (skipLoadingScreen)
                        {
                            fader.SkipExit();
                        }
                        else
                        {
                            yield return fader.ExitCoroutine();
                        }
                    }
#if UNITY_MODULES_AUDIO
                    fader.volume = originalFadeVolume;
                    fader.fadeInSound = originalFadeInSound;
#endif
                }
            }
        }

        private static IEnumerator LoadScenePathCoroutine(string path, IProgress prog)
        {
            var sceneName = GetSceneNameFromPath(path);
            var sceneLoadProg = prog.Subdivide(0, 0.25f, sceneName + ": loading...");
            var subSceneLoadProg = prog.Subdivide(0.25f, 0.75f, sceneName + ": loading components...");

            yield return LoadScene(sceneName, path, sceneLoadProg);

            var scene = SceneManager.GetSceneByPath(path);

            if (Application.isPlaying)
            {
                var toLoad = scene.FindAll<SubSceneController>().ToArray();
                for (var i = 0; i < toLoad.Length; ++i)
                {
                    var ss = toLoad[i];
                    if (ss.CanEnter)
                    {
                        yield return ss.EnterCoroutine(subSceneLoadProg.Subdivide(i, toLoad.Length, ss.name));
                    }
                }
            }
        }

        public void RemoveScene(string sceneName)
        {
            StartCoroutine(RemoveSceneCoroutine(sceneName));
        }

        private IEnumerator RemoveSceneCoroutine(string sceneName)
        {
            var scenePath = GetScenePathFromName(sceneName);
            if (IsSceneLoaded(scenePath))
            {
                var scene = SceneManager.GetSceneByPath(scenePath);
                var subScenes = scene.FindAll<SubSceneController>();
                foreach (var subScene in subScenes)
                {
                    if (subScene.CanExit)
                    {
                        yield return subScene.ExitCoroutine();
                    }
                }
                yield return SceneManager.UnloadSceneAsync(scene.buildIndex).AsCoroutine();
            }
        }

        /// <summary>
        /// Disable any subScenes that are alive in the main scene.
        /// </summary>
        public virtual void Awake()
        {
            if (splash != null)
            {
                splash.Activate();
            }

            if (Find.Any(out input))
            {
                input.enabled = false;
            }

            var faderFound = Find.Any(out fader);
            var interactionFound = Find.Any(out interaction);

#if UNITY_MODULES_AUDIO
            if (faderFound)
            {
                originalFadeInSound = fader.fadeInSound;
                originalFadeVolume = fader.volume;
                if (interactionFound)
                {
                    fader.volume = 0.5f;
                    fader.fadeInSound = interaction.soundOnStartUp;
                }
            }
#endif

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
            yield return ExitAllSubScenesExcept(CurrentSubScenes, null);

            if (fader != null)
            {
#if UNITY_MODULES_AUDIO
                fader.fadeOutSound = interaction.soundOnShutDown;
                fader.volume = 0.5f;
#endif
                yield return fader.EnterCoroutine();
            }

            Exit();
        }

        private IEnumerator ExitAllSubScenesExcept(IEnumerable<SubSceneController> subScenes, string excludePath)
        {
            subScenes = subScenes.ToArray();
            foreach (var subScene in subScenes)
            {
                if (subScene.gameObject.scene.path != excludePath
                    && subScene.CanExit)
                {
                    subScene.Exit();
                }
            }

            bool anyIncomplete;
            do
            {
                anyIncomplete = false;
                foreach (var subScene in subScenes)
                {
                    if (subScene.gameObject.scene.path != excludePath)
                    {
                        anyIncomplete |= !subScene.IsComplete;

                        if (subScene.IsComplete)
                        {
                            subScene.Deactivate();
                        }
                    }
                }
                if (anyIncomplete)
                {
                    yield return null;
                }
            } while (anyIncomplete);
        }
    }
}