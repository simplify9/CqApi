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

        public static HandlerInstance GetHandlerInstance(this IServiceProvider serviceProvider, HandlerInfo handlerInfo)
        {
            var handlerInstance = new HandlerInstance
            {
                Method = handlerInfo.Method,
                Instance = serviceProvider.GetService(handlerInfo.HandlerType)
            };

            if (handlerInstance.Instance is null) 
                
                throw new SWException($"Could not find required service {handlerInfo.Key} for resource {handlerInfo.Resource}.");

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
                        $"{handlerInfo.Resource}.{handlerInfo.HandlerType.Name}", 
                        $"{handlerInfo.Resource}.*" 
                    };

                    if (!requestContext.User.Claims.Any(c => c.Subject.RoleClaimType  == ClaimTypes.Role && requiredRoles.Contains(c.Value, StringComparer.OrdinalIgnoreCase)))

                        throw new CqApiForbidException();
                }
            }

            return handlerInstance;

        }

    }
}
