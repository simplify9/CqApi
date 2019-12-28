using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{
    [HandlerName("approve") ]
    public class Approve : ICommandHandler<int, ApproveCarCommand> 
    {
        async public Task<object> Handle(int key, ApproveCarCommand request)
        {
            return null;
        }
    }

    public class ApproveCarCommand
    {
        public int ApprovalNumber { get; set; }
    }


}
