using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using SW.CqApi.Utils;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        //private readonly ILogger<ServiceDiscovery> logger;
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

        public IEnumerable<string> GetRoles()
        {
            return resourceHandlers.GetRoles();
        }

        public string GetOpenApiDocument()
        {

            var desc = $"This API includes ways to manipulate ";
            var keysArr = resourceHandlers.Keys.ToArray<string>();
            for (byte i = 0; i < keysArr.Length; i++)
                desc += i + 1 == keysArr.Length ? $"{keysArr[i]}.\n" :
                        i + 1 == keysArr.Length - 1 ? $"{keysArr[i]} and " :
                        $"{keysArr[i]},";

            var components = new OpenApiComponents();
            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Open API Document",
                    Description = desc
                },
                Servers = new List<OpenApiServer>
                {
                    //new OpenApiServer { Url = "http://petstore.swagger.io/api" }
                },
                
                Paths = new OpenApiPaths(),
                Components = components,
            };
            var roles = resourceHandlers.GetRoles();
            document.Components = components.AddSecurity(roles);
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



                    var interfaceType = handler.Value.NormalizedInterfaceType;
                    var apiOperation = HandlerTypeMetadata.Handlers[interfaceType].OpenApiOperation.Clone();
                    apiOperation.Tags.Add(tag);
                    var returns = handler.Value.HandlerType.GetCustomAttributes<ReturnsAttribute>();
                    apiOperation.Responses = OpenApiUtils.GetOpenApiResponses(handler.Value.Method, returns, components, interfaceType.Name);

                    if (roles.Contains($"{res.Key}.{handler.Value.HandlerType.Name.ToLower()}")){
                        apiOperation.AddSecurity($"{res.Key}.{handler.Value.HandlerType.Name.ToLower()}", components);
                    }

                    if (handler.Key == "get")
                    {
                        initializePath(document, res.Key);
                        apiOperation.Parameters = OpenApiUtils.GetOpenApiParameters(handler.Value.Method.GetParameters(), components);
                        document.Paths[res.Key].Operations.Add(OperationType.Get, apiOperation);
                    }

                    else if (handler.Key == "get/key")
                    {

                        initializePath(document, $"{res.Key}/{{key}}");
                        apiOperation.Parameters = OpenApiUtils.GetOpenApiParameters(handler.Value.Method.GetParameters(), components, true);
                        document.Paths[$"{res.Key}/{{key}}"].Operations.Add(OperationType.Get, apiOperation);
                    }

                    else if (handler.Key == "post")
                    {

                        initializePath(document, res.Key);
                        apiOperation.RequestBody = OpenApiUtils.GetOpenApiRequestBody(handler.Value.Method, components);
                        document.Paths[res.Key].Operations.Add(OperationType.Post, apiOperation);
                    }

                    else if (handler.Key == "post/key")
                    {

                        initializePath(document, $"{res.Key}/{{key}}");
                        apiOperation.Parameters = OpenApiUtils.GetOpenApiParameters(handler.Value.Method.GetParameters().Take(1), components, true);
                        apiOperation.RequestBody = OpenApiUtils.GetOpenApiRequestBody(handler.Value.Method, components, true);
                        document.Paths[$"{res.Key}/{{key}}"].Operations.Add(OperationType.Post, apiOperation);
                    }

                    else if (handler.Key.StartsWith("post/key"))
                    {
                        var path = $"{res.Key}/{{key}}{handler.Key.Substring(handler.Key.LastIndexOf('/'))}";
                        initializePath(document, path);
                        apiOperation.Parameters = OpenApiUtils.GetOpenApiParameters(handler.Value.Method.GetParameters().Take(1), components, true);
                        apiOperation.RequestBody = OpenApiUtils.GetOpenApiRequestBody(handler.Value.Method, components, true);
                        document.Paths[path].Operations.Add(OperationType.Post, apiOperation);
                    }


                    else if (handler.Key.StartsWith("post/"))
                    {

                        var path = $"{res.Key}{handler.Key.Substring(handler.Key.LastIndexOf('/'))}";
                        initializePath(document, path);
                        apiOperation.RequestBody = OpenApiUtils.GetOpenApiRequestBody(handler.Value.Method, components, true);
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
