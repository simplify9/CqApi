using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Cars
{
    class Search : IQueryHandler
    {
        public async Task<object> Handle()
        {
            return 1;
        }
    }
}
