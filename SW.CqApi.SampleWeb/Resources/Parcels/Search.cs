
using SW.CqApi.SampleWeb.Model;
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
        private readonly RequestContextManager requestContextManager;

        public Search(RequestContextManager requestContextManager)
        {
            this.requestContextManager = requestContextManager;
        }

        async public Task<object> Handle(SearchyRequest searchyRequest, bool lookup = false, string searchPhrase = null)
        {
            var user = (await requestContextManager.GetCurrentContext()).User;
            return new SearchyResponse<CarDto>();
        }
    }
}
