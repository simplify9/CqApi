using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.CqApi.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Note: This function needs to be placed before UseRouting() (or similar functions)
        /// to ensure the rewrite process is successful
        /// </summary>
        /// <param name="builder"></param>
        public static void UseCqApi(this IApplicationBuilder builder)
        {

            builder.Use(async (context, next) =>
            {

                CqApiOptions options = (CqApiOptions)context.RequestServices.GetService(typeof(CqApiOptions));
                string scheme = context.Request.Scheme;
                string host = context.Request.Host.Value;
                string fullPath = context.Request.GetDisplayUrl();

                string route = fullPath.Substring(scheme.Length + 3 + host.Length);

                if (route.Count(s => s == '/') < 2)
                {
                    await next.Invoke();
                    return;
                }
                

                List<string> pathArr = new List<string>(route.Split('/').Skip(1));

                IList<string> segmented = new List<string>(pathArr.Take(pathArr.IndexOf(options.Prefix)));

                var rc = (RequestContext)context.RequestServices.GetService(typeof(RequestContext));

                if (segmented.Count == 0)
                {
                    await next.Invoke();
                    return;
                }

                //Set in request context.
                foreach(var segment in segmented) {

                    if(new Regex("v.*").Match(segment).Success)
                    {
                        //set version in rc
                        pathArr.Remove(segment);
                        continue;
                    }

                    bool langCultureMatch = new Regex("..-..").Match(segment).Success;
                    bool langMatch = new Regex("..").Match(segment).Success;

                    if(langCultureMatch || langMatch)
                    {
                        //set locale
                        rc.Set(null, null, "--TEST123");
                        pathArr.Remove(segment);
                        continue;
                    }
                }


                string newPath = scheme + "://" + host + '/' + string.Join('/', pathArr);

                context.Response.Headers[HeaderNames.Location] = newPath;
                context.Request.Path = '/' + string.Join('/', pathArr);

                string tmp = context.Request.GetDisplayUrl();
                
                await next();
            });
        }
    }
}
