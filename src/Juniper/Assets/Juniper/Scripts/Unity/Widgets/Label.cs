using Juniper.Display;

using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// A text box with an opaque background that floats in mid-air and has a connecting line to the
    /// object it is labeling.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(LineRenderer))]
    public class Label : MonoBehaviour
    {
        /// <summary>
        /// The object that is being labeled.
        /// </summary>
        public Transform target;

        /// <summary>
        /// The text to apply as the label to the <see cref="target"/>.
        /// </summary>
        public string label;

        /// <summary>
        /// Used to resize the text to make the text fill the backing area better.
        /// </summary>
        public float rescale = 100;

        /// <summary>
        /// When set to true, a connecting line is drawn between the <see cref="text"/> and the <see cref="target"/>.
        /// </summary>
        public bool drawConnectingLine = true;

        /// <summary>
        /// Gets the text renderer and attempts to find the line renderer on this element.
        /// </summary>
        public void Awake()
        {
            text = GetComponentInChildren<TextMesh>();

            line = GetComponent<LineRenderer>();
            if (line != null)
            {
                line.positionCount = 2;
            }

            camT = DisplayManager.MainCamera.transform;
        }

        /// <summary>
        /// Sets the position of the connecting line, updates the text on the TextMesh element, and
        /// resizes the text as requested.
        /// </summary>
        public void Update()
        {
            if (line != null && transform.parent != null)
            {
                line.enabled = drawConnectingLine;
                line.SetPosition(0, transform.position);
                if (target == null)
                {
                    line.SetPosition(1, transform.parent.position);
                }
                else
                {
                    line.SetPosition(1, target.position);
                }
            }

            var delta = transform.position - camT.position;
            delta.y = 0;
            transform.rotation = Quaternion.LookRotation(delta);

            if (label != lastLabel || System.Math.Abs(rescale - lastRescale) > 0.01f)
            {
                if (string.IsNullOrEmpty(label))
                {
                    label = transform.parent.name;
                }

                text.text = label.ToUpperInvariant();

                text.transform.localScale = rescale / Mathf.Sqrt(label.Length) * Vector3.one;

                lastRescale = rescale;
                lastLabel = label;
            }
        }

        /// <summary>
        /// The <see cref="label"/> text at the end of the last frame, used to detect when the text
        /// has changed.
        /// </summary>
        private string lastLabel;

        /// <summary>
        /// The text element to which the label text is being rendered.
        /// </summary>
        private TextMesh text;

        /// <summary>
        /// The value of <see cref="rescale"/> at the end of the last frame, used to detect changes.
        /// </summary>
        private float lastRescale;

        /// <summary>
        /// The renderer used to draw the connecting line between the <see cref="text"/> and the <see cref="target"/>.
        /// </summary>
        private LineRenderer line;

        /// <summary>
        /// The main camera.
        /// </summary>
        private Transform camT;
    }
}
