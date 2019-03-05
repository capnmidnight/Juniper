using UnityEngine;

namespace Juniper.Unity.Widgets
{
    /// <summary>
    /// A combination of a TextMesh object and a background label, where the text can resize to fill
    /// the background label area. Only one of this component is allowed on a gameObject at a time.
    /// This component executes in edit mode.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class Tooltip : MonoBehaviour
    {
        /// <summary>
        /// The text to display on <see cref="lbl"/>.
        /// </summary>
        public string text;

        /// <summary>
        /// The amount of room to include on the left and right sides of the text as padding.
        /// </summary>
        public float pad = 0.1f;

        /// <summary>
        /// The amount to multiply the character size and number of characters to get the final width
        /// of the <see cref="lbl"/>
        /// </summary>
        public float fudge = 1;

        /// <summary>
        /// When set to true, the tooltip will scale into view. When set to false, it will scale out
        /// of view.
        /// </summary>
        public bool show;

        /// <summary>
        /// The total width of the tooltip label.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get
            {
                if (sizer == null)
                {
                    return 0;
                }
                else
                {
                    return sizer.Width * fudge + pad;
                }
            }
        }

        /// <summary>
        /// The total height of the tooltip label.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get
            {
                if (bg == null)
                {
                    return 0;
                }
                else
                {
                    return bg.localScale.y;
                }
            }
        }

        /// <summary>
        /// Gets the label, creates the lablel sizer, and finds the background image.
        /// </summary>
        public void Awake()
        {
            lbl = gameObject.GetComponentInChildren<TextMesh>();
            if (lbl != null && string.IsNullOrEmpty(text))
            {
                text = lbl.text;
            }

            sizer = new TextMeshSize(lbl);

            bg = transform.Find("Background");
        }

        /// <summary>
        /// Updates the text on <see cref="lbl"/> from the <see cref="text"/> value of this
        /// component, resizes the tooltip to fit the text, and animates the label scaling into and
        /// out of view.
        /// </summary>
        public void Update()
        {
            if (lbl != null && text != lbl.text)
            {
                lbl.text = text;
            }

            if (bg != null && sizer != null)
            {
                var scale = bg.localScale;
                scale.x = Width;
                bg.localScale = scale;
            }

            var dt = Time.deltaTime * 10;
            var s = transform.localScale.x;
            if (show)
            {
                s += dt;
            }
            else
            {
                s -= dt;
            }

            s = Mathf.Clamp01(s);
            transform.localScale = s * Vector3.one;
        }

        /// <summary>
        /// The text renderer.
        /// </summary>
        private TextMesh lbl;

        /// <summary>
        /// An object that measures the sizes of TextMesh objects to determine how large the text
        /// needs to be to fill a certain area.
        /// </summary>
        private TextMeshSize sizer;

        /// <summary>
        /// The label background.
        /// </summary>
        private Transform bg;
    }
}
