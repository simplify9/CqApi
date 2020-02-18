using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.CqApi.Utils
{
    public static class SecurityUtils
    {
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
    }
}
