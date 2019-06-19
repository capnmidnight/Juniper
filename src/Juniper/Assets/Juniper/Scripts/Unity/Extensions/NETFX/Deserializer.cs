using Juniper.Serialization;

using Newtonsoft.Json;

namespace Juniper.Json
{
    public class Deserializer : IDeserializer
    {
        public bool TryDeserialize<T>(string text, out T value)
        {
            try
            {
                value = JsonConvert.DeserializeObject<T>(text);
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
