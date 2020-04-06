using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Cars
{
    public class TestQDto
    {
        public int Multiplier { get; set; }
    }
    public class QueryGen2 : IQueryHandler<int, TestQDto>
    {
        public async Task<object> Handle(int key, TestQDto request)
        {
            return key * request.Multiplier;
        }
    }
}
