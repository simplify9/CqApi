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
        public static void UseCqApi(this IApplicationBuilder app)
        {

            app.Use(async (context, next) =>
            {

                CqApiOptions options = (CqApiOptions)context.RequestServices.GetService(typeof(CqApiOptions));
                IList<string> pathArr = new List<string>(context.Request.Path.Value.Split('/').Skip(1));
                IList<string> segmented = new List<string>(pathArr.Take(pathArr.IndexOf(options.Prefix)));

                if (segmented.Count == 0)
                {
                    await next();
                    return;
                }

                var rc = (RequestContext)context.RequestServices.GetService(typeof(RequestContext));
                
                //Set in request context.
                foreach(var segment in segmented) {

                    if(new Regex("v.*").Match(segment).Success)
                    {
                        rc.SetVersion(segment);
                        pathArr.Remove(segment);
                        continue;
                    }

                    bool langCultureMatch = new Regex("..-..").Match(segment).Success;
                    bool langMatch = new Regex("..").Match(segment).Success;

                    if(langCultureMatch || langMatch)
                    {
                        rc.SetLocale(segment);
                        pathArr.Remove(segment);
                        continue;
                    }
                }

                context.Request.Path = '/' + string.Join('/', pathArr);
                
                await next();
            });
        }
    }
}
