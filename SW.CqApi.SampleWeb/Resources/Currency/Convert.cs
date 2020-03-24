using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Currency
{
    public class ConvertDto
    {
        public int Amount { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
    public class Convert : IQueryHandler<ConvertDto>
    {
        public async Task<object> Handle(ConvertDto request)
        {
            return $"{request.Amount * 2}{request.To}";
        }
    }
}
