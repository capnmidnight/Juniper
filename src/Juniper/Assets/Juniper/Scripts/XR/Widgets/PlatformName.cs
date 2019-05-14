using UnityEngine;

#if UNITY_MODULES_UI
using UnityEngine.UI;
#endif

#if UNITY_TEXTMESHPRO
using TMPro;
#endif


namespace Juniper.Widgets
{
    /// <summary>
    /// GameObjects with this component will be hidden in the live application view, only appearing
    /// in the Unity Editor.
    /// </summary>
    public class PlatformName : MonoBehaviour
    {
        public void Awake()
        {
#if UNITY_MODULES_UI
            var text = GetComponent<Text>();
            if (text != null)
            {
                text.text = text.text.Replace("{Juniper:Platform}", JuniperPlatform.CurrentPlatform.ToString());
            }

            var textMesh = GetComponent<TextMesh>();
            if (textMesh != null)
            {
                textMesh.text = textMesh.text.Replace("{Juniper:Platform}", JuniperPlatform.CurrentPlatform.ToString());
            }
#endif

#if UNITY_TEXTMESHPRO
            var tmp = GetComponent<TMP_Text>();
            if (tmp != null)
            {
                tmp.text = tmp.text.Replace("{Juniper:Platform}", JuniperPlatform.CurrentPlatform.ToString());
            }
#endif
        }
    }
}
