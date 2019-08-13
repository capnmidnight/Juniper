using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Yarrow.Client
{
    [Serializable]
    public class YarrowAudioSource : ISerializable
    {
        private readonly Vector3 position;

        private readonly float volume;

        private readonly bool spatialized;

        private readonly bool repeat;

        private readonly string relativeFilePath;

        public YarrowAudioSource(Vector3 position, float volume, bool spatialized, bool repeat, string relativeFilePath)
        {
            this.position = position;
            this.volume = volume;
            this.spatialized = spatialized;
            this.repeat = repeat;
            this.relativeFilePath = relativeFilePath;
        }

        protected YarrowAudioSource(SerializationInfo info, StreamingContext context)
        {
            position = info.GetVector3(nameof(position));
            volume = info.GetSingle(nameof(volume));
            spatialized = info.GetBoolean(nameof(spatialized));
            repeat = info.GetBoolean(nameof(repeat));
            relativeFilePath = info.GetString(nameof(relativeFilePath));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddVector3(nameof(position), position);
            info.AddValue(nameof(volume), volume);
            info.AddValue(nameof(spatialized), spatialized);
            info.AddValue(nameof(repeat), repeat);
            info.AddValue(nameof(relativeFilePath), relativeFilePath);
        }

        public Vector3 Position { get { return position; } }

        public float Volume { get { return volume; } }

        public bool IsSpatialized { get { return spatialized; } }

        public bool IsRepeating { get { return repeat; } }

        public string RelativeFilePath { get { return relativeFilePath; } }
    }
}