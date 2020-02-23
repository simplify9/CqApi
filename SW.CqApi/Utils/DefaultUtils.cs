using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.CqApi.Utils
{
    internal static class DefaultUtils
    {
        public static OpenApiResponses GetDefaultResponses(this OpenApiResponses baseResponses, MethodInfo info, string InterfaceType)
        {
            var parameters = info.GetParameters();
            bool searchy = info.GetParameters().Any(p => p.ParameterType == typeof(SearchyRequest));
            if (searchy)
            {
                var openMediaType = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "Dictionary<string, string>",
                        Example = new OpenApiString("foo, baz")
                    }
                };
                baseResponses.Add("206", new OpenApiResponse
                {
                    Description = "Lookup result",
                    Content =
                    {
                        ["application/json"] = openMediaType
                    }
                });
            }

            bool commandHandler = InterfaceType.Contains("CommandHandler"); 
            if(commandHandler)
            {
                baseResponses.Add("204", new OpenApiResponse
                {
                    Description = "No Content",
                });
            }

            bool queryHandler = InterfaceType.Contains("IQueryHandler") || InterfaceType.Contains("GetHandler"); 
            if(queryHandler)
            {
                baseResponses.Add("404", new OpenApiResponse
                {
                    Description = "Not found",
                });
            }
            return baseResponses;
        }
    }
}
