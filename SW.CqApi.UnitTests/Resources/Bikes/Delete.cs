using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Bikes
{
    public class Delete : IDeleteHandler<int, object>
    {
        public async Task<object> Handle(int key)
        {
            throw new NotImplementedException();
        }
    }
}
