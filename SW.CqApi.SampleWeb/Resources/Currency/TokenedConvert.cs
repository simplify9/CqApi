using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Currency
{
    [HandlerName("convert")]
    public class TokenedConvert: IQueryHandler<ConvertDto, object>
    {
        public async Task<object> Handle(ConvertDto request)
        {
            return $"{request.Amount * 2}{request.To}";
        }
    }
}
