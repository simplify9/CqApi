using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SW.PrimitiveTypes;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SW.CqApi
{
    internal class CqApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CqApiExceptionFilterAttribute>>();

            if (context.Exception is SWException)
            {
                if (context.Exception is SWNotFoundException)
                    context.Result = new NotFoundObjectResult(context.Exception.Message);

                else if (context.Exception is SWForbiddenException)
                    context.Result = new UnauthorizedResult();

                else if (context.Exception is SWUnauthorizedException)
                    context.Result = new UnauthorizedResult();

                else if (context.Exception is SWValidationException validationException)
                {
                    foreach (var kvp in validationException.Validations)
                        context.ModelState.AddModelError(kvp.Key, kvp.Value);

                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
                else
                {
                    context.ModelState.AddModelError(context.Exception.GetType().Name, context.Exception.Message);
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }

                logger.LogWarning(context.Exception, string.Empty);
            }
            else
                logger.LogError(context.Exception, string.Empty);

            return base.OnExceptionAsync(context);
        }
    }
}
