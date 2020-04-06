using Microsoft.OpenApi.Models;
using SW.CqApi.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi.Extensions
{
    public static class TypeExtensions
    {
        private static OpenApiSchema GetPrimtiveSchema(this Type parameter)
        {
            OpenApiSchema schema = parameter.GetJsonType().GetOpenApiSchema();
            return schema;
        }

        public static string GetGenericName(this Type genType)
        {
            string[] split = genType.Name.Split('`');
            string name = $"{split[0]}<";
            for(byte i = 0; i < genType.GenericTypeArguments.Length; i++)
            {
                name += (
                    genType.GenericTypeArguments[i].GenericTypeArguments.Length > 0? 
                    genType.GenericTypeArguments[i].GetGenericName() : 
                    genType.GenericTypeArguments[i].Name
                );

                if(i == genType.GenericTypeArguments.Length - 1) name += ">";
                else name += ",";
            }
            return name;
        }
    }
}
