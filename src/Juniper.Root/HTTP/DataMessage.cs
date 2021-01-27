using System;
using System.Runtime.Serialization;

namespace Juniper.HTTP
{
    [Serializable]
    public sealed class DataMessage : ISerializable
    {
        public string Message { get; }

        public System.Collections.Generic.IReadOnlyCollection<byte> Data { get; }

        public DataMessage(string message, byte[] data)
        {
            Message = message;
            Data = data;
        }

        private DataMessage(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Message = info.GetString(nameof(Message));
            Data = info.GetValue<byte[]>(nameof(Data));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Message), Message);
            info.AddValue(nameof(Data), Data);
        }
    }
}
