using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Price : ISerializable
    {
        public readonly string vat;
        public readonly string price_exvat;
        public readonly string price;
        public readonly bool is_free;

        public Price(SerializationInfo info, StreamingContext context)
        {
            vat = info.GetString(nameof(vat));
            price_exvat = info.GetString(nameof(price_exvat));
            price = info.GetString(nameof(price));
            is_free = info.GetBoolean(nameof(is_free));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(vat), vat);
            info.AddValue(nameof(price_exvat), price_exvat);
            info.AddValue(nameof(price), price);
            info.AddValue(nameof(is_free), is_free);
        }
    }
}
