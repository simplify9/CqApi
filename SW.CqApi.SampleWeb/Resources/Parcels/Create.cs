
using SW.CqApi.SampleWeb.Model;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{

    [Returns(Description = "", StatusCode = 201, Type = typeof(string))]
    public class Create : ICommandHandler<CarDto>
    {
        async public Task<object> Handle(CarDto request)
        {
            return null;
        }
    }
}
