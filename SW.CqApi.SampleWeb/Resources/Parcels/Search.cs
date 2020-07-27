
using SW.CqApi.SampleModel;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{

    [Protect(RequireRole = true)]
    [Returns(Type = typeof(SearchyResponse<CarDto>), StatusCode = 200, Description = "lookup == false")]
    public class Search : ISearchyHandler
    {
        private readonly RequestContext requestContext;

        public Search(RequestContext requestContext)
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
