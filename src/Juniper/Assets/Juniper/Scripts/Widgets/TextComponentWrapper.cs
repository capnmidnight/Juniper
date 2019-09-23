using UnityEngine;

#if UNITY_MODULES_UI

using UnityEngine.UI;

#endif

#if UNITY_TEXTMESHPRO

using TMPro;

#endif

namespace Juniper.Widgets
{
    public class TextComponentWrapper : MonoBehaviour
    {
#if UNITY_MODULES_UI
        private Text unityText;
        private TextMesh unityTextMesh;
#endif

#if UNITY_TEXTMESHPRO
        private TMP_Text textMeshPro;
#endif

        public string text;
        private string lastText;

        public void Awake()
        {
            SetupControls();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetupControls();
        }
#endif

        private void SetupControls()
        {
            var hasAny = false;
            var isButton = GetComponent<Clickable>() != null
                || GetComponent<Tooltipable>() != null;

#if UNITY_MODULES_UI
                isButton |= GetComponent<Button>() != null;
#endif

            var textGameObject = gameObject;
            if (isButton)
            {
                foreach (var trans in transform.Children())
                {
#if UNITY_MODULES_UI
                    if (trans.GetComponent<Text>() != null
                        || trans.GetComponent<Text>() != null)
                    {
                        textGameObject = trans.gameObject;
                        break;
                    }
#endif

#if UNITY_TEXTMESHPRO
                    if (trans.GetComponent<TMP_Text>() != null)
                    {
                        textGameObject = trans.gameObject;
                        break;
                    }
#endif
                }
            }

#if UNITY_MODULES_UI
            unityText = textGameObject.GetComponent<Text>();
            unityTextMesh = textGameObject.GetComponent<TextMesh>();

            hasAny |= unityText != null || unityTextMesh != null;
#endif

#if UNITY_TEXTMESHPRO
            textMeshPro = textGameObject.GetComponent<TMP_Text>();
            hasAny |= textMeshPro != null;
#endif

            if (!hasAny)
            {
                var isCanvas = GetComponentInParent<Canvas>() != null;
#if UNITY_TEXTMESHPRO
                if (isCanvas)
                {
                    textMeshPro = textGameObject.AddComponent<TextMeshProUGUI>();
                }
                else
                {
                    textMeshPro = textGameObject.AddComponent<TextMeshPro>();
                }
#elif UNITY_MODULES_UI
                if (isCanvas)
                {
                    unityText = textGameObject.AddComponent<Text>();
                }
                else
                {
                    unityTextMesh = textGameObject.AddComponent<TextMesh>();
                }
#endif
            }

            lastText = text = Text;
        }

        public void Update()
        {
            if (text != lastText)
            {
                lastText = Text = text;
            }
            else
            {
                lastText = text = Text;
            }
        }

        private string Text
        {
            get
            {
#if UNITY_MODULES_UI
                if (unityText != null)
                {
                    return unityText.text;
                }

                if (unityTextMesh != null)
                {
                    return unityTextMesh.text;
                }
#endif

#if UNITY_TEXTMESHPRO
                if (textMeshPro != null)
                {
                    return textMeshPro.text;
                }
#endif

                return null;
            }

            set
            {
#if UNITY_MODULES_UI
                if (unityText != null)
                {
                    unityText.text = value;
                }

                if (unityTextMesh != null)
                {
                    unityTextMesh.text = value;
                }
#endif

#if UNITY_TEXTMESHPRO
                if (textMeshPro != null)
                {
                    textMeshPro.text = value;
                }
#endif
            }
        }
    }
}