using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Flags : ISerializable, ICollection<string>
    {
        private readonly List<string> flags;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected Flags(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            flags = new List<string>();
            foreach (var field in info)
            {
                flags.Add(field.Name);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            foreach (var flag in flags)
            {
                info.AddValue(flag, 1);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var flag in flags)
            {
                yield return flag;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator
            return GetEnumerator();
#pragma warning restore HAA0401 // Possible allocation of reference type enumerator
        }

        public void Add(string item)
        {
            flags.Add(item);
        }

        public void Clear()
        {
            flags.Clear();
        }

        public bool Contains(string item)
        {
            return flags.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            for (var i = 0; i < flags.Count; ++i)
            {
                array[arrayIndex + i] = flags[i];
            }
        }

        public bool Remove(string item)
        {
            return flags.Remove(item);
        }

        public int Count { get { return flags.Count; } }

        public bool IsReadOnly { get { return false; } }
    }
}