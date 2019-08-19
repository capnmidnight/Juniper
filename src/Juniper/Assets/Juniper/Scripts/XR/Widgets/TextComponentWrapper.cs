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
            {
#if UNITY_MODULES_UI
                unityText = GetComponent<Text>();
                unityTextMesh = GetComponent<TextMesh>();
#endif

#if UNITY_TEXTMESHPRO
                textMeshPro = GetComponent<TMP_Text>();
#endif
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