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

    [HandlerName("special")]
    public class HandledQuery : IQueryHandler<string, Rq>
    {
        public async Task<object> Handle(string key, Rq request)
        {
            return 1;
        }
    }
}
