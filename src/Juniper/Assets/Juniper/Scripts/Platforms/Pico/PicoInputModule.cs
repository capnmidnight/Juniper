#if PICO

using Juniper.Display;

using Pvr_UnitySDKAPI;

using System;

using UnityEngine;
using UnityEngine.UI;

namespace Juniper.Input
{
    public abstract class PicoInputModule : AbstractUnifiedInputModule
    {
        public override void Install(bool reset)
        {
            var stageT = transform.Find("Stage");

            var sdkMgr = MakeSDKManager(stageT);
            MakeControllerManager(stageT);
            MakeController(stageT);

            base.Install(reset);

            MakeViewerToast(stageT.Find("Head"));
            MakeSafeArea(stageT);
            MakeSafeToast(stageT);
            MakeResetPanel(stageT);
            MakeSafePanel1(stageT);
            MakeSafePanel2(stageT, sdkMgr);
        }

        public override void Uninstall()
        {
            var stage = transform.Find("Stage");
            stage.Remove<Pvr_Controller>();
            stage.Remove<Pvr_ControllerManager>();
            stage.Remove<Pvr_UnitySDKManager>();

            stage.Find("ResetPanel")?.Destroy();
            stage.Find("SafePanel2")?.Destroy();
            stage.Find("SafePanel1")?.Destroy();
            stage.Find("SafeToast")?.Destroy();
            stage.Find("SafeArea2")?.Destroy();
            stage.Find("Head/Viewertoast")?.Destroy();

            base.Uninstall();
        }

        private static void MakeSafeArea(Transform parent)
        {
#if UNITY_EDITOR
            var safeArea = parent.Find("SafeArea2");
            if (safeArea == null)
            {
                safeArea = Instantiate(ResourceExt.EditorLoadAsset<GameObject>("Assets/PicoMobileSDK/Pvr_UnitySDK/Resources/Cylinder01.FBX"), parent).transform;
                safeArea.name = "SafeArea2";
                safeArea.Deactivate();
            }
#endif
        }

        private static RectTransform MakePanel(Transform parent, string containerName, Vector3 containerPosition, Quaternion containerRotation, Vector3 position, float width, float height, Color imageColor)
        {
            var container = parent.Ensure<Transform>(containerName);
            if (container.IsNew)
            {
                container.transform.position = containerPosition;
                container.transform.rotation = containerRotation;
            }

            var panel = container.Ensure<RectTransform>("Panel");
            var panelCanvas = panel.Ensure<Canvas>();
            if (panelCanvas.IsNew)
            {
                panelCanvas.Value.renderMode = RenderMode.WorldSpace;
            }

            if (panel.IsNew)
            {
                panel.SetAnchors(Vector2.zero, Vector2.one)
                    .SetPivot(0.5f * Vector2.one)
                    .SetScale(0.0043f * Vector3.one)
                    .SetSize(width, height)
                    .SetPosition(position);
            }

            var panelImage = panel
                .Ensure<CanvasRenderer>()
                .Ensure<Image>();
            if (panelImage.IsNew)
            {
#if UNITY_EDITOR
                panelImage.Value.sprite = ResourceExt.EditorLoadAsset<Sprite>("UI/Skin/Background.psd");
#endif
                panelImage.Value.color = imageColor;
                panelImage.Value.raycastTarget = true;
                panelImage.Value.type = Image.Type.Sliced;
                panelImage.Value.fillCenter = true;
            }

            container.Deactivate();

            return panel;
        }

        private static RectTransform MakeText(Transform parent, string name, Vector3 position, float width, float height, Vector3 scale, int fontSize, TextAnchor alignment, Color textColor)
        {
            var text = parent.Ensure<RectTransform>(name);
            if (text.IsNew)
            {
                text.SetAnchors(0.5f * Vector2.one, 0.5f * Vector2.one)
                    .SetPivot(0.5f * Vector2.one)
                    .SetScale(scale)
                    .SetPosition(position)
                    .SetSize(width, height);
            }

            var textTxt = text
                .Ensure<CanvasRenderer>()
                .Ensure<Text>();
            if (textTxt.IsNew)
            {
                textTxt.Value.font = ResourceExt.LoadAsset<Font>("Assets/PicoMobileSDK/Pvr_Controller/MicrosoftYaHeiGB.ttf");
                textTxt.Value.fontStyle = FontStyle.Normal;
                textTxt.Value.fontSize = fontSize;
                textTxt.Value.lineSpacing = 1;
                textTxt.Value.supportRichText = true;
                textTxt.Value.alignment = alignment;
                textTxt.Value.alignByGeometry = false;
                textTxt.Value.horizontalOverflow = HorizontalWrapMode.Wrap;
                textTxt.Value.verticalOverflow = VerticalWrapMode.Truncate;
                textTxt.Value.resizeTextForBestFit = false;
                textTxt.Value.color = textColor;
                textTxt.Value.raycastTarget = true;
            }

            return text;
        }

        private static RectTransform MakeImage(Transform parent, string name, Vector3 position, float width, float height, Sprite sprite, Image.Type type)
        {
            var image = parent.Ensure<RectTransform>(name);
            if (image.IsNew)
            {
                image.SetAnchors(0.5f * Vector2.one, 0.5f * Vector2.one)
                    .SetPivot(0.5f * Vector2.one)
                    .SetSize(width, height)
                    .SetPosition(position);
            }

            var imageImage = image
                .Ensure<CanvasRenderer>()
                .Ensure<Image>();
            if (imageImage.IsNew)
            {
                imageImage.Value.sprite = sprite;
                imageImage.Value.color = Color.white;
                imageImage.Value.raycastTarget = true;
                imageImage.Value.type = type;
                imageImage.Value.fillCenter = true;
                imageImage.Value.preserveAspect = false;
                imageImage.Value.useSpriteMesh = false;
            }

            return image;
        }

        private static void MakeViewerToast(Transform parent)
        {
            var panel = MakePanel(
                parent,
                "Viewertoast",
                2.5f * Vector3.forward,
                Quaternion.identity,
                Vector3.zero,
                300, 80,
                new Color(27f / 255, 27f / 255, 27f / 255, 204f / 255));

            MakeText(
                panel,
                "title",
                Vector3.zero,
                500, 100,
                0.5f * Vector3.one,
                30, TextAnchor.MiddleCenter,
                new Color(1f, 0.57f, 0f));
        }

        private static void MakeSafeToast(Transform parent)
        {
            var panel = MakePanel(
                parent,
                "SafeToast",
                Vector3.zero,
                Quaternion.identity,
                Vector3.zero,
                400, 400,
                new Color(27f / 255, 27f / 255, 27f / 255, 1));

            MakeText(
                panel,
                "title",
                153 * Vector3.up,
                415, 50,
                Vector3.one,
                30, TextAnchor.MiddleCenter,
                new Color(1f, 0.57f, 0f));

            MakeText(
                panel,
                "Text",
                -8 * Vector3.up,
                350, 250,
                Vector3.one,
                25, TextAnchor.UpperLeft,
                Color.white);

            MakeImage(
                panel,
                "Image",
                -82 * Vector3.up,
                200, 200,
                ResourceExt.LoadAsset<Sprite>("Assets/PicoMobileSDK/Pvr_Controller/Texture/0.8M.png"),
                Image.Type.Simple);
        }

        private static void MakeSafePanel1(Transform parent)
        {
            var panel = MakePanel(
                parent,
                "SafePanel1",
                Vector3.zero,
                Quaternion.identity,
                3 * Vector3.forward,
                400, 400,
                new Color(27f / 255, 27f / 255, 27f / 255, 1));

            panel.Ensure<GraphicRaycaster>();

            MakeText(
                panel,
                "toast1",
                83 * Vector3.up,
                340, 170,
                Vector3.one,
                25, TextAnchor.UpperLeft,
                Color.white);

            MakeImage(
                panel,
                "Image",
                -82 * Vector3.up,
                200, 200,
                ResourceExt.LoadAsset<Sprite>("Assets/PicoMobileSDK/Pvr_Controller/Texture/0.8M.png"),
                Image.Type.Simple);
        }

        private static PooledComponent<Button> MakeSafePanel2(Transform parent, PooledComponent<Pvr_UnitySDKManager> sdkMgr)
        {
            var panel = MakePanel(
                parent,
                "SafePanel2",
                Vector3.zero,
                Quaternion.identity,
                3 * Vector3.forward,
                400, 400,
                new Color(27f / 255, 27f / 255, 27f / 255, 1));

            panel.Ensure<GraphicRaycaster>();

            MakeText(
                panel,
                "Title",
                159 * Vector3.up,
                200, 45,
                Vector3.one,
                30, TextAnchor.MiddleCenter,
                new Color(1f, 0.57f, 0f));

            MakeText(
                panel,
                "toast2",
                5.3f * Vector3.up,
                359, 132,
                Vector3.one,
                25, TextAnchor.MiddleCenter,
                Color.white);

            var forceQuit = MakeImage(
                panel,
                "forcequitBtn",
                -82 * Vector3.up,
                200, 35,
                ResourceExt.LoadAsset<Sprite>("Assets/PicoMobileSDK/Pvr_Controller/Texture/Bt_background_long1.png"),
                Image.Type.Sliced);

            var forceQuitButton = forceQuit.Ensure<Button>();
            if (forceQuitButton.IsNew)
            {
                forceQuitButton.Value.interactable = true;
                forceQuitButton.Value.transition = Selectable.Transition.ColorTint;
                forceQuitButton.Value.targetGraphic = forceQuit.GetComponent<Image>();
                forceQuitButton.Value.colors = new ColorBlock
                {
                    normalColor = Color.white,
                    highlightedColor = Color.blue,
                    pressedColor = Color.red,
                    disabledColor = Color.grey,
                    colorMultiplier = 1,
                    fadeDuration = 0.1f
                };
                forceQuitButton.Value.navigation = new Navigation
                {
                    mode = Navigation.Mode.Automatic
                };
            }

            var forceQuitText = forceQuit
                .Ensure<RectTransform>("Text")
                .Ensure<Text>();
            if (forceQuitText.IsNew)
            {
                forceQuitText.Value.font = ResourceExt.LoadAsset<Font>("Assets/PicoMobileSDK/Pvr_Controller/MicrosoftYaHeiGB.ttf");
                forceQuitText.Value.fontStyle = FontStyle.Normal;
                forceQuitText.Value.fontSize = 20;
                forceQuitText.Value.lineSpacing = 1;
                forceQuitText.Value.supportRichText = true;
                forceQuitText.Value.alignment = TextAnchor.MiddleCenter;
                forceQuitText.Value.alignByGeometry = false;
                forceQuitText.Value.horizontalOverflow = HorizontalWrapMode.Wrap;
                forceQuitText.Value.verticalOverflow = VerticalWrapMode.Truncate;
                forceQuitText.Value.resizeTextForBestFit = false;
                forceQuitText.Value.color = new Color(50f / 255, 50f / 255, 50f / 255);
                forceQuitText.Value.raycastTarget = true;
            }

            if (forceQuitButton.IsNew || sdkMgr.IsNew)
            {
                forceQuitButton.Value.onClick.AddListener(sdkMgr.Value.SixDofForceQuit);
            }

            return forceQuitButton;
        }

        private static void MakeResetPanel(Transform parent)
        {
            var panel = MakePanel(
                parent,
                "ResetPanel",
                Vector3.zero,
                Quaternion.identity,
                3 * Vector3.forward,
                400, 400,
                new Color(27f / 255, 27f / 255, 27f / 255, 1));

            panel.Ensure<GraphicRaycaster>();

            MakeText(
                panel,
                "toast",
                new Vector3(3, 88, 0),
                340, 170,
                Vector3.one,
                25, TextAnchor.UpperLeft,
                Color.white);

            MakeImage(
                panel,
                "Image",
                -82 * Vector3.up,
                200, 200,
                ResourceExt.LoadAsset<Sprite>("Assets/PicoMobileSDK/Pvr_Controller/Texture/0.8M.png"),
                Image.Type.Simple);
        }

        private static PooledComponent<Pvr_UnitySDKManager> MakeSDKManager(Transform parent)
        {
            var sdkMgr = parent.Ensure<Pvr_UnitySDKManager>();
            if (sdkMgr.IsNew)
            {
                sdkMgr.Value.RtAntiAlising = RenderTextureAntiAliasing.X_2;
                sdkMgr.Value.RtBitDepth = RenderTextureDepth.BD_24;
                sdkMgr.Value.RtFormat = RenderTextureFormat.Default;
                sdkMgr.Value.DefaultRenderTexture = false;
                sdkMgr.Value.RtLevel = RenderTextureLevel.High;
                sdkMgr.Value.ShowFPS = false;
                sdkMgr.Value.ShowSafePanel = false;
                sdkMgr.Value.ScreenFade = false;
                sdkMgr.Value.HeadDofNum = HeadDofNum.ThreeDof;
                sdkMgr.Value.HandDofNum = HandDofNum.ThreeDof;
                sdkMgr.Value.SixDofRecenter = false;
                sdkMgr.Value.DefaultRange = true;
                sdkMgr.Value.MovingRatios = 1;
            }

            return sdkMgr;
        }

        private static void MakeControllerManager(Transform parent)
        {
            var ctrlMgr = parent.Ensure<Pvr_ControllerManager>();
            if (ctrlMgr.IsNew)
            {
                ctrlMgr.Value.toast = ScreenDebugger.TextBox;
            }
        }

        private static void MakeController(Transform parent)
        {
            var ctrl = parent.Ensure<Pvr_Controller>();
            if (ctrl.IsNew)
            {
                ctrl.Value.Axis = Pvr_Controller.ControllerAxis.Controller;
                ctrl.Value.Gazetype = Pvr_Controller.GazeType.Never;
            }
        }

        public override bool HasFloorPosition { get { return false; } }

        public override InputMode DefaultInputMode { get { return InputMode.SeatedVR; } }
    }
}
#endif
