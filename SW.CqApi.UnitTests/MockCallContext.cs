using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SW.ModelApi
{
    public class MockCallContext : IRequestContext
    {
        public MockCallContext(ClaimsPrincipal user)
        {
            User = user;
            //AdditionalValues = new Dictionary<string, string>();
        }

        public ClaimsPrincipal User { get; }

        public IReadOnlyCollection<RequestValue> Values => throw new NotImplementedException();

        public bool IsValid => throw new NotImplementedException();

        public string CorrelationId => throw new NotImplementedException();
    }
}
