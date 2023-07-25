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
        public static OpenApiSchema GetDefaultSchema(this Type type)
        {
            var schema = new OpenApiSchema();
            var stringSchema = typeof(string).GetJsonType().GetOpenApiSchema();

            if (type == typeof(DateTime))
            {
                schema.Title = "DateTime";
                schema.Type = "string";
            }
            if(type == typeof(SearchyRequest))
            {
                schema.Title = "Searchy Request";
                schema.Properties.Add("filter", new OpenApiSchema {
                    Type = "string",
                    Nullable = true,
                    Title = "filter",
                });
                schema.Properties.Add("sort", new OpenApiSchema
                {
                    Type = "string",
                    Nullable = true,
                    Title = "sort"
                });
                schema.Properties.Add("size", new OpenApiSchema { 
                    Type = "string",
                    Nullable = true,
                    Title = "sort"
                });
                schema.Properties.Add("page", new OpenApiSchema { 
                    Type = "string",
                    Nullable = true,
                    Title = "page"
                });
                schema.Properties.Add("count", new OpenApiSchema
                {
                    Type = "string",
                    Nullable = true,
                    Title = "count"
                });
            }

            return schema;
        }
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
            
            // if (!baseResponses.ContainsKey("200"))
            // {
            //     baseResponses.Add("200", new OpenApiResponse
            //     {
            //         Description = "Successful",
            //         
            //         Content =
            //         {
            //             ["application/json"] = new OpenApiMediaType
            //             {
            //                 
            //                 Schema =  new OpenApiSchema
            //                 {
            //                     
            //                     Type = info.ReturnType.FullName.Contains("Task")
            //                         ? info.ReturnType.GenericTypeArguments[0].FullName
            //                         : info.ReturnType.FullName,
            //                 }
            //             }
            //         }
            //     });
            // }
            
            return baseResponses;
        }
    }
}
