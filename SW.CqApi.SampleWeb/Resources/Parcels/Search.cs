
using SW.CqApi.SampleModel;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{

    public class Search : ISearchyHandler
    {
        private readonly IRequestContext requestContext;

        public Search(IRequestContext requestContext)
        {
            this.requestContext = requestContext;
        }

        async public Task<object> Handle(SearchyRequest searchyRequest, bool lookup = false, string searchPhrase = null)
        {
            var user = requestContext.User; 
            return new SearchyResponse<CarDto>(); 
        }
    }
}
