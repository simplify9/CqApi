using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi
{
    public class CqapiPrefixRouteConstraint : IRouteConstraint
    {
        private readonly CqApiOptions options;

        public CqapiPrefixRouteConstraint(CqApiOptions options)
        {
            this.options = options;
        }



        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var controllerPrefix = options.UrlPrefix?? "cqapi";
            var prefix = values["prefix"].ToString();
            if (prefix.ToLower() == controllerPrefix) return true;
            return false;
        }
    }
}
