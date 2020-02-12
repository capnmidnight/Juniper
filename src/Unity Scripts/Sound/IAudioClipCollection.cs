using UnityEngine;

#if UNITY_MODULES_AUDIO

using System.Collections.Generic;

#endif

namespace Juniper.Sound
{
    /// <summary>
    /// The IAudioClipCollection interface is implemented by two classes. One is a savable collection
    /// of audio clips. The other is a wrapper around a single audio clip to make it easier to work
    /// with collections and single clips in the same code.
    /// </summary>
    public interface IAudioClipCollection
#if UNITY_MODULES_AUDIO
: IEnumerable<AudioClip>
#endif
    {
#if UNITY_MODULES_AUDIO

        /// <summary>
        /// Get the number of clips in the collection.
        /// </summary>
        int Length
        {
            get;
        }

        /// <summary>
        /// Tells the interaction engine to randomize the pitch of audio clips that are played in sequence.
        /// </summary>
        bool RandomizeClipPitch
        {
            get;
        }

        /// <summary>
        /// Get a specific item out of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        AudioClip this[int index] { get; }

        /// <summary>
        /// Get a random item out of the collection.
        /// </summary>
        /// <returns></returns>
        AudioClip Random();

#endif
    }
}
