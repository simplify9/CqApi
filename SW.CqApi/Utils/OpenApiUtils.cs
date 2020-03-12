using Microsoft.OpenApi.Models;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.CqApi.Utils
{
    internal static class OpenApiUtils
    {
        public static OpenApiOperation Clone(this OpenApiOperation baseApiOperation)
        {
            return new OpenApiOperation
            {
                Deprecated = baseApiOperation.Deprecated,
                Description = baseApiOperation.Description,
                Callbacks = baseApiOperation.Callbacks,
                Extensions = baseApiOperation.Extensions,
                Responses = baseApiOperation.Responses,
                RequestBody = baseApiOperation.RequestBody,
                OperationId = baseApiOperation.OperationId,
                ExternalDocs = baseApiOperation.ExternalDocs,
                Parameters = baseApiOperation.Parameters,
                Security = baseApiOperation.Security,
                Servers = baseApiOperation.Servers,
                Summary = baseApiOperation.Summary
            };
        }
        public static OpenApiResponses GetOpenApiResponses(MethodInfo methodInfo, IEnumerable<ReturnsAttribute> returnsAttributes, OpenApiComponents components, string InterfaceType)
        {
            //var methodInfo = handlerInfo.Method;
            var returnMediaType = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = methodInfo.ReturnType.FullName.Contains("Task") ? methodInfo.ReturnType.GenericTypeArguments[0].Name : methodInfo.ReturnType.FullName ,
                }
            };

            var responses = returnsAttributes
                .ToOpenApiResponses(components)
                .GetDefaultResponses(methodInfo, InterfaceType);
            return responses;
        }

        public static IList<OpenApiParameter> GetOpenApiParameters(IEnumerable<ParameterInfo> parameters, OpenApiComponents components, bool withKey = false)
        {
            var openApiParams = new List<OpenApiParameter>();
            foreach(var parameter in parameters)
            {
                var schemaParam = TypeUtils.ExplodeParameter(parameter.ParameterType, components);
                if (schemaParam.Properties.Count > 0)
                {
                    foreach(var prop in schemaParam.Properties.Values)
                    {
                        openApiParams.Add(new OpenApiParameter
                        {
                            Name = prop.Title,
                            Required = !prop.Nullable,
                            AllowEmptyValue = !prop.Nullable,
                            In = withKey ? ParameterLocation.Path : ParameterLocation.Query,
                            Schema = new OpenApiSchema
                            {
                                Type = parameter.ParameterType.FullName
                            }
                        });
                    }
                }
                else
                {
                    openApiParams.Add(new OpenApiParameter
                    {
                        Name = parameter.Name,
                        //Required = !parameter.IsOptional,
                        AllowEmptyValue = parameter.IsOptional,
                        In = withKey ? ParameterLocation.Path : ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = parameter.ParameterType.FullName
                        }
                    });
                }
            }
            return openApiParams;
        }


        public static OpenApiRequestBody GetOpenApiRequestBody(MethodInfo methodInfo, OpenApiComponents components, bool withKey = false)
        {

            var conentMediaType  = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = methodInfo.ReturnType.FullName.Contains("Task") ? 
                        methodInfo.ReturnType.GenericTypeArguments[0].Name : methodInfo.ReturnType.FullName 
                }
            };
            var relevantParameters = withKey ? methodInfo.GetParameters().Skip(1): methodInfo.GetParameters();
            var paramterDict = new Dictionary<string, OpenApiSchema>();


            foreach(var param in relevantParameters)
            {
                paramterDict[param.Name] = TypeUtils.ExplodeParameter(param.ParameterType, components);
            }
            
            var requestBody = new OpenApiRequestBody
            {
                Description = "Command Body",
                Required = relevantParameters.Any(p => !p.IsOptional),
                Content =
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Title = "Body",
                            Properties = paramterDict,
                        }
                    }
                },
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.RequestBody,
                }
            };

            return requestBody;
        }
    }
}
