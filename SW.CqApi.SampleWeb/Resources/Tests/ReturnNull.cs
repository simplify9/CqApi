using System.Threading.Tasks;
using SW.PrimitiveTypes;

namespace SW.CqApi.SampleWeb.Resources.Tests;

[HandlerName("ReturnNull")]
[Unprotect]
public class ReturnNull : ICommandHandler<object, object>
{
    public async Task<object> Handle(object request)
    {
        return null;
    }
}