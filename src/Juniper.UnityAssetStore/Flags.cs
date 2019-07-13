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

        protected Flags(SerializationInfo info, StreamingContext context)
        {
            flags = new List<string>();
            foreach(var field in info)
            {
                flags.Add(field.Name);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach(var flag in flags)
            {
                info.AddValue(flag, 1);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return flags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            for(int i = 0; i < flags.Count; ++i)
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
