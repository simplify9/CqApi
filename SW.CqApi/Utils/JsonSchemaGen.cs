using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;

namespace SW.CqApi.Utils
{
    public static class JsonSchemaGen
    {
        public static JsonSchema GetJsonType(this Type t)
        {
            JsonSchemaGenerator gen = new JsonSchemaGenerator();
            JsonSchema json = gen.Generate(t);
            return json;
        }
        public static OpenApiSchema GetOpenApiSchema(this JsonSchema json )
        {
            OpenApiSchema op = new OpenApiSchema();
            if(json.Items != null)
                op.Items = json.Items[0].GetOpenApiSchema();

            if(json.Properties != null)
            {
                var props = new Dictionary<string, OpenApiSchema>();
                foreach(var prop in json.Properties)
                    props[prop.Key] = prop.Value.GetOpenApiSchema();
                op.Properties = props;
            }

            string jsonType = json.Type.ToJsonType();

            op.Type = jsonType;
            return op;
        }

        public static string ToJsonType(this JsonSchemaType? jsonSchemaType)
        {
            if (jsonSchemaType == null) return "";
            string jstString = jsonSchemaType.ToString();
            string splitJson = jstString.Split(',').Length > 1 ? jstString.Split(',')[0] : jstString;
            switch (splitJson)
            {
                case "Integer":
                case "Decimal":
                    return "number";
                case "String":
                    return "string";
                case "Object":
                    return "object";
                default:
                    return splitJson;
            }
        }
    }
}
