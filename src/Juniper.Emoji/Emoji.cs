namespace Juniper
{
    public partial class Emoji
    {
        public string Value { get; }
        public string Desc { get; }

        public Emoji(string value, string description)
        {
            Value = value;
            Desc = description;
        }

        public Emoji(Emoji copy)
        {
            Value = copy.Value;
            Desc = copy.Desc;
        }

        public virtual Emoji Random()
        {
            return this;
        }

        /// <summary>
        /// Determines of the provided Emoji or EmojiGroup is a subset of this emoji.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual bool Contains(Emoji e)
        {
            return Value.IndexOf(e.Value) >= 0;
        }
    }
}
