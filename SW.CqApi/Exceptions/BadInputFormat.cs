using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CqApi
{
    public class BadInputFormat : SWException
    {
        public BadInputFormat(Exception innerException) : base("Input was not in the correct format.", innerException)
        {
        }
    }
}
