using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.CqApi
{
    internal class ServiceDiscovery
    {

        /* Resource Abc
         *  get / => search     "get"
         *  get /key => get     "get/key"
         *  
         *  post /              "post"
         *  post /command1      "post/{token}"
         *  post /command2      
         *  post /key/command   "post/key/{token}"
         *  
         *  put /key            "put/key"
         *  
         *  delete /key         "delete/key"
         * 
         */




        private readonly ILogger<ServiceDiscovery> logger;
        private readonly IDictionary<string, IDictionary<string, HandlerInfo>> resourceHandlers = new Dictionary<string, IDictionary<string, HandlerInfo>>(StringComparer.OrdinalIgnoreCase);

        public ServiceDiscovery(IServiceProvider serviceProvider, ILogger<ServiceDiscovery> logger)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var registeredHandlers = scope.ServiceProvider.GetServices<IHandler>();

                foreach (var svc in registeredHandlers)
                {
                    var serviceType = svc.GetType();
                    var interfaceType = serviceType.GetTypeInfo().ImplementedInterfaces.Where(i => typeof(IHandler).IsAssignableFrom(i) && i != typeof(IHandler)).Single();
                    var interfaceTypeNormalized = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;

                    var typeNameArray = serviceType.FullName.Split('.');
                    var resourceName = typeNameArray[typeNameArray.Length - 2].ToLower();

                    var handlerNameAttribute = serviceType.GetCustomAttribute<HandlerNameAttribute>();
                    var handlerName = handlerNameAttribute == null ? "" : $"/{handlerNameAttribute.Name.ToLower()}";

                    if (!resourceHandlers.ContainsKey(resourceName))
                        resourceHandlers.Add(resourceName, new Dictionary<string, HandlerInfo>(StringComparer.OrdinalIgnoreCase));

                    var handlerKey = $"{HandlerTypeMetadata.Handlers[interfaceTypeNormalized].Key}{handlerName}";

                    resourceHandlers[resourceName][handlerKey] = new HandlerInfo
                    {
                        HandlerType = serviceType,
                        Method = interfaceType.GetMethod("Handle"),
                        ArgumentTypes = interfaceType.GetMethod("Handle").GetParameters().Select(p => p.ParameterType).ToList(),
                        Key = handlerKey,
                        Resource = resourceName,
                        NormalizedInterfaceType = interfaceTypeNormalized
                    };

                }
            }
        }

        public bool TryResolveHandler(string resourceName, string handlerKey, out HandlerInfo handlerInfo)
        {
            if (resourceHandlers.TryGetValue(resourceName, out var handlers))

                if (handlers.TryGetValue(handlerKey, out handlerInfo))
                    return true;

            handlerInfo = null;
            return false;

        }

        public HandlerInfo ResolveHandler(string resourceName, string handlerKey)
        {

            if (TryResolveHandler(resourceName, handlerKey, out var handlerInfo))
                return handlerInfo;

            throw new SWException($"Could not resolve {handlerKey} of resource {resourceName}.");

        }


        private IList<OpenApiParameter> GetOpenApiParameters(IList<ParameterInfo> parameters, bool withKey = false)
        {
            var openApiParams = new List<OpenApiParameter>();
            foreach(var parameter in parameters)
            {
                var openApiParam = new OpenApiParameter();
                openApiParam.Name = parameter.Name;
                openApiParam.Required = !parameter.IsOptional;
                openApiParam.AllowEmptyValue = parameter.IsOptional;
                openApiParam.In = withKey? ParameterLocation.Path : ParameterLocation.Query;
                openApiParam.Schema = new OpenApiSchema
                {
                    Type = parameter.ParameterType.FullName
                };
                openApiParams.Add(openApiParam);
            }
            return openApiParams;
        }

        private object ExplodeParameter (Type parameter)
        {
            if (parameter.IsPrimitive || parameter == typeof(string))
                return parameter.Name;


            else
            {
                var paramDict = new Dictionary<string, object>();
                var props = parameter.GetProperties();
                foreach (var prop in props)
                {
                    paramDict[prop.Name] = ExplodeParameter(prop.PropertyType);
                }
                return paramDict;
            }
        }

        private OpenApiRequestBody GetOpenApiRequestBody(MethodInfo methodInfo)
        {

            var conentMediaType  = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = methodInfo.ReturnType.FullName.Contains("Task") ? 
                        methodInfo.ReturnType.GenericTypeArguments[0].Name : methodInfo.ReturnType.FullName 
                }
            };
            var paramterDict = new Dictionary<string, object>();

            foreach(var param in methodInfo.GetParameters())
            {
                paramterDict[param.Name] = ExplodeParameter(param.ParameterType);
            }
            


            var requestBody = new OpenApiRequestBody
            {
                Description = "Command Body",
                Required = methodInfo.GetParameters().Any(p => !p.IsOptional),
                Content =
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Example = new OpenApiString(Newtonsoft.Json.JsonConvert.SerializeObject(paramterDict))
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

        private OpenApiResponses GetOpenApiResponses(MethodInfo methodInfo)
        {
            var returnMediaType = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = methodInfo.ReturnType.FullName.Contains("Task") ? methodInfo.ReturnType.GenericTypeArguments[0].Name : methodInfo.ReturnType.FullName 
                }
            };

            var responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "OK",
                    Content = {
                        ["application/json"] = returnMediaType
                    }

                }
            };

            return responses;

        }

        public string GetOpenApiDocument()
        {
            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Swagger Petstore (Simple)",
                },
                Servers = new List<OpenApiServer>
                {
                    new OpenApiServer { Url = "http://petstore.swagger.io/api" }
                },
                Paths = new OpenApiPaths(),


            };


            foreach (var res in resourceHandlers)
            {
                var pathItem = new OpenApiPathItem();

                document.Paths.Add(res.Key, pathItem);
                pathItem.Operations = new Dictionary<OperationType, OpenApiOperation>();
                foreach (var handler in res.Value)
                {
                    var baseApiOperation = HandlerTypeMetadata.Handlers[handler.Value.NormalizedInterfaceType].OpenApiOperation;
                    if (handler.Key == "get")
                    {
                        initializePath(document, res.Key);
                        baseApiOperation.Parameters = GetOpenApiParameters(handler.Value.Method.GetParameters());
                        baseApiOperation.Responses = GetOpenApiResponses(handler.Value.Method);
                        document.Paths[res.Key].Operations.Add(OperationType.Get, baseApiOperation);
                    }

                    else if (handler.Key == "get/key")
                    {

                        initializePath(document, $"{res.Key}/{{key}}");
                        baseApiOperation.Parameters = GetOpenApiParameters(handler.Value.Method.GetParameters(), true);
                        baseApiOperation.Responses = GetOpenApiResponses(handler.Value.Method);
                        document.Paths[$"{res.Key}/{{key}}"].Operations.Add(OperationType.Get, baseApiOperation);
                    }

                    else if (handler.Key == "post")
                    {

                        initializePath(document, res.Key);
                        baseApiOperation.RequestBody = GetOpenApiRequestBody(handler.Value.Method);
                        baseApiOperation.Responses = GetOpenApiResponses(handler.Value.Method);
                        document.Paths[res.Key].Operations.Add(OperationType.Post, baseApiOperation);
                    }

                    else if (handler.Key == "post/key")
                    {

                        initializePath(document, $"{res.Key}/{{key}}");
                        var openApiOp = HandlerTypeMetadata.Handlers[handler.Value.NormalizedInterfaceType].OpenApiOperation;

                        document.Paths[$"{res.Key}/{{key}}"].Operations.Add(OperationType.Post, openApiOp);
                    }

                    else if (handler.Key.StartsWith("post/key"))
                    {
                        var path = $"{res.Key}/{{key}}{handler.Key.Substring(handler.Key.LastIndexOf('/'))}";

                        initializePath(document, path);

                        document.Paths[path].Operations.Add(OperationType.Post, new OpenApiOperation
                        {

                            RequestBody = new OpenApiRequestBody
                            {
                                
                            },
                            Description = "Returns all pets from the system that the user has access to",
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "OK",

                                }
                            }
                        });
                    }


                    else if (handler.Key.StartsWith("post/"))
                    {

                        var path = $"{res.Key}{handler.Key.Substring(handler.Key.LastIndexOf('/'))}";

                        initializePath(document, path);

                        document.Paths[path].Operations.Add(OperationType.Post, new OpenApiOperation
                        {

                            RequestBody = new OpenApiRequestBody
                            {
                                Description = "Command",
                                Required = true
                            },
                            Description = "Returns all pets from the system that the user has access to",
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "OK",

                                }
                            }
                        });
                    }


                }
            }

            return document.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);
        }

        void initializePath(OpenApiDocument document, string path)
        {
            if (!document.Paths.ContainsKey(path))

                document.Paths.Add(path, new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>()
                });
        }

    }
}
