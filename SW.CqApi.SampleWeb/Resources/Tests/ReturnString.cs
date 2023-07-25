using System.Threading.Tasks;
using SW.PrimitiveTypes;

namespace SW.CqApi.SampleWeb.Resources.Tests;
[HandlerName("ReturnString")]
[Unprotect]
public class ReturnString : ICommandHandler<object, object>
{
    public async Task<object> Handle(object request)
    {
        return "CqApi";
    }
}