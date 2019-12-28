using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{

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
