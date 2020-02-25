using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi.AuthOptions
{
    public enum AuthType
    {
        NoAuth = 0,
        OpenIdConnect = 1,
        ApiKey = 2,
        Http = 3,
        Oauth2 = 4,
    }

    public enum ParameterLocation
    {
        Query = 0,
        Header = 1,
        Path = 2,
        Cookie = 3
    }

    public class CqApiAuthOptions
    { 
        public AuthType? AuthType { get; set; }
        public string AuthTitle { get; set; }
        public Uri? TokenUrl { get; set; }
        public Uri? AuthUrl { get; set; }
        public Uri? RefreshUrl { get; set; }
        public Uri? OpenIdConnectUrl { get; set; }
        public ParameterLocation? In { get; set; }
        public IEnumerable<string>? Roles { get; set; }
        public IDictionary<string, string>? RolesWithDescription { get; set; }
        public CqApiAuthOptions()
        {
        }

    }
}
