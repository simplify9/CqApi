using System.Threading.Tasks;
using SW.PrimitiveTypes;

namespace SW.CqApi.SampleWeb.Resources.Tests;

[HandlerName("ReturnObject")]
[Unprotect]
public class ReturnObject : ICommandHandler<object, object>
{
    public async Task<object> Handle(object request)
    {
        return new
        {
            Name = "Cq",
            Version = new
            {
                Major = 6,
                minor = 1,
                patch = 0
            }
        };
    }
}