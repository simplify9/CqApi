using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Cars
{
    public class Get : IGetHandler<int>
    {
        public async Task<object> Handle(int key, bool lookup = false)
        {
            return key;
        }
    }
}
