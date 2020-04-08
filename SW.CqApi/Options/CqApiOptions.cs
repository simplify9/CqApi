using Microsoft.OpenApi.Any;
using SW.CqApi.AuthOptions;
using SW.CqApi.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi
{

    public class ResourceDescriptions
    {
        private readonly IDictionary<string, string> Descriptions = new Dictionary<string, string>();

        public void Add(string key, string description)
        {
            Descriptions[key] = description;
        }
        public string Get(string key)
        {
            if (Descriptions.ContainsKey(key))
                return Descriptions[key];
            else return null;
        }

        public bool ContainsDescription(string key)
        {
            if (Descriptions.ContainsKey(key)) return true;
            return false;
        }
    }

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
        public ResourceDescriptions ResourceDescriptions { get; }
        /// <summary>
        /// Authentication settings
        /// </summary>
        public CqApiAuthOptions AuthOptions { get; set; }
        /// <summary>
        /// Maps for types, how they're sent through the API
        /// </summary>
        public TypeMaps Maps { get; }
        public CqApiOptions()
        {

            Maps = new TypeMaps();
            ResourceDescriptions = new ResourceDescriptions();
            var objectExample = new OpenApiObject();
            objectExample.TryAdd("foo", new OpenApiInteger(42));
            objectExample.TryAdd("bar", new OpenApiInteger(23));
            objectExample.TryAdd("baz", new OpenApiInteger(1337));
            Maps.AddMap<Dictionary<string, int>, object>(objectExample);

            objectExample.Remove("foo");
            objectExample.Remove("bar");
            objectExample.Remove("baz");

            objectExample.TryAdd("foo", new OpenApiString("lorem ipsum"));
            objectExample.TryAdd("bar", new OpenApiString("John Doe"));
            objectExample.TryAdd("baz", new OpenApiString("54"));

            Maps.AddMap<Dictionary<string, string>, object>(objectExample);
        }





    }
}
