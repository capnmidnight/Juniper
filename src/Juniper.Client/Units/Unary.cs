namespace Juniper
{
    /// <summary>
    /// A type with a single value. It's sometimes useful to
    /// have such a thing when we use generics and want to
    /// indicate the lack of options in the specialization
    /// of the generic structure.
    /// </summary>
    public enum Unary
    {
        /// <summary>
        /// The only value of the enumeration.
        /// </summary>
        One
    }
}