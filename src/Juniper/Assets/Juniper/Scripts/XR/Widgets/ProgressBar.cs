using UnityEngine;
using UnityEngine.UI;

namespace Juniper.Unity.Widgets
{
    [ExecuteInEditMode]
    public class ProgressBar : MonoBehaviour
    {
        [Range(0, 1)]
        public float value;

        public void Update()
        {
            if(bar == null)
            {
                bar = GetComponentInChildren<Image>();
            }

            if(rect == null)
            {
                rect = GetComponent<RectTransform>();
            }
            value = Mathf.Clamp01(value);
            bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.rect.width * value);
        }

        public void SetValue(int isOver, int of)
        {
            if (of == 0)
            {
                value = 0;
            }
            else
            {
                value = isOver / (float)of;
            }
        }

        private Image bar;
        private RectTransform rect;
    }
}
