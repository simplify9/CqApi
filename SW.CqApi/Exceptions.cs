using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi
{
    public class CqApiNotFoundException : SWException
    {
        public CqApiNotFoundException(string message) : base(message)
        {
        }
    }

    public class CqApiForbidException : SWException
    {
    }

    public class CqApiUnauthorizedException : SWException
    {
    }
}
