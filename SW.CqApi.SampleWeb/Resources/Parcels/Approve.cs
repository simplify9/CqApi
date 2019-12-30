using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{
    [HandlerName("approve") ]
    
    class Approve : ICommandHandler<int, ApproveCarCommand> 
    {
        private readonly IRequestContext requestContext;

        public Approve(IRequestContext requestContext )
        {
            this.requestContext = requestContext;
        }

        async public Task<object> Handle(int key, ApproveCarCommand request)
        {
            return null;
        }
    }

    class ApproveCarCommand
    {
        public int ApprovalNumber { get; set; }
    }

    class Validator : AbstractValidator<ApproveCarCommand>
    {
        public Validator()
        {
            RuleFor(p => p.ApprovalNumber).NotEmpty();
        }
    }




}
