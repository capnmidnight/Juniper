using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Juniper.Sound
{
    /// <summary>
    /// A collection of related audio clips. For example, one might have many different gunshot
    /// sounds to have some variety between shots. The audio clip collection can be used to get a
    /// random clip every time a clip is needed, and also randomize the pitch of the clip.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "AudioClipCollection", menuName = "Configuration/Audio Clips")]
    public class AudioClipCollection : ScriptableObject, IAudioClipCollection
    {
        /// <summary>
        /// Whether or not the pitch on the clip that is about to be played should be twiddled a
        /// little bit, to add variety between repeat plays of the clip.
        /// </summary>
        public bool randomizePitch = true;

#if UNITY_MODULES_AUDIO

        /// <summary>
        /// All the clips that need to play
        /// </summary>
        public AudioClip[] clips;

        /// <summary>
        /// Get the number of clips in the collection.
        /// </summary>
        public int Length
        {
            get
            {
                return clips.Length;
            }
        }

        /// <summary>
        /// Tells the interaction engine to randomize the pitch of audio clips that are played in sequence.
        /// </summary>
        public bool RandomizeClipPitch
        {
            get
            {
                return randomizePitch;
            }
        }

        /// <summary>
        /// Get a specific item out of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AudioClip this[int index]
        {
            get
            {
                return clips[index];
            }
        }

        /// <summary>
        /// Get a random item out of the collection.
        /// </summary>
        /// <returns></returns>
        public AudioClip Random()
        {
            return clips.Random();
        }

        /// <summary>
        /// Get an enumerator over the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<AudioClip> GetEnumerator()
        {
            foreach(var clip in clips)
            {
                yield return clip;
            }
        }

        /// <summary>
        /// Get a non-generic enumerator over the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return clips.GetEnumerator();
        }

#endif
    }
}
