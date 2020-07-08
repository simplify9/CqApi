using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{

    [HandlerName("ScanIncoming")]
    [Protect(RequireRole = true)]
    [Returns(Type = typeof(object), StatusCode = 202, Description = "lookup == false")]
    public class ScanIncoming : ICommandHandler<ScanIncomingCommand>
    {

        public ScanIncoming()
        {
        }

        async public Task<object> Handle(ScanIncomingCommand request)
        {
            return null;
        }
    }

    public class ScanIncomingCommand
    {
        public int Number { get; set; }
    }
}
