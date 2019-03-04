using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Juniper.Widgets
{
    public class ListViewItem : MonoBehaviour
    {
        public bool interactable = true;
        public Color normalColor;
        public Color disabledColor;

        public Color BackgroundColor
        {
            get
            {
                return img.color;
            }
            set
            {
                img.color = value;
            }
        }

        public Font Font
        {
            get
            {
                return text.font;
            }
            set
            {
                text.font = value;
            }
        }

        public int FontSize
        {
            get
            {
                return text.fontSize;
            }
            set
            {
                img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value * 1.2f);
                text.fontSize = value;
            }
        }

        public string Text
        {
            get
            {
                return text.text;
            }
            set
            {
                text.text = value;
            }
        }

        public string Key
        {
            get; set;
        }

        public object DataItem
        {
            get; set;
        }

        public void Update()
        {
            btn.interactable = interactable;
            text.color = interactable ? normalColor : disabledColor;
        }

        public void Awake()
        {
            img = this.EnsureComponent<Image>();
            img.rectTransform.anchorMin = Vector2.up;
            img.rectTransform.anchorMax = Vector2.up;
            img.rectTransform.pivot = Vector2.up;

            btn = this.EnsureComponent<Button>();

            text = GetComponentInChildren<Text>();
            if (text == null)
            {
                var txtGo = new GameObject();
                txtGo.transform.SetParent(transform, false);
                text = txtGo.AddComponent<Text>();
            }

            text.alignment = TextAnchor.MiddleLeft;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
        }

        public void RemoveAllListeners()
        {
            btn.onClick.RemoveAllListeners();
        }

        public void AddListener(UnityAction action)
        {
            btn.onClick.AddListener(action);
        }

        private Image img;
        private Button btn;
        private Text text;
    }
}