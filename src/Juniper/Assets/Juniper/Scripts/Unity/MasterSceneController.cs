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
using Juniper.Unity.Progress;
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
        /// <param name="path">Path.</param>
        private static IProgress LoadScene(string scenePath, string sceneName)
        {
            if (Application.isPlaying)
            {
                var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                if (op != null)
                {
                    return new UnityAsyncOperationProgress(op);
                }
            }
#if UNITY_EDITOR
            else
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
#endif
            return StaticProgress.COMPLETE;
        }

        private static string GetSceneNameFromPath(string path)
        {
            return SceneNamePattern.Match(path).Groups[1].Value;
        }

        public LoadingBar loadingBar;

        public Image splash;

        public GameObject optionsInterface;

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
        /// Get all of the scenes defined in the Build settings.
        /// </summary>
        /// <value>All scenes.</value>
        public static IEnumerable<Scene> AllScenes
        {
            get
            {
                for (var i = 0; i < SceneManager.sceneCount; ++i)
                {
                    yield return SceneManager.GetSceneAt(i);
                }
            }
        }

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
        public void SwitchToSceneName(string sceneName, bool skipFadeOut = false)
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

        private IEnumerator SwitchToSceneNameCoroutine(string subSceneName, bool skipFadeOut = false)
        {
            loadingBar?.Activate();
            loadingBar?.SetProgress(0);

            if (!skipFadeOut)
            {
                FadeOut(skipFadeOut);
                yield return darth?.Waiter;
            }

            var split = loadingBar?.Split(2);
            var parts = subSceneName.Split('.');
            var sceneName = parts[0];
            var sceneFileName = sceneName + ".unity";
            var scenePath = Array.Find(subSceneNames, s => s.EndsWith(sceneFileName));

            yield return LoadScenePathCoroutine(scenePath, split[0]);
            yield return LoadingCompleteCoroutine();
        }

        private IEnumerator LoadingCompleteCoroutine()
        {
            loadingBar?.SetProgress(1);
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
            Install(false);

            // Unity hacks the sense of null, creating a value reference that compares to null,
            // but doesn't work with the null coallescing operator. So we make it actually,
            // really null here.
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
            reset &= Application.isEditor;

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

            var sys = transform.EnsureTransform("SystemUserInterface");
            if (sys.IsNew)
            {
                sys.transform.localPosition = 1.5f * Vector3.forward;
                sys.EnsureComponent<FollowMainCamera>()
                    .Value
                    .followDistance = 1.5f;
            }

            var transparentLayer = LayerMask.NameToLayer("TransparentFX");

            var bar = LoadingBar.Ensure(sys, transparentLayer);
            if (bar.IsNew)
            {
                bar.transform.localPosition = (2f / 3f) * Vector3.down;
                bar.transform.localScale = new Vector3(1f, 0.1f, 0.1f);

                loadingBar = bar;
            }

#if UNITY_MODULES_UI
            var canv = sys.EnsureTransform("Canvas")
                .EnsureComponent<Canvas>((c) =>
                {
                    c.renderMode = RenderMode.WorldSpace;

                    c.SetAnchors(0.5f * Vector2.one, 0.5f * Vector2.one)
                        .SetPivot(0.5f * Vector2.one)
                        .SetSize(1000, 1000)
                        .SetScale(new Vector3(0.001f, 0.001f, 1));

                    var debugText = c
                        .EnsureRectTransform("DebugText")
                        .EnsureComponent<Text>((d) =>
                        {
                            d.gameObject.layer = transparentLayer;
                            d.raycastTarget = false;
                            d.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                            d.supportRichText = false;
                            d.alignment = TextAnchor.UpperLeft;
                            d.horizontalOverflow = HorizontalWrapMode.Wrap;
                            d.verticalOverflow = VerticalWrapMode.Truncate;
                            d.color = Color.green;
                            d.fontSize = 50;
                            d.GetComponent<RectTransform>()
                                .SetAnchors(Vector2.zero, Vector2.one)
                                .SetPivot(Vector2.up)
                                .SetPosition(Vector3.zero);
                        });

                    debugText.EnsureComponent<ScreenDebugger>();
                });

            canv.EnsureComponent<GraphicRaycaster>();
            canv.gameObject.layer = transparentLayer;

            if (splash == null)
            {
                splash = canv.EnsureRectTransform("SplashImage")
                    .EnsureComponent<Image>((splashImg) =>
                    {
#if UNITY_EDITOR
                        splashImg.sprite = ComponentExt.EditorLoadAsset<Sprite>("Assets/Juniper/Textures/logo-large.png");
#endif
                        splashImg
                            .SetAnchors(Vector2.zero, Vector2.one)
                            .SetPivot(0.5f * Vector2.one)
                            .SetPosition(Vector3.zero)
                            .gameObject.layer = transparentLayer;
                    });
            }

            var opts = canv.EnsureRectTransform("Options");
            optionsInterface = opts.gameObject;
            if (opts.IsNew)
            {
                opts.SetAnchors(Vector2.zero, Vector2.one)
                    .SetPivot(0.5f * Vector2.one)
                    .SetPosition(Vector3.zero);
            }

            var icon = opts
                .EnsureRectTransform("Image")
                .EnsureComponent<Image>();
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
            var optionsPanel = opts
                .EnsureRectTransform("OptionsPanel");
            if (optionsPanel.IsNew)
            {
                optionsPanel
                    .SetAnchors(Vector2.zero, Vector2.one)
                    .SetPivot(0.5f * Vector2.one)
                    .SetSize(Vector2.zero);
            }

            var toggle = icon.EnsureComponent<Toggle>();
            toggle.Value.isOn = true;
            toggle.Value.onValueChanged.AddListener(optionsPanel.SetActive);

            var qualitySlider = MakeLabeledSlider(optionsPanel, "Quality", 225 * Vector2.down);
            qualitySlider.Value.wholeNumbers = true;
            qualitySlider.Value.minValue = 0;
            qualitySlider.Value.maxValue = 5;
            var qualityDegrader = ComponentExt.FindAny<QualityDegrader>();
            qualityDegrader.qualitySlider = qualitySlider;

            var volumeSlider = MakeLabeledSlider(optionsPanel, "Volume", 350 * Vector2.down);

            var aud = ComponentExt.FindAny<InteractionAudio>();
            if (aud != null)
            {
                aud.volumeSlider = volumeSlider.gameObject;
            }
#endif
#endif

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

            sun.EnsureComponent<GPSLocation>();
            sun.EnsureComponent<SunPosition>();
            sun.EnsureComponent<LightMeasurement>();

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
            sunRig.EnsureComponent<CompassRose>();

            return true;
        }

        private static PooledComponent<Slider> MakeLabeledSlider(PooledComponent<RectTransform> optionsPanel, string name, Vector2 position)
        {
            var panel = optionsPanel
                            .EnsureRectTransform(name);
            if (panel.IsNew)
            {
                panel
                    .SetAnchors(0.5f * Vector2.up, new Vector2(1, 0.5f))
                    .SetPivot(0.5f * Vector2.one)
                    .SetPosition(position)
                    .SetSize(new Vector2(0, 100));
            }

            var label = panel
                .EnsureRectTransform(name + "Label")
                .EnsureComponent<TMPro.TextMeshProUGUI>();
            if (label.IsNew)
            {
                label
                    .SetAnchors(0.5f * Vector2.right, new Vector2(0.5f, 1))
                    .SetPivot(0.5f * Vector2.one)
                    .SetPosition(370 * Vector2.left)
                    .SetSize(260, panel.Value.rect.height);
                label.Value.text = name;
                label.Value.alignment = TMPro.TextAlignmentOptions.MidlineRight;
            }

            var slider = panel
                .EnsureRectTransform(name + "Slider")
                .EnsureComponent<Slider>();
            if (slider.IsNew)
            {
                slider
                    .SetAnchors(0.5f * Vector2.one, 0.5f * Vector2.one)
                    .SetPivot(0.5f * Vector2.one)
                    .SetPosition(new Vector3(140, 0))
                    .SetSize(170, 20)
                    .SetScale(4f * Vector3.one);
                slider.Value.direction = Slider.Direction.LeftToRight;
            }

            var background = slider
                .EnsureRectTransform("Background")
                .EnsureComponent<Image>();
            if (background.IsNew)
            {
                background.SetAnchors(0.25f * Vector2.up, new Vector2(1, 0.75f))
                    .SetPosition(Vector2.zero)
                    .SetSize(Vector2.zero);
                background.Value.sprite = ComponentExt.EditorLoadAsset<Sprite>("UI/Skin/Background.psd");
                background.Value.type = Image.Type.Sliced;
            }

            var fillArea = slider
                .EnsureRectTransform("Fill Area");
            if (fillArea.IsNew)
            {
                fillArea
                    .SetAnchors(0.25f * Vector2.up, new Vector2(1, 0.75f))
                    .SetPosition(5 * Vector2.left)
                    .SetSize(20 * Vector2.left);
            }

            var fill = fillArea
                .EnsureRectTransform("Fill")
                .EnsureComponent<Image>();
            if (fill.IsNew)
            {
                fill
                    .SetAnchors(Vector2.zero, Vector2.up)
                    .SetSize(new Vector2(10, 0));
                fill.Value.sprite = ComponentExt.EditorLoadAsset<Sprite>("UI/Skin/UISprite.psd");
                fill.Value.type = Image.Type.Sliced;
            }

            var handleArea = slider
                .EnsureRectTransform("Handle Slide Area");
            if (handleArea.IsNew)
            {
                handleArea
                    .SetAnchors(Vector2.zero, Vector2.one)
                    .SetSize(20 * Vector2.left);
            }

            var handle = handleArea
                .EnsureRectTransform("Handle")
                .EnsureComponent<Image>();
            if (handle.IsNew)
            {
                handle.SetSize(20 * Vector2.right);
                handle.Value.sprite = ComponentExt.EditorLoadAsset<Sprite>("UI/Skin/Knob.psd");
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
private IEnumerator LoadAllScenesCoroutine(IProgressReceiver prog = null)
{
    prog?.SetProgress(0);

    if (subSceneNames?.Length > 0)
    {
        var split = prog.Split(subSceneNames.Length);
        for (var i = 0; i < subSceneNames.Length; ++i)
        {
            yield return LoadScenePathCoroutine(subSceneNames[i], split[i]);
        }
    }

    prog?.SetProgress(1);
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
    var iter = LoadAllScenesCoroutine(new UnityEditorProgressDialog("Loading scenes"));
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
/// <param name="path">Path.</param>
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

private IEnumerator LoadScenePathCoroutine(string path, IProgressReceiver prog)
{
    prog?.SetProgress(0);

    var sceneName = GetSceneNameFromPath(path);

    var split = prog.Split(2);
    var sceneLoadProg = split[0];
    var subSceneLoadProg = split[1];

    if (IsScenePathLoaded(path))
    {
        sceneLoadProg?.SetProgress(1);
        yield return sceneName + " already loaded.";
    }
    else
    {
        var op = LoadScene(path, sceneName);
        while (!op.IsComplete())
        {
            sceneLoadProg?.SetProgress(op.Progress);
            yield return sceneName + " " + (prog?.Progress)?.ToString("P1") ?? "N/A";
        }
    }

    Scene? scene = null;
    while (scene == null)
    {
        scene = GetScene(sceneName, path);
        yield return sceneName + " " + (prog?.Progress)?.ToString("P1") ?? "N/A";
    }

    var toLoad = from root in scene.Value.GetRootGameObjects()
                 from subScene in root.GetComponentsInChildren<SubSceneController>(true)
                 select subScene;

    subSceneLoadProg.ForEach(toLoad, (ss, p) => ss.Load(p), Debug.LogException);

    while (prog?.IsComplete() == false)
    {
        yield return sceneName + " " + (prog?.Progress)?.ToString("P1") ?? "N/A";
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
