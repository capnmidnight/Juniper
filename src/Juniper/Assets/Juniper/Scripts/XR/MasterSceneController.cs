using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.SceneManagement;

using Juniper.Unity.Animation;
using Juniper.Unity.Audio;
using Juniper.Unity.Display;
using Juniper.Unity.Widgets;
using Juniper.Progress;

using UnityEngine.UI;

using Juniper.Unity.World.LightEstimation;
using Juniper.Unity.World;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace Juniper.Unity
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

        public Image splash;

        public Transform optionsInterface;

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
            loadingBar?.Report(1);
            yield return loadingBar?.Waiter;
            splash?.Deactivate();
            loadingBar?.Deactivate();
            optionsInterface?.Activate();
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
            JuniperPlatform.Install(false);

            // Unity hacks the sense of null, creating a value reference that compares to null, but
            // doesn't work with the null coallescing operator. So we make it actually, really null here.
            if (splash == null)
            {
                splash = null;
            }

            if (optionsInterface == null)
            {
                optionsInterface = null;
            }

            optionsInterface?.Deactivate();
            splash?.Activate();

            darth = ComponentExt.FindAny<FadeTransition>();

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

        public bool Install(bool reset)
        {
            var qualityDegrader = ComponentExt.FindAny<QualityDegrader>();
            var aud = ComponentExt.FindAny<InteractionAudio>();
            if (qualityDegrader == null || aud == null)
            {
                return false;
            }

            SetupFader(reset);
            SetupSystemInterface(reset, qualityDegrader, aud);
            SetupLighting();
            SetupGround();

            return true;
        }

        private void SetupFader(bool reset)
        {
#if UNITY_EDITOR
            if (sceneFaderMaterial == null || reset)
            {
                sceneFaderMaterial = ComponentExt.EditorLoadAsset<Material>("Assets/Juniper/Materials/FaderMaterial.mat");
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

        private void SetupSystemInterface(bool reset, QualityDegrader qualityDegrader, InteractionAudio aud)
        {
            var sys = transform.Query("/SystemUserInterface");
            if (sys == null)
            {
                sys = new GameObject("SystemUserInterface").transform;
                sys.localPosition = 1.5f * Vector3.forward;
                var follow = sys.Ensure<FollowMainCamera>();
                follow.Value.followDistance = 1.5f;
            }

            var transparentLayer = LayerMask.NameToLayer("TransparentFX");

            var bar = LoadingBar.Ensure(sys, transparentLayer);
            if (bar.IsNew || reset)
            {
                bar.transform.localPosition = (2f / 3f) * Vector3.down;
                bar.transform.localScale = new Vector3(1f, 0.1f, 0.1f);

                loadingBar = bar;
            }

#if UNITY_MODULES_UI
            var canv = sys.Ensure<RectTransform>("Canvas")
                .Ensure<Canvas>();

            if (canv.IsNew || reset)
            {
                canv.Value.renderMode = RenderMode.WorldSpace;

                canv.SetAnchors(0.5f * Vector2.one, 0.5f * Vector2.one)
                    .SetPivot(0.5f * Vector2.one)
                    .SetSize(1000, 1000)
                    .SetScale(new Vector3(0.001f, 0.001f, 1));
            }

            canv.Ensure<GraphicRaycaster>();
            canv.gameObject.layer = transparentLayer;

            var debugText = canv.Value
                    .Ensure<RectTransform>("DebugText")
                    .Ensure<Text>();
            if (debugText.IsNew || reset)
            {
                debugText.Value.gameObject.layer = transparentLayer;
                debugText.Value.raycastTarget = false;
                debugText.Value.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                debugText.Value.supportRichText = false;
                debugText.Value.alignment = TextAnchor.UpperLeft;
                debugText.Value.horizontalOverflow = HorizontalWrapMode.Wrap;
                debugText.Value.verticalOverflow = VerticalWrapMode.Truncate;
                debugText.Value.color = Color.green;
                debugText.Value.fontSize = 50;
                debugText.SetAnchors(Vector2.zero, Vector2.one)
                    .SetPivot(Vector2.up)
                    .SetPosition(Vector3.zero);
            }

            debugText.Ensure<ScreenDebugger>();

            if (splash == null || reset)
            {
                var splashImg = canv.Ensure<RectTransform>("SplashImage")
                    .Ensure<Image>();

                if (splashImg.IsNew || reset)
                {
#if UNITY_EDITOR
                    splashImg.Value.sprite = ComponentExt.EditorLoadAsset<Sprite>("Assets/Juniper/Textures/logo-large.png");
#endif
                    splashImg
                        .SetAnchors(Vector2.zero, Vector2.one)
                        .SetPivot(0.5f * Vector2.one)
                        .SetPosition(Vector3.zero)
                        .gameObject.layer = transparentLayer;
                }

                splash = splashImg;
            }

            if (optionsInterface == null || reset)
            {
                var opts = canv.Ensure<RectTransform>("Options");
                if (opts.IsNew || reset)
                {
                    opts.SetAnchors(Vector2.zero, Vector2.one)
                        .SetPivot(0.5f * Vector2.one)
                        .SetPosition(Vector3.zero);
                }

                optionsInterface = opts.transform;
            }

            var icon = optionsInterface
                .Ensure<RectTransform>("Image")
                .Ensure<Image>();
            if (icon.IsNew)
            {
#if UNITY_EDITOR
                icon.Value.sprite = ComponentExt.EditorLoadAsset<Sprite>("Assets/Juniper/Textures/gear.png");
#endif

                icon.SetAnchors(Vector2.zero, Vector2.zero)
                    .SetPivot(Vector2.zero)
                    .SetSize(100, 100);
            }

#if UNITY_TEXTMESHPRO
            var optionsPanel = optionsInterface.Ensure<RectTransform>("OptionsPanel");
            if (optionsPanel.IsNew)
            {
                optionsPanel
                    .SetAnchors(Vector2.zero, Vector2.one)
                    .SetPivot(0.5f * Vector2.one)
                    .SetSize(Vector2.zero);
            }

            var toggle = icon.Ensure<Toggle>();
            toggle.Value.isOn = true;
            toggle.Value.onValueChanged.AddListener(optionsPanel.SetActive);

            var qualitySlider = MakeLabeledSlider(optionsPanel, "Quality", 225 * Vector2.down);
            qualitySlider.Value.wholeNumbers = true;
            qualitySlider.Value.minValue = 0;
            qualitySlider.Value.maxValue = 5;

            qualityDegrader.qualitySlider = qualitySlider;

            var volumeSlider = MakeLabeledSlider(optionsPanel, "Volume", 350 * Vector2.down);
            aud.volumeSlider = volumeSlider.gameObject;

#endif
#endif
        }

        private void SetupLighting()
        {
            var sun = RenderSettings.sun;
#if UNITY_EDITOR
            if (sun?.gameObject?.scene.name != gameObject.scene.name)
            {
                sun = (from light in ComponentExt.FindAll<Light>()
                       where light.type == LightType.Directional
                        && light.gameObject?.scene.name == gameObject.scene.name
                       orderby light.intensity descending
                       select light).FirstOrDefault();

                if (sun == null)
                {
                    sun = new GameObject("Sun").AddComponent<Light>();
                    sun.type = LightType.Directional;
                    sun.shadows = LightShadows.Soft;
                }

                RenderSettings.sun = sun;
            }
#endif

            sun.Ensure<GPSLocation>();
            sun.Ensure<SunPosition>();
            sun.Ensure<LightMeasurement>();

            var estimator = sun.GetComponent<AbstractLightEstimate>();
            if (estimator == null)
            {
                estimator = sun.gameObject.AddComponent<IndoorLightEstimate>();
            }

            var sunRig = sun.transform.parent;
            if (sunRig == null)
            {
                sunRig = new GameObject().transform;
                sun.transform.SetParent(sunRig, false);
            }

            sunRig.name = "SunRig";
            sunRig.Ensure<CompassRose>();
        }

        private void SetupGround()
        {
            var ground = transform.Query("/Ground");
            if (ground == null)
            {
                ground = new GameObject("Ground").transform;
            }
            ground.Ensure<Ground.Ground>();
        }

        private static RectTransform MakeLabeledPanel(RectTransform optionsPanel, string name, Vector2 position)
        {
            var panelRect = optionsPanel.Ensure<RectTransform>(name).Value;
            var panel = panelRect.Ensure<HorizontalLayoutGroup>();
            if (panel.IsNew)
            {
                panel.Value.childAlignment = TextAnchor.MiddleLeft;
                panel.Value.childControlWidth = false;
                panel.Value.childControlHeight = true;
                panel.Value.childForceExpandWidth = true;
                panel.Value.childForceExpandHeight = true;
                panel.SetAnchors(0.5f * Vector2.up, new Vector2(1, 0.5f))
                    .SetPivot(0.5f * Vector2.one)
                    .SetPosition(position)
                    .SetSize(new Vector2(0, 100));
            }

            var labelRect = panel.Ensure<RectTransform>(name + "Label").Value;
            var label = labelRect.Ensure<TMPro.TextMeshProUGUI>();
            if (label.IsNew)
            {
                label.SetPivot(0.5f * Vector2.one)
                    .SetSize(260, panelRect.rect.height);
                label.Value.text = name;
                label.Value.alignment = TMPro.TextAlignmentOptions.MidlineRight;
            }

            var content = panel.Ensure<RectTransform>(name + "Content");
            if (content.IsNew)
            {
                content.SetPivot(new Vector2(1, 0.5f))
                    .SetSize(panelRect.rect.width - labelRect.rect.width, panelRect.rect.height);
            }

            return content;
        }

        private static PooledComponent<Slider> MakeLabeledSlider(RectTransform optionsPanel, string name, Vector2 position)
        {
            var panel = MakeLabeledPanel(optionsPanel, name, position);

            var slider = panel
                .Ensure<RectTransform>(name + "Slider")
                .Ensure<Slider>();
            if (slider.IsNew)
            {
                var size = new Vector2(160, 20);
                slider
                    .SetAnchors(Vector2.up, Vector2.up)
                    .SetPivot(Vector2.zero)
                    .SetScale(4f * Vector3.one)
                    .SetSize(size)
                    .SetPosition(new Vector3(0.5f * (panel.rect.width - size.x * 4), -0.5f * (panel.rect.height + size.y * 4)));
                slider.Value.direction = Slider.Direction.LeftToRight;
            }

            var background = slider
                .Ensure<RectTransform>("Background")
                .Ensure<Image>();
            if (background.IsNew)
            {
                background.SetAnchors(0.25f * Vector2.up, new Vector2(1, 0.75f))
                    .SetPosition(Vector2.zero)
                    .SetSize(Vector2.zero);
#if UNITY_EDITOR
                background.Value.sprite = ComponentExt.EditorLoadAsset<Sprite>("UI/Skin/Background.psd");
#endif
                background.Value.type = Image.Type.Sliced;
            }

            var fillArea = slider
                .Ensure<RectTransform>("Fill Area");
            if (fillArea.IsNew)
            {
                fillArea
                    .SetAnchors(0.25f * Vector2.up, new Vector2(1, 0.75f))
                    .SetPosition(5 * Vector2.left)
                    .SetSize(20 * Vector2.left);
            }

            var fill = fillArea
                .Ensure<RectTransform>("Fill")
                .Ensure<Image>();
            if (fill.IsNew)
            {
                fill
                    .SetAnchors(Vector2.zero, Vector2.up)
                    .SetSize(new Vector2(10, 0));
#if UNITY_EDITOR
                fill.Value.sprite = ComponentExt.EditorLoadAsset<Sprite>("UI/Skin/UISprite.psd");
#endif
                fill.Value.type = Image.Type.Sliced;
            }

            var handleArea = slider
                .Ensure<RectTransform>("Handle Slide Area");
            if (handleArea.IsNew)
            {
                handleArea
                    .SetAnchors(Vector2.zero, Vector2.one)
                    .SetSize(20 * Vector2.left);
            }

            var handle = handleArea
                .Ensure<RectTransform>("Handle")
                .Ensure<Image>();
            if (handle.IsNew)
            {
                handle.SetSize(20 * Vector2.right);
#if UNITY_EDITOR
                handle.Value.sprite = ComponentExt.EditorLoadAsset<Sprite>("UI/Skin/Knob.psd");
#endif
            }

            slider.Value.fillRect = fill.GetComponent<RectTransform>();
            slider.Value.handleRect = handle.GetComponent<RectTransform>();
            slider.Value.targetGraphic = handle;
            return slider;
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
            StartCoroutine(LoadAllScenesCoroutine2());
        }

        private IEnumerator LoadAllScenesCoroutine2()
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
            prog?.Report(0);

            if (subSceneNames?.Length > 0)
            {
                var split = prog.Split(subSceneNames.Length);
                for (var i = 0; i < subSceneNames.Length; ++i)
                {
                    yield return LoadScenePathCoroutine(subSceneNames[i], split[i]);
                }
            }

            prog?.Report(1);
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
                if (obj is IEnumerator)
                {
                    var subIter = (IEnumerator)obj;
                    while (subIter?.MoveNext() == true)
                    {
                        print(subIter.Current);
                    }
                }
                else
                {
                    print(obj);
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
        private Scene? GetScene(string sceneName, string path)
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
            var split = prog.Split(2);
            var sceneLoadProg = split[0];
            var subSceneLoadProg = split[1];

            if (IsScenePathLoaded(path))
            {
                sceneLoadProg?.Report(1);
                yield return sceneName + " already loaded.";
            }
            else
            {
                var op = LoadScene(path, sceneName);
                if (op == null)
                {
                    sceneLoadProg?.Report(1);
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

            subSceneLoadProg.ForEach(toLoad, (ss, p) => ss.Load(p), Debug.LogException);

            while (prog?.IsComplete() == false)
            {
                yield return sceneName + " " + (prog?.Progress).Label(UnitOfMeasure.Percent, 1);
            }
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