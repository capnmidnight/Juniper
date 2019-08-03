using System;
using System.Runtime.Serialization;

namespace Juniper.Google.Maps.Places
{
    [Serializable]
    public class PlaceSearchResponse : ISerializable
    {
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotImplementedException();
        }

        protected PlaceSearchResponse(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
