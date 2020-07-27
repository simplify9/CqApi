using SW.CqApi.SampleWeb.Resources.Currency;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{

    public class SpecialQuery : IQueryHandler<string, ConvertDto>
    {
        private readonly RequestContext context;
        public SpecialQuery(RequestContext context)
        {
            this.context = context;
        }
        public async Task<object> Handle(string key, ConvertDto request)
        {
            var locale = context.CorrelationId;
            return key;
        }
    }
}
