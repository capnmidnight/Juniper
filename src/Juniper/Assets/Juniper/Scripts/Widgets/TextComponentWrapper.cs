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
            bool hasAny = false;

#if UNITY_MODULES_UI
            unityText = GetComponentInChildren<Text>(true);
            unityTextMesh = GetComponentInChildren<TextMesh>(true);
            hasAny |= unityText != null || unityTextMesh != null;
#endif

#if UNITY_TEXTMESHPRO
            textMeshPro = GetComponentInChildren<TMP_Text>(true);
            hasAny |= textMeshPro != null;
#endif

            if (hasAny)
            {
                lastText = text = Text;
            }
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

        public string Text
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