using Newtonsoft.Json;
using System;

namespace SW.CqApi
{
    public class TimeSpanJsonConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan) || objectType == typeof(TimeSpan?);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var stc = reader.Value;
            if (stc == null)
            {
                if (objectType == typeof(TimeSpan))
                    return default(TimeSpan);
                else
                    return null;

            }

            if (TimeSpan.TryParse(stc.ToString(), out var result))
            {
                return result;
            }

            throw new JsonSerializationException($"can not deserialize value {stc.ToString()} to TimeSpan");


        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string toWrite = null;

            if (value != null)
                toWrite = ((TimeSpan)value).ToString();

            writer.WriteValue(toWrite);





        }
    }
}
