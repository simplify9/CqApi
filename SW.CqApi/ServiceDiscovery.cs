using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using SW.PrimitiveTypes;
using System;
using System.Collections;
using System.Collections.Generic;
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


        private IList<OpenApiParameter> GetOpenApiParameters(IEnumerable<ParameterInfo> parameters, OpenApiComponents components, bool withKey = false)
        {
            var openApiParams = new List<OpenApiParameter>();
            foreach(var parameter in parameters)
            {
                var schemaParam = ExplodeParameter(parameter.ParameterType, components);
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
                        Required = !parameter.IsOptional,
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

        private IOpenApiAny GetExample(Type parameter)
        {
            if (parameter == typeof(string))
            {
                var intfs = parameter.GetInterfaces();
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
                int randomNum = new Random().Next() % 4;
                for(int _ = 0; _ < randomNum; _++)
                {
                    var innerType = parameter.GetElementType() != null? 
                                    parameter.GetElementType() : parameter.GenericTypeArguments[0];
                    exampleArr.Add(GetExample(innerType));
                }

                return exampleArr;
            }
            else
            {
                return new OpenApiNull();
            }

        }

        private OpenApiSchema ExplodeParameter (Type parameter, OpenApiComponents components)
        {
            //strings are enumerable
            if(parameter.IsPrimitive || parameter.GetInterfaces().Contains(typeof(IEnumerable)))
            {
                return new OpenApiSchema
                {
                    Type = parameter.Name,
                    Example = GetExample(parameter)
                };
            }
            else
            {
                if (components.Schemas.ContainsKey(parameter.Name))
                    //TODO: Reference instead of copy
                    return components.Schemas[parameter.Name];
                else
                {
                    var paramSchemeDict = new Dictionary<string, OpenApiSchema>();
                    var props = parameter.GetProperties();
                    foreach (var prop in props)
                    {
                        paramSchemeDict[prop.Name] = ExplodeParameter(prop.PropertyType, components);
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

        private OpenApiRequestBody GetOpenApiRequestBody(MethodInfo methodInfo, OpenApiComponents components, bool withKey = false)
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
                paramterDict[param.Name] = ExplodeParameter(param.ParameterType, components);
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

        private OpenApiResponses GetOpenApiResponses(MethodInfo methodInfo)
        {
            var returnMediaType = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = methodInfo.ReturnType.FullName.Contains("Task") ? methodInfo.ReturnType.GenericTypeArguments[0].Name : methodInfo.ReturnType.FullName ,
                }
            };

            var responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "OK",
                    Content = {
                        ["application/json"] = returnMediaType
                    },
                    

                }
            };

            return responses;

        }

        public string GetOpenApiDocument()
        {

            var components = new OpenApiComponents();

            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Open API Document",
                },
                Servers = new List<OpenApiServer>
                {
                    new OpenApiServer { Url = "http://petstore.swagger.io/api" }
                },
                Paths = new OpenApiPaths(),
                Components = components

            };


            foreach (var res in resourceHandlers)
            {
                var pathItem = new OpenApiPathItem();
                document.Paths.Add(res.Key, pathItem);
                var tag = new OpenApiTag {
                    Name = res.Key,
                    Description = $"Commands and Queries related to {res.Key}"
                };
                document.Tags.Add(tag);
                pathItem.Operations = new Dictionary<OperationType, OpenApiOperation>();
                foreach (var handler in res.Value)
                {
                    var baseApiOperation = HandlerTypeMetadata.Handlers[handler.Value.NormalizedInterfaceType].OpenApiOperation;
                    var apiOperation = new OpenApiOperation()
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
                    apiOperation.Tags.Add(tag);
                    if (handler.Key == "get")
                    {
                        initializePath(document, res.Key);
                        apiOperation.Parameters = GetOpenApiParameters(handler.Value.Method.GetParameters(), components);
                        apiOperation.Responses = GetOpenApiResponses(handler.Value.Method);
                        document.Paths[res.Key].Operations.Add(OperationType.Get, apiOperation);
                    }

                    else if (handler.Key == "get/key")
                    {

                        initializePath(document, $"{res.Key}/{{key}}");
                        apiOperation.Parameters = GetOpenApiParameters(handler.Value.Method.GetParameters(), components, true);
                        apiOperation.Responses = GetOpenApiResponses(handler.Value.Method);
                        document.Paths[$"{res.Key}/{{key}}"].Operations.Add(OperationType.Get, apiOperation);
                    }

                    else if (handler.Key == "post")
                    {

                        initializePath(document, res.Key);
                        apiOperation.RequestBody = GetOpenApiRequestBody(handler.Value.Method, components);
                        apiOperation.Responses = GetOpenApiResponses(handler.Value.Method);
                        document.Paths[res.Key].Operations.Add(OperationType.Post, apiOperation);
                    }

                    else if (handler.Key == "post/key")
                    {

                        initializePath(document, $"{res.Key}/{{key}}");
                        apiOperation.Parameters = GetOpenApiParameters(handler.Value.Method.GetParameters().Take(1), components, true);
                        apiOperation.RequestBody = GetOpenApiRequestBody(handler.Value.Method, components, true);
                        apiOperation.Responses = GetOpenApiResponses(handler.Value.Method);
                        document.Paths[$"{res.Key}/{{key}}"].Operations.Add(OperationType.Post, apiOperation);
                    }

                    else if (handler.Key.StartsWith("post/key"))
                    {
                        var path = $"{res.Key}/{{key}}{handler.Key.Substring(handler.Key.LastIndexOf('/'))}";
                        initializePath(document, path);
                        apiOperation.Parameters = GetOpenApiParameters(handler.Value.Method.GetParameters().Take(1), components, true);
                        apiOperation.RequestBody = GetOpenApiRequestBody(handler.Value.Method, components, true);
                        apiOperation.Responses = GetOpenApiResponses(handler.Value.Method);
                        document.Paths[path].Operations.Add(OperationType.Post, apiOperation);
                    }


                    else if (handler.Key.StartsWith("post/"))
                    {

                        var path = $"{res.Key}{handler.Key.Substring(handler.Key.LastIndexOf('/'))}";
                        initializePath(document, path);
                        apiOperation.RequestBody = GetOpenApiRequestBody(handler.Value.Method, components, true);
                        apiOperation.Responses = GetOpenApiResponses(handler.Value.Method);
                        document.Paths[path].Operations.Add(OperationType.Post, apiOperation);
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
