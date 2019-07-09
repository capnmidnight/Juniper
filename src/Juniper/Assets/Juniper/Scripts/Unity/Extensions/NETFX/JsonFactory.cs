using Juniper.Serialization;

using Newtonsoft.Json;

namespace Juniper.Json
{
    public class JsonFactory : IDeserializer, ISerializer
    {
        public string Serialize<T>(string name, T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public bool TryDeserialize<T>(string text, out T value)
        {
            try
            {
                value = Deserialize<T>(text);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }
    }
}
