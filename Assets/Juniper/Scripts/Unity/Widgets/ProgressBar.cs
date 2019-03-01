using UnityEngine;
using UnityEngine.UI;

namespace Juniper.Widgets
{
    [ExecuteInEditMode]
    public class ProgressBar : MonoBehaviour
    {
        [Range(0, 1)]
        public float value;

        public void Awake()
        {
            bar = GetComponentInChildren<Image>();
            rect = GetComponent<RectTransform>();
        }

        public void Update()
        {
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
