using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi.UnitTests.Resources.Cars
{
    class Create : ICommandHandler<CreateCommand>
    {
        async public Task<object> Handle(CreateCommand request)
        {
            return null;
        }
    }

    class CreateCommand
    {
        public string Owner { get; set; }
    }
}
