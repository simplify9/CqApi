using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Bags
{

    public class Rq
    {
        public string Q { get; set; }
    }
    public class Rs
    {
        public string Q { get; set; }
    }

    [HandlerName("special")]
    [Unprotect]
    public class HandledQuery : IQueryHandler<string, Rq,Rs>
    {
        public async Task<Rs> Handle(string key, Rq request)
        {
            return new Rs(){Q = "samer"};
        }
    }
}
