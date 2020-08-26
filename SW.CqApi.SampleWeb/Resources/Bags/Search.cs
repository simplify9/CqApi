using SW.CqApi.SampleWeb.Model;
using SW.PrimitiveTypes;
using SW.PrimitiveTypes.Contracts.CqApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Bags
{
    [Unprotect]
    public class Search : ISearchyHandler
    {
        async public Task<object> Handle(SearchyRequest searchyRequest, bool lookup = false, string searchPhrase = null)
        {
            return new SearchyResponse<CarDto>();
        }
    }
}
