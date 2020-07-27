
using SW.CqApi.SampleWeb.Model;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{
    [Returns(Type = typeof(object), StatusCode = 202, Description = "lookup == false")]
    public class Update : ICommandHandler<int, CarDto>
    {
        async public Task<object> Handle(int key, CarDto request)
        {
            return null;
        }
    }
}
