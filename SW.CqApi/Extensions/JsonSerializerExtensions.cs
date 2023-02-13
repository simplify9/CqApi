using System;
using System.IO;
using Newtonsoft.Json;

namespace SW.CqApi.Extensions
{
    public static class JsonSerializerExtensions
    {
        public static object DeserializeObject(this JsonSerializer serializer, string data, Type target)
        {
            using StringReader sr = new StringReader(data);
            using JsonReader reader = new JsonTextReader(sr);
            return serializer.Deserialize(reader, target);
        }

        public static string SerializeObject(this JsonSerializer serializer, object obj)
        {
            using StringWriter sw = new StringWriter();
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, obj);
            return sw.ToString();
        }
    }
}