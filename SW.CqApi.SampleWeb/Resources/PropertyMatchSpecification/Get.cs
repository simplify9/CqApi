using System.Threading.Tasks;
using SW.CqApi.SampleWeb.Model;
using SW.PrimitiveTypes;

namespace SW.CqApi.SampleWeb.Resources.PropertyMatchSpecification;

[Unprotect]
public class CustomFilters : IQueryHandler
{
    public async Task<object> Handle()
    {
        return new OrSpec(new AndSpec(new OneOfSpec("data", new[] { "s" }),
                new OrSpec(new OneOfSpec("status", new[] { "otf" }), new OneOfSpec("date", new[] { "123" }))),
            new NotOneOfSpec("hi", new[] { "asd" }));
    }
}