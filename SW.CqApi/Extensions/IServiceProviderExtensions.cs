using Microsoft.Extensions.DependencyInjection;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace SW.CqApi
{
    internal static class IServiceProviderExtensions
    {
        static IRequestContext GetRequestContext(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetServices<IRequestContext>().Where(rc => rc.IsValid).SingleOrDefault();
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

            if (handlerInfo.HandlerType.GetCustomAttribute<ProtectAttribute>() is ProtectAttribute protectAttribute)
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
                        $"{resourceName}.{handlerInfo.HandlerType.Name}", 
                        $"{resourceName}.*" 
                    };

                    if (!requestContext.User.Claims.Any(c => c.Subject.RoleClaimType  == ClaimTypes.Role && requiredRoles.Contains(c.Value, StringComparer.OrdinalIgnoreCase)))

                        throw new CqApiForbidException();
                }
            }

            return handlerInfo;

        }

    }
}
