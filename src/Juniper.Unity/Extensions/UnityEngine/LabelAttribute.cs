namespace UnityEngine
{
    /// <summary>
    /// Add labels to things.
    /// </summary>
    public sealed class LabelAttribute : PropertyAttribute
    {
        /// <summary>
        /// The label text.
        /// </summary>
        public string label;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityEngine.LabelAttribute"/> class.
        /// </summary>
        /// <param name="label">Label.</param>
        public LabelAttribute(string label)
        {
            this.label = label;
        }
    }
}