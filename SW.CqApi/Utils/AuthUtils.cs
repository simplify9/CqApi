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
        public static SecuritySchemeType ToSecuritySchemeType(this AuthOptions.AuthType? type)
        {
            switch (type)
            {
                case AuthOptions.AuthType.Http:
                    return SecuritySchemeType.Http;
                case AuthOptions.AuthType.OAuth2:
                    return SecuritySchemeType.OAuth2;
                case AuthOptions.AuthType.OpenIdConnect:
                    return SecuritySchemeType.OpenIdConnect;
                case AuthOptions.AuthType.ApiKey:
                    return SecuritySchemeType.ApiKey;
                default:
                    return SecuritySchemeType.Http;
            }
        }

        public static ParameterLocation ToParameterLoc(this AuthOptions.ParameterLocation? location)
        {
            switch (location)
            {
                case AuthOptions.ParameterLocation.Cookie:
                    return ParameterLocation.Cookie;
                case AuthOptions.ParameterLocation.Header:
                    return ParameterLocation.Header;
                case AuthOptions.ParameterLocation.Path:
                    return ParameterLocation.Path;
                case AuthOptions.ParameterLocation.Query:
                    return ParameterLocation.Query;
                default:
                    return ParameterLocation.Header;
            }
        }

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
        public static OpenApiComponents AddSecurity(this OpenApiComponents components, IEnumerable<string> roles, AuthOptions.CqApiAuthOptions options)
        {
            components.SecuritySchemes.Add("auth", new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                Type = SecuritySchemeType.Http,
            });
            return components;
        }

        public static void AddSecurity(this OpenApiOperation apiOperation, string role, OpenApiComponents components)
        {
            apiOperation.Security = new List<OpenApiSecurityRequirement>();
            var security = new OpenApiSecurityRequirement();
            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "auth"
                }
            };
;
            security.Add(scheme, new List<string>());

            apiOperation.Security.Add(security);
        }
    }
}
