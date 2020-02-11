using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.CqApi.Utils
{
    public static class TypeUtils
    {
        private static OpenApiSchema GetPrimtiveSchema(this Type parameter)
        {
            OpenApiSchema schema = parameter.GetJsonType().GetOpenApiSchema();
            return schema;
        }

        public static OpenApiSchema ExplodeParameter(Type parameter, OpenApiComponents components)
        {
            OpenApiSchema schema = new OpenApiSchema();
            var jsonifed = parameter.GetJsonType();
            if (components.Schemas.ContainsKey(parameter.Name))
            {
                return components.Schemas[parameter.Name];
            }
            else if (parameter.IsEnum)
            {
                List<IOpenApiAny> enumVals = new List<IOpenApiAny>();
                foreach(var enumSingle in parameter.GetEnumNames())
                {
                    var enumStr = enumSingle.ToString();
                    enumVals.Add(new OpenApiString(enumStr));
                }
                schema.Type = "string";
                schema.Enum = enumVals;
            }
            else if(parameter.IsPrimitive || IsNumericType(parameter) || parameter == typeof(string))
            {
                schema.Type = jsonifed.Type.ToJsonType();
                schema.Example = GetExample(parameter);
            }
            else if (jsonifed.Items != null)
            {
                schema.Type = jsonifed.Type.ToJsonType();
                schema.Items = jsonifed.Items[0].GetOpenApiSchema();
                schema.Example = GetExample(parameter);
            }
            else if(parameter.GetProperties().Length != 0 && !IsNumericType(parameter))
            {
                Dictionary<string, OpenApiSchema> props = new Dictionary<string, OpenApiSchema>();
                foreach(var prop in parameter.GetProperties())
                {
                    if (prop.PropertyType == parameter) continue;
                    props[prop.Name] = ExplodeParameter(prop.PropertyType, components);
                }

                schema.Properties = props;
                components.Schemas[parameter.Name] = schema;
            }
            else
            {
                schema.Type = "undefined";
            }
            return schema;

        }

        public static bool IsNumericType(Type t )
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        static public IOpenApiAny GetExample(Type parameter)
        {
            if (parameter == typeof(string))
            {
                int randomNum = new Random().Next() % 3;
                var words = new string[] { "foo", "bar", "baz" };
                return new OpenApiString(words[randomNum]);
            }
            else if (IsNumericType(parameter))
            {
                int randomNum = new Random().Next() % 400;
                return new OpenApiInteger(randomNum);
            }
            else if (parameter == typeof(bool))
            {
                int randomNum = new Random().Next() % 1;
                return new OpenApiBoolean(randomNum == 0);
            }
            else if (parameter.GetInterfaces().Contains(typeof(IEnumerable)))
            {
                var exampleArr = new OpenApiArray();
                int randomNum = new Random().Next() % 3;
                for(int _ = 0; _ < randomNum + 1; _++)
                {
                    var innerType = parameter.GetElementType() ?? parameter.GenericTypeArguments[0];
                    exampleArr.Add(GetExample(innerType));
                }

                return exampleArr;
            }
            else
            {
                return new OpenApiNull();
            }

        }
    }
}
