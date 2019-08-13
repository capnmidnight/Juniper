using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Yarrow.Client
{
    [Serializable]
    public class YarrowInteractionPoint : ISerializable
    {
        private readonly Vector3 position;
        private readonly Quaternion orientation;
        private readonly string normalRelativeFilePath;
        private readonly string hoverRelativeFilePath;
        private readonly string pressRelativeFilePath;
        private readonly string disabledRelativeFilePath;

        public YarrowInteractionPoint(Vector3 position, Quaternion orientation, string normalRelativeFilePath, string hoverRelativeFilePath = null, string pressRelativeFilePath = null, string disabledRelativeFilePath = null)
        {
            this.position = position;
            this.orientation = orientation;
            this.normalRelativeFilePath = normalRelativeFilePath;
            this.hoverRelativeFilePath = hoverRelativeFilePath;
            this.pressRelativeFilePath = pressRelativeFilePath;
            this.disabledRelativeFilePath = disabledRelativeFilePath;
        }

        protected YarrowInteractionPoint(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public Vector3 Position { get { return position; } }

        public Quaternion Orientation { get { return orientation; } }

        public string NormalRelativeFilePath { get { return normalRelativeFilePath; } }

        public string HoverRelativeFilePath { get { return hoverRelativeFilePath ?? normalRelativeFilePath; } }

        public string PresskRelativeFilePath { get { return pressRelativeFilePath ?? normalRelativeFilePath; } }

        public string DisabledRelativeFilePath { get { return disabledRelativeFilePath ?? normalRelativeFilePath; } }
    }
}