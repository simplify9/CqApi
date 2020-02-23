using SW.PrimitiveTypes;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Linq;

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
        public static OpenApiComponents AddSecurity(this OpenApiComponents components, IEnumerable<string> roles)
        {
            components.SecuritySchemes.Add("oauth", new OpenApiSecurityScheme
            {
                Scheme = "https",
                OpenIdConnectUrl = new Uri("https://www.url.com"),
                Type = SecuritySchemeType.OAuth2,
                In = ParameterLocation.Header,
                Name = "oauth2",
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://www.url.com"),
                        TokenUrl = new Uri("https://www.url.com"),
                        Scopes = roles.ToDictionary(x => $"Used to {x.Split('.')[1]} {x.Split('.')[0]}"),
                    }
                }
            });
            return components;
        }

        public static void AddSecurity(this OpenApiOperation apiOperation, string role, OpenApiComponents components)
        {
            apiOperation.Security = new List<OpenApiSecurityRequirement>();
            var security = new OpenApiSecurityRequirement();
            var dict = new Dictionary<string, string>();
            dict.Add(role, "");
            var scheme = components.SecuritySchemes["oauth"];
            security.Add(scheme, new List<string> { role });

            apiOperation.Security.Add(security);
        }
    }
}
