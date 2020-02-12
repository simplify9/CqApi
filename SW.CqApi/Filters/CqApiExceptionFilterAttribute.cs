using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.CqApi
{
    internal class CqApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is CqApiNotFoundException)
                context.Result = new NotFoundObjectResult(context.Exception.Message);

            else if (context.Exception is CqApiUnauthorizedException)

                context.Result = new UnauthorizedResult();

            else if (context.Exception is CqApiForbidException)

                context.Result = new UnauthorizedResult();

            else if (context.Exception is SWException)
            {
                context.ModelState.AddModelError(context.Exception.GetType().Name, context.Exception.Message);
                context.Result = new BadRequestObjectResult(context.ModelState);
            }

            
            return base.OnExceptionAsync(context);
        }
    }
}
