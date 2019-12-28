
using SW.CqApi.SampleModel;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{
    public class Update : ICommandHandler<int, CarDto>
    {
        async public Task<object> Handle(int key, CarDto request)
        {
            return null;
        }
    }
}
