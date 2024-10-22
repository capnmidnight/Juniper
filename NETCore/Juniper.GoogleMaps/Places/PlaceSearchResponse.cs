using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.Places
{
    [Serializable]
    public class PlaceSearchResponse : ISerializable
    {
        protected PlaceSearchResponse(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            throw new NotImplementedException();
        }
    }
}
