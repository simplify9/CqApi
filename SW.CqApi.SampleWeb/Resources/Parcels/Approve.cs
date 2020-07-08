using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CqApi.SampleWeb.Resources.Parcels
{


    [HandlerName("approve")]
    [Protect(RequireRole=true)]

    class Approve : ICommandHandler<int, ApproveCarCommand>
    {
        private readonly RequestContextManager requestContextManager;

        public Approve(RequestContextManager requestContextManager)
        {
            
            this.requestContextManager = requestContextManager;
        }

        async public Task<object> Handle(int key, ApproveCarCommand request)
        {


            throw new SWException("Invalid data.");
            //return null;
        }



        private class Validator : AbstractValidator<ApproveCarCommand>
        {
            public Validator()
            {
                RuleFor(p => p.Notes).NotEmpty();
            }
        }
    }



    public class ApproveCarCommand
    {
        public int ApprovalNumber { get; set; }
        public string Notes { get; set; }
    }




}
