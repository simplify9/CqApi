using SW.CqApi.SampleWeb.Model;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{

    [Returns(Type = typeof(CarOwner), StatusCode = 200, Description = "If lookup is false")]
    [Returns(Type = typeof(string), StatusCode = 202, Description = "If lookup is true")]
    public class Get : IGetHandler<int>
    {
        async public Task<object> Handle(int key, bool lookup = false)
        {
            return  new 
            { 
                Name = "Samer",
                Age = 13
            };
        }
    }
}
