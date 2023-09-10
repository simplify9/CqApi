using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SW.PrimitiveTypes;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace SW.CqApi
{
    internal class CqApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CqApiExceptionFilterAttribute>>();

            if (context.Exception is SWException || 
                context.Exception is TargetInvocationException { InnerException: SWException })
            {
                var exception = context.Exception is TargetInvocationException ? 
                    context.Exception.InnerException : context.Exception;
                if (exception is SWNotFoundException)
                    context.Result = new NotFoundObjectResult(exception.Message);

                else if (exception is SWForbiddenException)
                    context.Result = new UnauthorizedResult();

                else if (exception is SWUnauthorizedException)
                    context.Result = new UnauthorizedResult();

                else if (exception is SWValidationException validationException)
                {
                    foreach (var kvp in validationException.Validations)
                        context.ModelState.AddModelError(kvp.Key, kvp.Value);

                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
                else
                {
                    context.ModelState.AddModelError(exception.GetType().Name, exception.Message);
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }

                logger.LogWarning(exception, string.Empty);
            }
            else
                logger.LogError(context.Exception, string.Empty);

            return base.OnExceptionAsync(context);
        }
    }
}
