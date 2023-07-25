using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Cars
{
    class Update : ICommandHandler<int,CarDto, object>
    {
        public async Task<object> Handle(int key, CarDto request)
        {
            return $"{key}{request.Plate}";
        }
    }
}
