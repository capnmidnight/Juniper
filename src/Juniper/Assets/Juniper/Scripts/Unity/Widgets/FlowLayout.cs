using UnityEngine;

namespace Juniper.Widgets
{
    public class FlowLayout : MonoBehaviour
    {
        public RectOffset padding;

        public void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        public void Update()
        {
            var j = 0;
            var pos = Vector3.zero;
            pos.x = padding.left;
            pos.y -= padding.top;
            for (var i = 0; i < rect.childCount; ++i)
            {
                var child = rect.GetChild(i).GetComponent<RectTransform>();
                if (child.gameObject.activeInHierarchy)
                {
                    ++j;
                    child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                                                    rect.rect.width - padding.left - padding.right);
                    var p = Vector3.Lerp(child.transform.localPosition, pos, 0.5f);
                    var dy = (pos.y - child.transform.localPosition.y) / child.sizeDelta.y;
                    p.x = padding.left + Mathf.Sin(dy * Mathf.PI) * 10f;
                    child.transform.localPosition = p;
                    pos.y -= child.sizeDelta.y;

                    var subChild = child.GetChild(0).GetComponent<RectTransform>();
                    subChild.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, child.rect.width);
                    subChild.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, child.rect.height);
                }
            }

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, padding.top + padding.bottom - pos.y);
        }

        private RectTransform rect;
    }
}