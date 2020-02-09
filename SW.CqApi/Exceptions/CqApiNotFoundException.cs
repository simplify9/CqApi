using SW.PrimitiveTypes;

namespace SW.CqApi
{
    public class CqApiNotFoundException : SWException
    {
        public CqApiNotFoundException(string message) : base(message)
        {
        }
    }
}
