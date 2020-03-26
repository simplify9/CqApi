using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Cars
{
    public class CarDto {
        public int Plate { get; set; }
    }
    public class QueryGen1 : IQueryHandler<CarDto>
    {
        public async Task<object> Handle(CarDto request)
        {
            return new CarDto
            {
                Plate = request.Plate
            };
        }
    }
}
