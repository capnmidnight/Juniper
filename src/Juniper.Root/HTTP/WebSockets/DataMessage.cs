using System;
using System.Runtime.Serialization;

namespace Juniper.HTTP.WebSockets
{
    [Serializable]
    public sealed class DataMessage : ISerializable
    {
        public readonly string Message;
        public readonly byte[] Data;

        public DataMessage(string message, byte[] data)
        {
            Message = message;
            Data = data;
        }

        private DataMessage(SerializationInfo info, StreamingContext context)
        {
            Message = info.GetString(nameof(Message));
            Data = info.GetValue<byte[]>(nameof(Data));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Message), Message);
            info.AddValue(nameof(Data), Data);
        }
    }
}
