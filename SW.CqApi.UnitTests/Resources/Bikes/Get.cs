using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Bikes
{
    public class Get : IGetHandler<int, object>
    {
        public async Task<object> Handle(int key)
        {
            return key;
        }
    }
}
