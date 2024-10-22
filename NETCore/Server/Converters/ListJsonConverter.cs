using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juniper.Converters;

public class ListJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        if (typeToConvert.GetGenericTypeDefinition() != typeof(List<>))
        {
            return false;
        }

        var typeArguments = typeToConvert.GetGenericArguments();
        if (typeArguments.Length != 1)
        {
            return false;
        }

        return true;
    }

    public override JsonConverter CreateConverter(
        Type type,
        JsonSerializerOptions options)
    {
        var typeArguments = type.GetGenericArguments();
        var valueType = typeArguments[0];

        var converter = (JsonConverter)Activator.CreateInstance(
            typeof(ListConverterInner<>).MakeGenericType([valueType]),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new[] { options },
            culture: null
        )!;

        return converter;
    }

    private class ListConverterInner<TValue> :
        JsonConverter<List<TValue>>
    {
        private readonly JsonConverter<TValue> valueConverter;
        private readonly Type valueType;

        public ListConverterInner(JsonSerializerOptions options)
        {
            // For performance, use the existing converter.
            valueConverter = (JsonConverter<TValue>)options
                .GetConverter(typeof(TValue));

            // Cache the key and value types.
            valueType = typeof(TValue);
        }

        public override List<TValue> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            var list = new List<TValue>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return list;
                }

                var value = valueConverter.Read(ref reader, valueType, options)!;

                // Add to list.
                list.Add(value);
            }

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            List<TValue> list,
            JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var value in list)
            {
                valueConverter.Write(writer, value, options);
            }

            writer.WriteEndArray();
        }
    }
}