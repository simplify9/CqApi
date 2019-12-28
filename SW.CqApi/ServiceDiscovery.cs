using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using SW.PrimitiveTypes;
using System;
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

        //private readonly IDictionary<Type, IDictionary<string, Type>> serviceCatalog = new Dictionary<Type, IDictionary<string, Type>>();
        //private readonly IDictionary<string, ICollection<Type>> modelServices = new Dictionary<string, ICollection<Type>>(StringComparer.OrdinalIgnoreCase);
        //private readonly IDictionary<string, Type> models = new Dictionary<string, Type>();

        public ServiceDiscovery(IServiceProvider serviceProvider, ILogger<ServiceDiscovery> logger)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var registeredHandlers = scope.ServiceProvider.GetServices<IHandler>();

                foreach (var svc in registeredHandlers)
                {
                    var serviceType = svc.GetType();
                    var interfaceType = serviceType.GetTypeInfo().ImplementedInterfaces.Where(i => typeof(IHandler).IsAssignableFrom(i) && i != typeof(IHandler)).Single();

                    var typeNameArray = serviceType.FullName.Split('.');
                    var resourceName = typeNameArray[typeNameArray.Length - 2].ToLower();

                    if (!resourceHandlers.ContainsKey(resourceName))
                        resourceHandlers.Add(resourceName, new Dictionary<string, HandlerInfo>(StringComparer.OrdinalIgnoreCase));

                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IGetHandler<>))

                        resourceHandlers[resourceName]["get/key"] = new HandlerInfo
                        {
                            HandlerType = serviceType,
                            Method = interfaceType.GetMethod("Handle"),
                            ArgumentTypes = interfaceType.GetMethod("Handle").GetParameters().Select(p => p.ParameterType).ToList()
                        };

                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                    {
                        var handlerNameAttribute = serviceType.GetCustomAttribute<HandlerNameAttribute>();
                        var handlerKey = handlerNameAttribute == null ? "post" : $"post/{handlerNameAttribute.Name.ToLower()}";
                        resourceHandlers[resourceName][handlerKey] = new HandlerInfo
                        {
                            HandlerType = serviceType,
                            Method = interfaceType.GetMethod("Handle"),
                            ArgumentTypes = interfaceType.GetMethod("Handle").GetParameters().Select(p => p.ParameterType).ToList()
                        };
                    }

                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                    {
                        var handlerNameAttribute = serviceType.GetCustomAttribute<HandlerNameAttribute>();
                        var handlerKey = handlerNameAttribute == null ? "put/key" : $"post/key/{handlerNameAttribute.Name.ToLower()}";
                        resourceHandlers[resourceName][handlerKey] = new HandlerInfo
                        {
                            HandlerType = serviceType,
                            Method = interfaceType.GetMethod("Handle"),
                            ArgumentTypes = interfaceType.GetMethod("Handle").GetParameters().Select(p => p.ParameterType).ToList()
                        };
                    }

                    if (interfaceType == typeof(ISearchHandler))

                        resourceHandlers[resourceName]["get"] = new HandlerInfo
                        {
                            HandlerType = serviceType,
                            Method = interfaceType.GetMethod("Handle"),
                            ArgumentTypes = interfaceType.GetMethod("Handle").GetParameters().Select(p => p.ParameterType).ToList()
                        };


                }
            }
        }


        //public Type GetModelType(string modelName)
        //{
        //    return models[modelName];
        //}


        //public IDictionary<string, IEnumerable<string>> ListModels()
        //{
        //    return modelServices.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(t => t.Name));
        //}

        public HandlerInfo ResolveHandler(string resourceName, string handlerKey)
        {
            try
            {
                return resourceHandlers[resourceName][handlerKey];
            }
            catch
            {
                throw new SWException($"Could not resolve {handlerKey} of resource {resourceName}.");
            }
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
                    if (handler.Key == "get")
                    {

                        initializePath(document, res.Key);

                        document.Paths[res.Key].Operations.Add(OperationType.Get, new OpenApiOperation
                        {
                            Parameters = new List<OpenApiParameter>
                            {
                                new OpenApiParameter
                                {
                                    Name = "size",
                                    AllowEmptyValue = true,
                                    In = ParameterLocation.Query,

                                    Schema = new OpenApiSchema
                                    {
                                        Type = "integer",
                                    }
                                }
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

                    else if (handler.Key == "get/key")
                    {

                        initializePath(document, $"{res.Key}/{{key}}");

                        document.Paths[$"{res.Key}/{{key}}"].Operations.Add(OperationType.Get, new OpenApiOperation
                        {


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


                    else if (handler.Key == "post")
                    {

                        initializePath(document, res.Key);

                        document.Paths[res.Key].Operations.Add(OperationType.Post, new OpenApiOperation
                        {

                            RequestBody = new OpenApiRequestBody
                            {
                                Required = true,
                                Description = "Command body",
                                Reference  = new OpenApiReference
                                {
                                    Type = ReferenceType.RequestBody,
                                    
                                } 
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

                    else if (handler.Key == "put/key")
                    {

                        initializePath(document, $"{res.Key}/{{key}}");

                        document.Paths[$"{res.Key}/{{key}}"].Operations.Add(OperationType.Put, new OpenApiOperation
                        {


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

                    else if (handler.Key.StartsWith("post/key"))
                    {
                        var path = $"{res.Key}/{{key}}{handler.Key.Substring(handler.Key.LastIndexOf('/'))}";

                        initializePath(document, path);

                        document.Paths[path].Operations.Add(OperationType.Post, new OpenApiOperation
                        {


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
