//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Primitives;
//using SW.PrimitiveTypes;
//using System;
//using System.Collections.Generic;

//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace SW.CqApi
//{
//    internal class RequestContextProvider : IRequestContextProvider
//    {
//        private readonly IHttpContextAccessor httpContextAccessor;

//        public RequestContextProvider(IHttpContextAccessor httpContextAccessor)
//        {
//            this.httpContextAccessor = httpContextAccessor;
//        }

//        async public Task<RequestContext> GetContext()
//        {
//            //var ignoredKeys = new string[] { "filter", "sort", "size", "page", "count", "request-correlation-id" };
//            var httpContext = httpContextAccessor.HttpContext;

//            if (httpContext == null) return null;
//            if (!httpContext.User.Identity.IsAuthenticated) return null;

//            var vals = new List<RequestValue>();

//            foreach (var h in httpContextAccessor.HttpContext.Request.Headers)
//                vals.Add(new RequestValue(h.Key, string.Join(";", h.Value.ToArray()), RequestValueType.HttpHeader));

//            foreach (var q in httpContextAccessor.HttpContext.Request.Query)
//                //if (!ignoredKeys.Contains(q.Key, StringComparer.OrdinalIgnoreCase))
//                vals.Add(new RequestValue(q.Key, string.Join(";", q.Value.ToArray()), RequestValueType.QueryParameter));

//            string correlationId = Guid.NewGuid().ToString("N");

//            if (httpContext.Request.Headers.TryGetValue("request-correlation-id", out var cid) && cid.Count > 0)
//                correlationId = cid.First();

//            return new RequestContext(httpContext.User, vals, correlationId);
//        }

//        //public ClaimsPrincipal User
//        //{
//        //    get
//        //    {
//        //        var httpContext = httpContextAccessor.HttpContext;
//        //        if (httpContext == null) return null;
//        //        if (httpContext.User.Identity.IsAuthenticated)
//        //        {
//        //            return httpContext.User;
//        //        }
//        //        return null;
//        //    }
//        //}

//        //public bool IsValid
//        //{
//        //    get
//        //    {
//        //        var httpContext = httpContextAccessor.HttpContext;
//        //        if (httpContext == null) return false;
//        //        if (httpContext.User.Identity.IsAuthenticated)
//        //        {
//        //            return true;
//        //        }
//        //        return false;
//        //    }
//        //}

//        //public IReadOnlyCollection<RequestValue> Values
//        //{
//        //    get
//        //    {
//        //        if (httpContextAccessor.HttpContext == null) return null;

//        //        var vals = new List<RequestValue>();

//        //        foreach (var h in httpContextAccessor.HttpContext.Request.Headers)
//        //        {
//        //            vals.Add(new RequestValue(h.Key, string.Join(";", h.Value.ToArray()), RequestValueType.HttpHeader));
//        //        }

//        //        var ignoredKeys = new string[] { "filter", "sort", "size", "page", "count" };

//        //        foreach (var q in httpContextAccessor.HttpContext.Request.Query)

//        //            if (!ignoredKeys.Contains(q.Key, StringComparer.OrdinalIgnoreCase))

//        //                vals.Add(new RequestValue(q.Key, string.Join(";", q.Value.ToArray()), RequestValueType.QueryParameter));

//        //        return vals;
//        //    }
//        //}

//        //public string CorrelationId
//        //{
//        //    get
//        //    {
//        //        var httpContext = httpContextAccessor.HttpContext;

//        //        if (httpContext == null)
//        //            return null;

//        //        if (httpContext.Request.Headers.TryGetValue("request-correlation-id", out var cid) && cid.Count > 0)
//        //            return cid.First();


//        //        return correlationId;
//        //    }
//        //}

//        public string Name => "Http";


//    }
//}
