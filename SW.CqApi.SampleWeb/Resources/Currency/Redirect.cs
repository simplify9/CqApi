using System.Threading.Tasks;
using SW.PrimitiveTypes;

namespace SW.CqApi.SampleWeb.Resources.Currency
{
    [HandlerName("redirect")]
    [Unprotect]
    public class Redirect : IGetHandler<string>
    {
        public async Task<object> Handle(string key, bool lookup = false)
        {
            return new CqApiResult<string>("https://simplify9.com")
            {
                Status = CqApiResultStatus.ChangedLocation
            };
        }
    }
}