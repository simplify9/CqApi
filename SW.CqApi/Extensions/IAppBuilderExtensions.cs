using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.CqApi.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void UseCqApiRouting(this IApplicationBuilder builder)
        {

            builder.Use(async (context, next) =>
            {

                CqApiOptions options = (CqApiOptions)context.RequestServices.GetService(typeof(CqApiOptions));
                string protocol = context.Request.Protocol;
                string host = context.Request.Host.Host;
                string fullPath = context.Request.Path.Value;

                string route = fullPath.Substring(protocol.Length + host.Length);
                
                List<string> pathArr = new List<string>(route.Split('/'));

                IList<string> segmented = new List<string>(pathArr.Take(pathArr.IndexOf(options.Prefix)));

                var rc = (RequestContext)context.RequestServices.GetService(typeof(RequestContext));

                //Set in request context.
                foreach(var segment in segmented) {

                    if(new Regex("v.*").Match(segment).Success)
                    {
                        //set version in rc
                        segmented.Remove(segment);
                    }
                    else if(new Regex("..-..").Match(segment).Success)
                    {
                        //set locale
                        segmented.Remove(segment);
                    }
                }

                string newPath = protocol + host + string.Join('/', segmented);

                context.Request.Path = newPath;

                await next.Invoke();
            });
        }
    }
}
