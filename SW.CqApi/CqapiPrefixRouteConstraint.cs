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
        private readonly IConfiguration configuration;



        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            //var config = httpContext.RequestServices.GetService<IConfiguration>()["cqapi:prefix"];
            var prefix = values["prefix"].ToString();
            if (prefix.ToLower() == "cqapi") return true;
            return false;
        }
    }
}
