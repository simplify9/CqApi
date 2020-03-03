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
            components.SecuritySchemes.Add("oauth", new OpenApiSecurityScheme
            {
                Scheme = options.AuthScheme?? "https",
                OpenIdConnectUrl = options.OpenIdConnectUrl?? new Uri("https://www.url.com"),
                Type = options.AuthType.HasValue? options.AuthType.ToSecuritySchemeType() : SecuritySchemeType.OAuth2,
                In = options.In.HasValue? options.In.ToParameterLoc() : ParameterLocation.Header,
                Name = options.AuthTitle?? "oauth2",
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = options.AuthUrl?? new Uri("https://www.url.com"),
                        TokenUrl = options.TokenUrl?? new Uri("https://www.url.com"),
                        Scopes = roles.ToDictionary(x => 
                            options.RolesWithDescription.ContainsKey(x)? 
                            options.RolesWithDescription[x] : 
                            x.Contains("*")? $"Used for full access of {x.Split(".")[0]}": 
                            $"Used to {x.Split('.')[1]} {x.Split('.')[0]}"),
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
