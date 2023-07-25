using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{
    class Delete : IDeleteHandler<int, object>
    {
        public Task<object> Handle(int key)
        {
            throw new NotImplementedException();
        }
    }
}
