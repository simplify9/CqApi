
using SW.CqApi.SampleModel;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{
    public class Search : ISearchHandler
    {
        async public Task<object> Handle(SearchyRequest searchyRequest, bool lookup = false, string searchPhrase = null)
        {
            return new SearchyResponse<CarDto>(); 
        }
    }
}
