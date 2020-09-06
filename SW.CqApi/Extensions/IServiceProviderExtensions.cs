using Microsoft.Extensions.DependencyInjection;
using SW.PrimitiveTypes;
using SW.PrimitiveTypes.Contracts.CqApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi
{
    internal static class IServiceProviderExtensions
    {
        async public static Task<HandlerInstance> GetHandlerInstance(this IServiceProvider serviceProvider, HandlerInfo handlerInfo)
        {
            var handlerInstance = new HandlerInstance
            {
                Method = handlerInfo.Method,
                Instance = serviceProvider.GetService(handlerInfo.HandlerType)
            };

            if (handlerInstance.Instance is null)

                throw new SWException($"Could not find required service {handlerInfo.Key} for resource {handlerInfo.Resource}.");

            CqApiOptions options = serviceProvider.GetService<CqApiOptions>() ?? new CqApiOptions();
            var protectAttribute = handlerInfo.HandlerType.GetCustomAttribute<ProtectAttribute>();
            var unprotectAttribute = handlerInfo.HandlerType.GetCustomAttribute<UnprotectAttribute>();

            if ((options.ProtectAll && unprotectAttribute == null) || protectAttribute is ProtectAttribute)
            {
                var requestContext = serviceProvider.GetRequiredService<RequestContext>();

                if (!requestContext.IsValid)

                    throw new SWUnauthorizedException();

                if (protectAttribute?.RequireRole ?? false)
                {

                    var prefix = string.IsNullOrWhiteSpace(options.RolePrefix) ? handlerInfo.Resource : $"{options.RolePrefix}.{handlerInfo.Resource}";

                    var requiredRoles = new string[]
                    {
                        $"{prefix}.{handlerInfo.HandlerType.Name}",
                        $"{prefix}.*"
                    };

                    if (!requestContext.User.Claims.Any(c => c.Subject.RoleClaimType == ClaimTypes.Role && requiredRoles.Contains(c.Value, StringComparer.OrdinalIgnoreCase)))
                        throw new SWForbiddenException();
                }
            }

            return handlerInstance;
        }

    }
}
