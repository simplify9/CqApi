using SW.PrimitiveTypes;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Microsoft.OpenApi.Models;

namespace SW.CqApi.Utils
{
    internal static class AuthUtils
    {
        public static IEnumerable<string> GetRoles(this IDictionary<string, IDictionary<string, HandlerInfo>> resourceHandlers)
        {
            List<string> roles = new List<string>();
            foreach (var res in resourceHandlers)
            {
                bool secured = false;
                var key = res.Key.ToLower();
                foreach(var handler in res.Value)
                {
                    var protect = handler.Value.HandlerType.GetCustomAttribute<ProtectAttribute>();
                    if(protect != null && protect.RequireRole)
                    {
                        secured = true;
                        string name = handler.Value.HandlerType.Name.ToLower();
                        roles.Add( $"{key}.{name}");
                    }
                }
                if(secured) roles.Add( $"{key}.*");
            }
            return roles;
        }

        public static void AddSecurity(this OpenApiOperation apiOperation, string role)
        {
            apiOperation.Security = new List<OpenApiSecurityRequirement>();
            var security = new OpenApiSecurityRequirement();
            var dict = new Dictionary<string, string>();
            dict.Add(role, "");
            var scheme = new OpenApiSecurityScheme
            {
                Name = "Security",
                Type = SecuritySchemeType.OAuth2,
                In = ParameterLocation.Cookie,
                OpenIdConnectUrl = new Uri("https://www.url.com"),
                Scheme = "scheme",
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://www.url.com"),
                        TokenUrl = new Uri("https://www.url.com"),
                        Scopes = dict
                    }
                }
            };
            security.Add(scheme, new List<string> { role });

            apiOperation.Security.Add(security);
        }
    }
}
