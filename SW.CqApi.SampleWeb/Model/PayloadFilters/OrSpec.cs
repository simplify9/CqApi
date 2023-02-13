namespace SW.CqApi.SampleWeb.Model;

public interface IExchangePayloadReader
{
    bool TryGetValue(string path, out string value);
}

public class OrSpec : IPropertyMatchSpecification
{
    public IPropertyMatchSpecification Left { get; private set; }

    public IPropertyMatchSpecification Right { get; private set; }


    public OrSpec(IPropertyMatchSpecification left, IPropertyMatchSpecification right)
    {
        Left = left;
        Right = right;
    }


    public bool IsMatch(IExchangePayloadReader reader) => Left.IsMatch(reader) || Right.IsMatch(reader);

    public override string ToString()
    {
        return $"({Left}) OR ({Right})";
    }

    public string Name => "or";
}