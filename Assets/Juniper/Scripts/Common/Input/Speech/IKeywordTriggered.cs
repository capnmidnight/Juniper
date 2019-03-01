using System.Collections.Generic;

namespace Juniper.Input.Speech
{
    /// <summary>
    /// An interface for systems that respond to keyword utterances.
    /// </summary>
    public interface IKeywordTriggered
    {
        /// <summary>
        /// The keywords that will trigger a response in the implementor of this interface.
        /// </summary>
        /// <value>The keywords.</value>
        IEnumerable<string> Keywords { get; }
    }
}
