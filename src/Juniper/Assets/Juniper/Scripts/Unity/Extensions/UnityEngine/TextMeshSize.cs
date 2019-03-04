using System.Collections.Generic;

namespace UnityEngine
{
    /// <summary>
    /// Calculates the width of a set of text that is rendered to a TextMesh object, for being able
    /// to resize backing labels to completely back them.
    /// </summary>
    public class TextMeshSize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnityEngine.TextMeshSize"/> class.
        /// </summary>
        /// <param name="tm">Tm.</param>
        public TextMeshSize(TextMesh tm)
        {
            mesh = tm;
            renderer = tm.GetComponent<Renderer>();
            charWidths = new Dictionary<char, float>();

            //the space can not be got alone
            var aw = GetTextWidth("a");
            mesh.text = "a a";
            var cw = CurrentWidth - 2 * aw;

            charWidths.Add(' ', cw);
        }

        /// <summary>
        /// The width of the text on the TextMesh.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get
            {
                if (mesh.text != lastText)
                {
                    lastText = mesh.text;
                    lastWidth = GetTextWidth(mesh.text);
                }
                return lastWidth;
            }
        }

        /// <summary>
        /// Inserts newline characters into the text to make its maximum width fit the given
        /// <paramref name="targetWidth"/>.
        /// </summary>
        /// <param name="targetWidth">Target width.</param>
        /// <param name="maxLines">Max lines.</param>
        public void FitToWidth(float targetWidth, int maxLines = -1)
        {
            var oldText = mesh.text;
            mesh.text = "";

            var lines = oldText.Split('\n');

            var numLines = 0;
            foreach (var line in lines)
            {
                mesh.text += WrapLines(line, targetWidth, maxLines - numLines);
                numLines++;
                if (maxLines != -1 && numLines >= maxLines)
                {
                    return;
                }
                mesh.text += "\n";
            }
        }

        /// <summary>
        /// The widths of individual characters.
        /// </summary>
        private readonly Dictionary<char, float> charWidths;

        /// <summary>
        /// A temporary TextMesh to which to test-render text to figure out the character width for
        /// each character. This won't handle kerning very well, but Unity's text engines generally
        /// don't do kerning and it won't impact the use-case of resizing a rectangle *that* much.
        /// </summary>
        private readonly TextMesh mesh;

        /// <summary>
        /// The renderer from the TextMesh.
        /// </summary>
        private readonly Renderer renderer;

        /// <summary>
        /// The transform that holds the TextMesh.
        /// </summary>
        private Transform parent;

        /// <summary>
        /// The text that was being measured in the last frame. Used to skip measuring the text if
        /// the text didn't change.
        /// </summary>
        private string lastText;

        /// <summary>
        /// The width of the text from the last request for text width.
        /// </summary>
        private float lastWidth;

        /// <summary>
        /// The scale value for the TextMesh from the previous frame.
        /// </summary>
        private Vector3 oldScale;

        /// <summary>
        /// The rotation value of the TextMesh from the previous frame.
        /// </summary>
        private Quaternion oldRotation;

        /// <summary>
        /// When set to true, the resizer won't perform a width calculation. This is to avoid
        /// situations where different coroutines could be attempting to use the same mesh sizer concurrently.
        /// </summary>
        private bool locked;

        /// <summary>
        /// The width of the text mesh as it has currently been set.
        /// </summary>
        /// <value>The width of the current.</value>
        private float CurrentWidth
        {
            get
            {
                var lockAcquired = Lock();
                var cw = renderer.bounds.size.x;
                if (lockAcquired)
                {
                    Unlock();
                }
                return cw;
            }
        }

        /// <summary>
        /// Measures a string by testing the rendering of individual characters.
        /// </summary>
        /// <returns>The text width.</returns>
        /// <param name="s">S.</param>
        private float GetTextWidth(string s)
        {
            float width = 0;
            var oldText = mesh.text;
            Lock();
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];

                if (charWidths.ContainsKey(c))
                {
                    width += charWidths[c];
                }
                else
                {
                    mesh.text = "" + c;

                    var cw = CurrentWidth;

                    width += cw;
                    charWidths.Add(c, cw);
                }
            }
            Unlock();
            mesh.text = oldText;
            return width;
        }

        /// <summary>
        /// Locks the text mesh to a particular size while measuring the text size.
        /// </summary>
        /// <returns>The lock.</returns>
        private bool Lock()
        {
            if (!locked)
            {
                locked = true;
                parent = renderer.transform.parent;
                oldScale = parent.localScale;
                var newScale = Vector3.one;
                var head = parent.parent;
                while (head != null)
                {
                    var s = head.localScale;
                    newScale = new Vector3(
                        newScale.x / s.x,
                        newScale.y / s.y,
                        newScale.z / s.z);
                    head = head.parent;
                }
                parent.localScale = newScale;
                oldRotation = renderer.transform.rotation;
                renderer.transform.rotation = Quaternion.identity;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// <see cref="Lock"/>. Does the opposite of that.
        /// </summary>
        private void Unlock()
        {
            if (locked)
            {
                renderer.transform.rotation = oldRotation;
                parent.localScale = oldScale;
                locked = false;
            }
        }

        /// <summary>
        /// Measures the text while fitting it into a maximum width.
        /// </summary>
        /// <returns>The lines.</returns>
        /// <param name="s">S.</param>
        /// <param name="w">The width.</param>
        /// <param name="maxLines">Max lines.</param>
        private string WrapLines(string s, float w, int maxLines = -1)
        {
            // need to check if smaller than maximum character length, really...
            if (w > 0 && s.Length > 0)
            {
                float wordWidth = 0;
                float currentWidth = 0;

                var word = "";
                var newText = "";
                var oldText = mesh.text;

                var numLines = 0;

                for (var i = 0; i < s.Length; i++)
                {
                    var c = s[i];

                    float charWidth = 0;
                    if (charWidths.ContainsKey(c))
                    {
                        charWidth = charWidths[c];
                    }
                    else
                    {
                        mesh.text = "" + c;
                        var temp = renderer.transform.rotation;
                        renderer.transform.rotation = Quaternion.identity;
                        charWidth = renderer.bounds.size.x;
                        renderer.transform.rotation = temp;
                        charWidths.Add(c, charWidth);
                        //here check if max char length
                    }

                    if (c == ' ' || i == s.Length - 1)
                    {
                        if (c != ' ')
                        {
                            word += c.ToString();
                            wordWidth += charWidth;
                        }

                        if (currentWidth + wordWidth < w)
                        {
                            currentWidth += wordWidth;
                            newText += word;
                        }
                        else
                        {
                            if (maxLines != -1 && numLines >= maxLines)
                            {
                                break;
                            }

                            currentWidth = wordWidth;
                            newText += word.Replace(" ", "\n");
                            numLines++;
                        }

                        word = "";
                        wordWidth = 0;
                    }

                    word += c.ToString();
                    wordWidth += charWidth;
                }

                mesh.text = oldText;
                return newText;
            }
            else
            {
                return s;
            }
        }
    }
}