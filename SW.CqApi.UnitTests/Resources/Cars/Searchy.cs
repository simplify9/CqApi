using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Cars
{
    [HandlerName("search")]
    class Searchy : ISearchyHandler
    {
        public async Task<object> Handle(SearchyRequest searchyRequest, bool lookup = false, string searchPhrase = null)
        {
            var x =  searchyRequest.ToString();
            return x;
        }
    }
}
