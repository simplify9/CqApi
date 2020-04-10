using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Bikes
{

    public class WheelDto {
        public int Q { get; set; }
    }

    [HandlerName("Wheel")]
    public class GetWheelType : IQueryHandler<int, WheelDto>
    {
        public async Task<object> Handle(int key, WheelDto request)
        {
            return key + request.Q;
        }
    }
}
