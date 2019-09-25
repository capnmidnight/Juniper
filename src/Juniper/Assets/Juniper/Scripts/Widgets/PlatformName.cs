using UnityEngine;

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
            var text = this.Ensure<TextComponentWrapper>().Value;
            text.text = text.text.Replace("{Juniper:Platform}", JuniperSystem.CurrentPlatform.ToString());
        }
    }
}