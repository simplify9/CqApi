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
        OAuth2 = 4,
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
        /// <summary>
        /// Name of the cookie, header or query parameter used.
        /// </summary>
        public string AuthName { get; set; }
        /// <summary>
        ///  The name of the HTTP Authorization scheme to be used in the Authorization
        /// as defined in RFC7235.
        /// </summary>
        public string AuthScheme { get; set; }
        public Uri? TokenUrl { get; set; }
        public Uri? AuthUrl { get; set; }
        public Uri? RefreshUrl { get; set; }
        public Uri? OpenIdConnectUrl { get; set; }
        public ParameterLocation? In { get; set; }
        /// <summary>
        /// Roles with their keys in here will have their description replaced with the value
        /// </summary>
        public IDictionary<string, string>? RolesWithDescription { get; set; }
        public CqApiAuthOptions()
        {
            RolesWithDescription = new Dictionary<string, string>();
        }

    }
}
