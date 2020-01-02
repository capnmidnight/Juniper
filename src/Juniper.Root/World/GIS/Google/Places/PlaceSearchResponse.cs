using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.Places
{
    [Serializable]
    public class PlaceSearchResponse : ISerializable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
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
