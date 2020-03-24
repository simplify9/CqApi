using SW.CqApi.AuthOptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi
{
    public class CqApiOptions
    {
        /// <summary>
        /// The project's name
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// The project's description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The controller prefix, how the API is accessed. 
        /// Do not add a trailing slash.
        /// </summary>
        public string Prefix { get; set; } = "cqapi";
        /// <summary>
        /// Any resource with its key found here, will have its description replaced with the value
        /// </summary>
        public IDictionary<string, string>? ResourceDefinitions { get; set; }
        /// <summary>
        /// Authentication settings
        /// </summary>
        public CqApiAuthOptions AuthOptions { get; set; }
        public CqApiOptions()
        {

        }





    }
}
