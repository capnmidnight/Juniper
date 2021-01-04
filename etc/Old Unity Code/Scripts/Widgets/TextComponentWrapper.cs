﻿using UnityEngine;

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
        [SerializeField]
        [HideInNormalInspector]
        private Text unityText;

        [SerializeField]
        [HideInNormalInspector]
        private TextMesh unityTextMesh;
#endif

#if UNITY_TEXTMESHPRO
        [SerializeField]
        [HideInNormalInspector]
        private TMP_Text textMeshPro;
#endif

        public void Awake()
        {
            SetupControls();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
#if UNITY_MODULES_UI
            unityText = null;
            unityTextMesh = null;
#endif

#if UNITY_TEXTMESHPRO
            textMeshPro = null;
#endif

            SetupControls();
        }
#endif

        private T Get<T>()
        {
            foreach(var trans in transform.Family())
            {
                var v = trans.GetComponent<T>();
                if(v != null)
                {
                    return v;
                }
            }

            return default;
        }

        internal void SetupControls()
        {
#if UNITY_MODULES_UI
            if (unityText == null)
            {
                unityText = Get<Text>();
            }

            if (unityTextMesh == null)
            {
                unityTextMesh = Get<TextMesh>();
            }
#endif

#if UNITY_TEXTMESHPRO
            if (textMeshPro == null)
            {
                textMeshPro = Get<TMP_Text>();
            }
#endif
        }

        public string Text
        {
            get
            {
#if UNITY_TEXTMESHPRO
                if (textMeshPro != null)
                {
                    return textMeshPro.text;
                }
#endif

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
                return string.Empty;
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