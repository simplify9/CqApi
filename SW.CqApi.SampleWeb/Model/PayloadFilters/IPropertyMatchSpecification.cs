namespace SW.CqApi.SampleWeb.Model;


public interface IPropertyMatchSpecification
{
    bool IsMatch(IExchangePayloadReader reader);

    string Name { get; }
}