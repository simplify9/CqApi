using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.CqApi.Extensions
{
    public static class IApplicationBuilderExtensions
    {
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


                string newPath = string.Join('/', pathArr);

                context.Request.Path = newPath;

                await next.Invoke();
            });
        }
    }
}
