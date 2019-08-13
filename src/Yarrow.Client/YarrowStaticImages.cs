using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Yarrow.Client
{
    [Serializable]
    public class YarrowStaticImages : ISerializable
    {
        private readonly Vector3 position;

        private readonly Quaternion orientation;

        private readonly string relativeFilePath;

        public YarrowStaticImages(Vector3 position, Quaternion orientation, string relativeFilePath)
        {
            this.position = position;
            this.orientation = orientation;
            this.relativeFilePath = relativeFilePath;
        }

        protected YarrowStaticImages(SerializationInfo info, StreamingContext context)
        {
            position = info.GetVector3(nameof(position));
            orientation = info.GetQuaternion(nameof(orientation));
            relativeFilePath = info.GetString(nameof(relativeFilePath));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddVector3(nameof(position), position);
            info.AddQuaternion(nameof(orientation), orientation);
            info.AddValue(nameof(relativeFilePath), relativeFilePath);
        }

        public Vector3 Position { get { return position; } }

        public Quaternion Orientation { get { return orientation; } }

        public string RelativeFilePath { get { return relativeFilePath; } }
    }
}