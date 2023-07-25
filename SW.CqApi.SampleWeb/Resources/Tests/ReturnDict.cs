using System.Collections.Generic;
using System.Threading.Tasks;
using SW.PrimitiveTypes;

namespace SW.CqApi.SampleWeb.Resources.Tests;

[HandlerName("ReturnDict")]
[Unprotect]
public class ReturnDict : ICommandHandler<object, object>
{
    public async Task<object> Handle(object request)
    {
        return new Dictionary<string, string>
        {
            ["key"] = "mykey",
            ["Name"] = "name",
            ["pHonE"] = "009"
        };
    }
}