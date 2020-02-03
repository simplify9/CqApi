using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.CqApi.Utils
{
    public class TypeUtils
    {

        public static OpenApiSchema ExplodeParameter (Type parameter, OpenApiComponents components)
        {
            bool isEnumurableType = parameter.GetInterfaces().Contains(typeof(IEnumerable));
            bool isSimpleEnum = parameter == typeof(string);
            var type = parameter;
            if (isEnumurableType && !isSimpleEnum)
            {
                type = parameter.GetElementType() ?? parameter.GenericTypeArguments[0];
                isSimpleEnum = type.IsPrimitive || type == typeof(string) || type == typeof(object) || type == typeof(decimal);
            }

            if (parameter.IsPrimitive || isSimpleEnum || parameter == typeof(decimal) || parameter == typeof(object))
            {
                return new OpenApiSchema
                {
                    Type = parameter.Name,
                    Example = GetExample(parameter)
                };
            }
            else
            {
                if (components.Schemas.ContainsKey(parameter.Name)) return components.Schemas[parameter.Name];
                if (parameter.Name.Contains("Nullable")) return ExplodeParameter(parameter.GenericTypeArguments[0], components);

                else
                {
                    var paramSchemeDict = new Dictionary<string, OpenApiSchema>();
                    if (isEnumurableType)
                        return ExplodeParameter(type, components);
                    
                    if (type.IsPrimitive) return ExplodeParameter(type, components);

                    if (type.IsEnum)
                    {
                        var values = type.GetEnumNames();
                        var openApiValues = new List<IOpenApiAny>();
                        foreach(var value in values)
                            openApiValues.Add(new OpenApiString(value));

                        return new OpenApiSchema
                        {
                            Type = "String",
                            Enum = openApiValues
                        };
                    }
                    else
                    {
                        var props = type.GetProperties();
                        foreach (var prop in props)
                        {
                            if (prop.PropertyType == parameter) continue;
                            paramSchemeDict[prop.Name] = ExplodeParameter(prop.PropertyType, components);
                        }
                    }

                    var schema = new OpenApiSchema
                    {
                        Title = parameter.Name,
                        Properties = paramSchemeDict
                    };
                    components.Schemas[parameter.Name] = schema;
                    return schema;
                }

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
            else if (parameter == typeof(int) || parameter.IsAssignableFrom(typeof(int)))
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
