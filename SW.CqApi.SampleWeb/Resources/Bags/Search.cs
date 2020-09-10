using SW.CqApi.SampleWeb.Model;
using SW.PrimitiveTypes;
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
