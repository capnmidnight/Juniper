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

using UnityEngine.UI;

using Juniper.Speech;

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

        /// <summary>
        /// Get a scene by name (or path, if we're running in the editor).
        /// </summary>
        /// <returns>The scene.</returns>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="path">     Path.</param>
        private static AsyncOperation LoadScene(string scenePath, string sceneName)
        {
            if (Application.isPlaying)
            {
                return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
#if UNITY_EDITOR
            else
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
#endif
            return null;
        }

        private static string GetSceneNameFromPath(string path)
        {
            return SceneNamePattern.Match(path).Groups[1].Value;
        }

        public LoadingBar loadingBar;

        public UnityImage splash;

        /// <summary>
        /// The material to apply to Darth Fader.
        /// </summary>
        [Header("Scene fader")]
        public Material sceneFaderMaterial;

        public bool loadAll = true;

        public bool exitOnEscape = true;

        /// <summary>
        /// All of the scene files to load. Use the "truncated scene path" format for these paths.
        /// Truncated scene paths do not need to reference the "Assets/" (it is assumed) and do not
        /// need the ".unity" file extension.
        /// </summary>
        public string[] subSceneNames;

        /// <summary>
        /// Parse the scene name.
        /// </summary>
        private const string SceneNamePatternStr = "([^/]+)\\.unity$";

        /// <summary>
        /// Parse the scene name.
        /// </summary>
        private static readonly Regex SceneNamePattern = new Regex(SceneNamePatternStr, RegexOptions.Compiled);

        private FadeTransition darth;

        private InteractionAudio interaction;

        /// <summary>
        /// A flag that indicates the cursor was locked on the previous frame.
        /// </summary>
        private bool wasLocked;

        /// <summary>
        /// All of the <see cref="SubSceneController"/> references in the project.
        /// </summary>
        /// <value>The sub scenes.</value>
        public IEnumerable<SubSceneController> SubScenes
        {
            get
            {
                return ComponentExt.FindAll<SubSceneController>();
            }
        }

        /// <summary>
        /// All of the active SubSceneControllers
        /// </summary>
        /// <value>The current sub scenes.</value>
        public SubSceneController[] CurrentSubScenes
        {
            get
            {
                return (from scene in SubScenes
                        where scene.isActiveAndEnabled
                        select scene)
                    .ToArray();
            }
        }

        public void Update()
        {
            if (exitOnEscape && !wasLocked && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApp();
            }

            wasLocked = Cursor.lockState != CursorLockMode.None;
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
        public void SwitchToSceneName(string sceneName)
        {
            SwitchToSceneName(sceneName, false);
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
        /// <returns></returns>
        private void SwitchToSceneName(string sceneName, bool skipFadeOut)
        {
            StartCoroutine(SwitchToSceneNameCoroutine(sceneName, skipFadeOut));
        }

        private void FadeOut(bool skipFadeOut)
        {
            if (skipFadeOut)
            {
                darth?.SkipEnter();
            }
            else
            {
                darth?.Enter();
            }
        }

        private IEnumerator SwitchToSceneNameCoroutine(string subSceneName, bool skipFadeOut)
        {
            loadingBar?.Activate();
            loadingBar?.Report(0);

            if (!skipFadeOut)
            {
                FadeOut(skipFadeOut);
                yield return darth?.Waiter;
            }

            var parts = subSceneName.Split('.');
            var sceneName = parts[0];
            var sceneFileName = sceneName + ".unity";
            var scenePath = Array.Find(subSceneNames, s => s.EndsWith(sceneFileName));

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
                            || c.worldCamera == DisplayManager.MainCamera));
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

            ComponentExt.FindAny<KeywordRecognizer>()?.RefreshKeywords();

            yield return loadingBar?.Waiter;
            splash?.Deactivate();
            loadingBar?.Deactivate();
            darth?.Exit();
            yield return darth?.Waiter;
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

            darth = ComponentExt.FindAny<FadeTransition>();
            interaction = ComponentExt.FindAny<InteractionAudio>();

            foreach (var subScene in FindObjectsOfType<SubSceneController>())
            {
                subScene.Deactivate();
            }
        }

        public virtual void Reinstall()
        {
            Install(true);
        }

#if UNITY_EDITOR

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
                print(prefab);
                sys = Instantiate(prefab).transform;
                sys.name = "SystemUserInterface";
            }
#endif

            loadingBar = sys.Query<LoadingBar>("LoadingBar");

            splash = sys.Query<UnityImage>("Canvas/SplashImage");
        }

        public void Uninstall()
        {
        }

        /// <summary>
        /// Wait until the Start method to load the scenes so the MasterSceneController or any child
        /// class of it can be found by the newly loaded scenes more reliably.
        /// </summary>
        protected virtual void Start()
        {
            FadeOut(true);

            if (loadAll)
            {
                Invoke(nameof(LoadAllScenes), 0.5f);
            }
            else
            {
                Invoke(nameof(LoadFirstScene), 0.5f);
            }
        }

        public void LoadAllScenes()
        {
            StartCoroutine(LoadAllScenesCoroutineWithProgress());
        }

        private IEnumerator LoadAllScenesCoroutineWithProgress()
        {
            loadingBar?.Activate();
            yield return LoadAllScenesCoroutine(loadingBar);
            yield return LoadingCompleteCoroutine();
        }

        /// <summary>
        /// Use this function by right-clicking in the editor to open up all the scenes in additive mode.
        /// </summary>
        private IEnumerator LoadAllScenesCoroutine(IProgress prog = null)
        {
            if (subSceneNames?.Length > 0)
            {
                yield return new InterleavedEnumerator(
                    prog.Select(subSceneNames,
                    (subSceneName, p) =>
                        LoadScenePathCoroutine(subSceneName, p)));
            }
        }

        private void LoadFirstScene()
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
                    SwitchToSceneName(name, true);
                }
            }
        }

        private static bool IsScenePathLoaded(string path)
        {
            var scene = SceneManager.GetSceneByPath(path);
            return scene.IsValid() && scene.isLoaded;
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

#if UNITY_EDITOR

        /// <summary>
        /// Use this function by right-clicking in the editor to open up all the scenes in additive mode.
        /// </summary>
        [ContextMenu("Load Scenes")]
        private void LoadScenes_MenuItem()
        {
            var iter = LoadAllScenesCoroutine(
                new Progress.UnityEditorProgressDialog("Loading scenes"));
            while (iter.MoveNext())
            {
                var obj = iter.Current;
                if (obj is IEnumerator subIter)
                {
                    while (subIter?.MoveNext() == true)
                    {
                        Debug.Log(subIter.Current);
                    }
                }
                else
                {
                    Debug.Log(obj);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        public void OnValidate()
        {
            Invoke(nameof(SetBuildSettings), 100);
        }

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

#endif

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

        private IEnumerator LoadScenePathCoroutine(string path, IProgress prog)
        {
            prog?.Report(0);

            var sceneName = GetSceneNameFromPath(path);
            var sceneLoadProg = prog.Subdivide(0, 0.25f);
            var subSceneLoadProg = prog.Subdivide(0.25f, 0.75f);

            if (IsScenePathLoaded(path))
            {
                yield return sceneName + " already loaded.";
            }
            else
            {
                var op = LoadScene(path, sceneName);
                if (op == null)
                {
                    yield return sceneName + " 100%";
                }
                else
                {
                    while (!op.isDone)
                    {
                        sceneLoadProg?.Report(op.progress);
                        yield return sceneName + " " + (prog?.Progress).Label(UnitOfMeasure.Percent, 1);
                    }
                }
            }

            Scene? scene = null;
            while (scene == null)
            {
                scene = GetScene(sceneName, path);
                yield return sceneName + " " + (prog?.Progress).Label(UnitOfMeasure.Percent, 1);
            }

            var toLoad = from root in scene.Value.GetRootGameObjects()
                         from subScene in root.GetComponentsInChildren<SubSceneController>(true)
                         select subScene;

            var loading = new InterleavedEnumerator(subSceneLoadProg.Select(
                toLoad,
                (ss, p) =>
                {
                    ss.Enter(p);
                    return ss.Waiter;
                }));
            yield return loading;
        }

#if UNITY_EDITOR

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

#endif

        /// <summary>
        /// Quit out of the application, making sure any exit transitions for the current scene are
        /// ran first.
        /// </summary>
        private IEnumerator QuitCoroutine()
        {
            foreach (var subScene in CurrentSubScenes)
            {
                subScene.Exit();
            }

            yield return new WaitUntil(() =>
                CurrentSubScenes.All(subScene =>
                    subScene.IsComplete));

            if (darth != null)
            {
                darth.Enter();
                yield return darth.Waiter;
            }

            Exit();
        }
    }
}