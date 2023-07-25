using System.Threading.Tasks;
using SW.CqApi.SampleWeb.Model;
using SW.PrimitiveTypes;

namespace SW.CqApi.SampleWeb.Resources.PropertyMatchSpecification;

public class TestModel
{
    public IPropertyMatchSpecification MatchExpression { get; set; }
}
[Unprotect]
public class Post : ICommandHandler<TestModel, object>
{
    public async Task<object> Handle(TestModel request)
    {
        return request.MatchExpression;
    }
}