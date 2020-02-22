using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SW.CqApi
{
    internal class RequestContext : IRequestContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string correlationId;

        public RequestContext(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            correlationId = Guid.NewGuid().ToString("N");
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            if (httpContext.User.Identity.IsAuthenticated)
            {
                User = httpContext.User;
                IsValid = true;
            }
        }

        public ClaimsPrincipal User { get; }
        public bool IsValid { get; }

        public IReadOnlyCollection<RequestValue> Values
        {
            get
            {
                if (httpContextAccessor.HttpContext == null) return null;

                var vals = new List<RequestValue>();

                foreach (var h in httpContextAccessor.HttpContext.Request.Headers)
                {
                    vals.Add(new RequestValue(h.Key, string.Join(";", h.Value.ToArray()), RequestValueType.HttpHeader));
                }

                var ignoredKeys = new string[] { "filter", "sort", "size", "page", "count" };

                foreach (var q in httpContextAccessor.HttpContext.Request.Query)

                    if (!ignoredKeys.Contains(q.Key, StringComparer.OrdinalIgnoreCase))

                        vals.Add(new RequestValue(q.Key, string.Join(";", q.Value.ToArray()), RequestValueType.QueryParameter));

                return vals;
            }
        }

        public string CorrelationId
        {
            get
            {
                var httpContext = httpContextAccessor.HttpContext;

                if (httpContext == null)
                    return null;

                if (httpContext.Request.Headers.TryGetValue("request-correlation-id", out var cid) && cid.Count > 0)
                    return cid.First();


                return correlationId;
            }
        }
    }
}
