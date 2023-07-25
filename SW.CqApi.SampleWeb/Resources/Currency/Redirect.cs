using System.Threading.Tasks;
using SW.PrimitiveTypes;

namespace SW.CqApi.SampleWeb.Resources.Currency
{
    [HandlerName("redirect")]
    [Unprotect]
    public class Redirect : IGetHandler<string, object>
    {
        public async Task<object> Handle(string key)
        {
            return new CqApiResult<string>("https://simplify9.com")
            {
                Status = CqApiResultStatus.ChangedLocation
            };
        }
    }
}