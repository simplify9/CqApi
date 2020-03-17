using SW.CqApi.AuthOptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi
{
    public class CqApiOptions
    {
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        public string Prefix { get; set; } = "cqapi";
        public IDictionary<string, string>? ResourceDefinitions { get; set; }
        public CqApiAuthOptions AuthOptions { get; set; }
        public CqApiOptions()
        {

        }





    }
}
