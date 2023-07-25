using SW.CqApi.SampleWeb.Model;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{

    [Returns(Type = typeof(CarOwner), StatusCode = 201, Description = "If lookup is false")]
    [Returns(Type = typeof(string), StatusCode = 202, Description = "If lookup is true")]
    public class Get : IGetHandler<int, object>
    {
        async public Task<object> Handle(int key)
        {
            return  new 
            { 
                Name = "Samer",
                Age = 13
            };
        }
    }
}
