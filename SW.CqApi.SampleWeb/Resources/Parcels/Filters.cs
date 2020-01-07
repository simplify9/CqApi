using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{

    [HandlerName("filters")]
    public class Filters : IQueryHandler
    {
        async public Task<object> Handle()
        {
            return 1;
        }
    }
}
