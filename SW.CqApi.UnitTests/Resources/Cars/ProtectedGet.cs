using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Cars
{

    [HandlerName("protectedget")]
    [Protect]
    class ProtectedGet : ICommandHandler<ProtectedGetCommand>
    {
        public Task<object> Handle(ProtectedGetCommand request)
        {
            return null;
        }
    }

    class ProtectedGetCommand
    {
        public string Owner { get; set; }
    }
}
