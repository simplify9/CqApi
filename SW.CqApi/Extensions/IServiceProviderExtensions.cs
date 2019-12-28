using Microsoft.Extensions.DependencyInjection;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SW.CqApi
{
    internal static class IServiceProviderExtensions
    {

        //private class DefaultRequestContext : IRequestContext
        //{
        //    public ClaimsPrincipal User => new ClaimsPrincipal();
        //    public IReadOnlyCollection<RequestValue> Values => new List<RequestValue>();
        //    public bool IsValid => true;
        //}

        public static IRequestContext GetRequestContext(this IServiceProvider serviceProvider)
        {
            var requestContext = serviceProvider.GetServices<IRequestContext>().Where(rc => rc.IsValid).SingleOrDefault();

            //if (requestContext == null) 
            //    return new DefaultRequestContext();

            return requestContext; 
        }

        public static HandlerInfo ResolveHandler(this IServiceProvider serviceProvider, string resourceName, string handlerKey)
        {
            var sd = serviceProvider.GetService<ServiceDiscovery>();

            if (sd is null)

                throw new SWException("Could not find required service. services.AddMapi() seems missing from startup.");

            var handlerInfo = sd.ResolveHandler(resourceName, handlerKey);

            handlerInfo.Instance = serviceProvider.GetService(handlerInfo.HandlerType);

            if (handlerInfo.Instance is null) 
                
                throw new SWException($"Could not find required service {handlerKey} for resource {resourceName}.");

            //var svcType = svc.GetType();

            if (handlerInfo.HandlerType.GetCustomAttributes(typeof(ProtectAttribute), false).FirstOrDefault() is ProtectAttribute protectAttribute)
            {
                var requestContext = serviceProvider.GetRequestContext();

                if (requestContext is null)

                    throw new SWException("Could not find a valid request context service.");

                if (!requestContext.User.Identity.IsAuthenticated)

                    throw new CqApiUnauthorizedException();
                
                if (protectAttribute.RequireRole)
                {
                    var requiredRoles = new string[] 
                    { 
                        //$"mapi.{svcType.FullName}.{methodName}", 
                        $"mapi.{handlerInfo.HandlerType.FullName}.*" 
                    };

                    if (!requestContext.User.Claims.Any(c => c.Subject.RoleClaimType  == ClaimTypes.Role && requiredRoles.Contains(c.Value, StringComparer.OrdinalIgnoreCase)))

                        throw new CqApiForbidException();
                }
            }

            return handlerInfo;
            //{
            //    Method = svcType.GetMethod(methodName),
            //    Instance = svc,
            //    ModelType = modelType

            //};
        }

        //public static IDictionary<string, IEnumerable<string>> ListModels(this IServiceProvider serviceProvider)
        //{
        //    var sd = serviceProvider.GetRequiredService<ServiceDiscovery>();
        //    return sd.ListModels();
        //}
    }
}
