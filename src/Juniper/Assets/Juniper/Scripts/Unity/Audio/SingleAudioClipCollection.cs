using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Juniper.Audio
{
    /// <summary>
    /// Wraps around a single audio clip to make it interface with code that handles full collections
    /// of audio clips.
    /// </summary>
    public class SingleAudioClipCollection : IAudioClipCollection
    {
#if UNITY_MODULES_AUDIO

        /// <summary>
        /// Create a collection-like interface for a single audio clip.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="randomizePitch"></param>
        public SingleAudioClipCollection(AudioClip clip, bool randomizePitch = true)
        {
            this.clip = clip;
            RandomizeClipPitch = randomizePitch;
        }

        /// <summary>
        /// Get the number of clips in the collection (which should be 1).
        /// </summary>
        public int Length
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Tells the interaction engine to randomize the pitch of audio clips that are played in sequence.
        /// </summary>
        public bool RandomizeClipPitch
        {
            get;
        }

        /// <summary>
        /// Get the wrapped item out of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AudioClip this[int index]
        {
            get
            {
                return clip;
            }
        }

        /// <summary>
        /// Get the wrapped item out of the collection.
        /// </summary>
        /// <returns></returns>
        public AudioClip Random()
        {
            return clip;
        }

        /// <summary>
        /// Get an enumerator over the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<AudioClip> GetEnumerator()
        {
            yield return clip;
        }

        /// <summary>
        /// Satisfy the IEnumerable interface.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return clip;
        }

        /// <summary>
        /// The singular audio clip being wrapped.
        /// </summary>
        private readonly AudioClip clip;

#endif
    }
}