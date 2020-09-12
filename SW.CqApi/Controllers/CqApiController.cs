using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SW.PrimitiveTypes;
using Newtonsoft.Json;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using SW.HttpExtensions;

namespace SW.CqApi
{

    [CqApiExceptionFilter]
    [Route("{prefix:cqapiPrefix}")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [AllowAnonymous]
    public class CqApiController : ControllerBase
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ServiceDiscovery serviceDiscovery;
        private readonly ILogger<CqApiController> logger;

        public CqApiController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            logger = serviceProvider.GetService<ILogger<CqApiController>>();
            serviceDiscovery = serviceProvider.GetService<ServiceDiscovery>();
        }

        [HttpGet("_roles")]
        public ActionResult<IDictionary<string, string>> GetRoles()
        {
            var sd = serviceProvider.GetService<ServiceDiscovery>();
            return Ok(sd.GetRoles().OrderBy(e => e).ToDictionary(k => k, v => v));
        }

        [HttpGet("_roles/{role}")]
        public ActionResult<string> GetRole(string role)
        {
            return role;
        }

        [HttpGet("swagger.json")]
        public ActionResult<string> GetOpenApiDocument()
        {
            var sd = serviceProvider.GetService<ServiceDiscovery>();
            return Ok(sd.GetOpenApiDocument());
        }


        [HttpGet("{resourceName}")]
        public Task<IActionResult> Get(
            string resourceName,
            [FromQuery(Name = "filter")] string[] filters,
            [FromQuery(Name = "sort")] string[] sorts,
            [FromQuery(Name = "size")] int pageSize,
            [FromQuery(Name = "page")] int pageIndex,
            [FromQuery(Name = "count")] bool countRows,
            [FromQuery(Name = "search")] string searchPhrase,
            [FromQuery(Name = "lookup")] bool lookup)
        {
            var handlerInfo = serviceDiscovery.ResolveHandler(resourceName, "get");
            var searchyRequest = new SearchyRequest(filters, sorts, pageSize, pageIndex, countRows);
            return ExecuteHandler(handlerInfo, searchyRequest, lookup, searchPhrase, null, null);

        }

        [HttpGet("{resourceName}/{key}/{token}")]
        public async Task<IActionResult> GetWithTokenAndKey(
            string resourceName,
            string token,
            string key)
        {

            var handler = serviceDiscovery.ResolveHandler(resourceName, $"get/key/{token}");
            return await ExecuteHandler(handler, null, false, null, key, null);

        }

        [HttpGet("{resourceName}/{token}")]
        public async Task<IActionResult> GetWithToken(
            string resourceName,
            string token,
            [FromQuery(Name = "filter")] string[] filters,
            [FromQuery(Name = "sort")] string[] sorts,
            [FromQuery(Name = "size")] int pageSize,
            [FromQuery(Name = "page")] int pageIndex,
            [FromQuery(Name = "search")] string searchPhrase,
            [FromQuery(Name = "count")] bool countRows,
            [FromQuery(Name = "lookup")] bool lookup)
        {

            var searchyRequest = new SearchyRequest(filters, sorts, pageSize, pageIndex, countRows);
            if (String.IsNullOrEmpty(searchyRequest.ToString())) searchyRequest = null;

            if (serviceDiscovery.TryResolveHandler(resourceName, $"get/{token}", out var handlerInfo))
                return await ExecuteHandler(handlerInfo, searchyRequest, lookup, searchPhrase, null, null);

            else if (serviceDiscovery.TryResolveHandler(resourceName, "get/key", out handlerInfo))
                return await ExecuteHandler(handlerInfo, searchyRequest, lookup, searchPhrase, token, null);

            else
                return NotFound();

        }

        [HttpPost("{resourceName}")]
        public async Task<IActionResult> Post(string resourceName, [FromBody] object body)
        {
            var handlerInfo = serviceDiscovery.ResolveHandler(resourceName, "post");
            return await ExecuteHandler(handlerInfo, null, false, null, null, body);
            //return Created(new Uri(new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}"), $"/mapi/{resourceName}/{key}"), null);
            //new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}"),
        }

        [HttpPost("{resourceName}/{token}")]
        public async Task<IActionResult> PostWithToken(string resourceName, string token, [FromBody] object body)
        {

            if (serviceDiscovery.TryResolveHandler(resourceName, $"post/{token}", out var handlerInfo))

                return await ExecuteHandler(handlerInfo, null, false, null, null, body);

            else if (serviceDiscovery.TryResolveHandler(resourceName, "post/key", out handlerInfo))

                return await ExecuteHandler(handlerInfo, null, false, null, token, body);

            else
                return NotFound();

        }

        [HttpPost("{resourceName}/{key}/{command}")]
        public async Task<IActionResult> PostWithKeyAndCommandName(string resourceName, string key, string command, [FromBody] object body)
        {
            var handlerInfo = serviceDiscovery.ResolveHandler(resourceName, $"post/key/{command}");
            return await ExecuteHandler(handlerInfo, null, false, null, key, body);
        }

        [HttpDelete("{resourceName}/{key}")]
        public async Task<IActionResult> Delete(string resourceName, string key)
        {
            var handlerInfo = serviceDiscovery.ResolveHandler(resourceName, "delete/key");
            return await ExecuteHandler(handlerInfo, null, false, null, key, null);
        }

        async Task<bool> ValidateInput(object input)
        {
            var tvalidator = typeof(IValidator<>).MakeGenericType(input.GetType());

            if (!(serviceProvider.GetService(tvalidator) is IValidator validator)) return true;

            var context = new ValidationContext<object>(input);
            var validationResult = await validator.ValidateAsync(context);

            if (validationResult.IsValid) return true;

            foreach (var error in validationResult.Errors)
                ModelState.AddModelError($"Field.{error.PropertyName}", error.ErrorMessage);

            return false;
        }

        async Task<IActionResult> ExecuteHandler(HandlerInfo handlerInfo, SearchyRequest searchyRequest, bool lookup, string searchPhrase, string key, object body)
        {
            var handlerInstance = await serviceProvider.GetHandlerInstance(handlerInfo);

            if (handlerInfo.NormalizedInterfaceType == typeof(ISearchyHandler))
            {
                var result = await handlerInstance.Invoke(searchyRequest, lookup, searchPhrase);
                if (lookup) return StatusCode(206, result);
                return Ok(result);

            }
            else if (handlerInfo.NormalizedInterfaceType == typeof(IQueryHandler))
            {
                var result = await handlerInstance.Invoke();
                return Ok(result);
            }
            else if (handlerInfo.NormalizedInterfaceType == typeof(IQueryHandler<>))
            {
                var request = Request.Query.GetInstance(handlerInfo.ArgumentTypes[0]);
                var result = await handlerInstance.Invoke(request);
                return Ok(result);
            }
            else if (handlerInfo.NormalizedInterfaceType == typeof(IQueryHandler<,>))
            {
                object keyParam;
                try
                {
                    keyParam = key.ConvertValueToType(handlerInfo.ArgumentTypes[0]);
                }
                catch (Exception ex)
                {
                    throw new BadInputFormatException(ex);
                }
                var request = Request.Query.GetInstance(handlerInfo.ArgumentTypes[1]);
                var result = await handlerInstance.Invoke(keyParam, request);
                return Ok(result);
            }
            else if (handlerInfo.NormalizedInterfaceType == typeof(ICommandHandler))
            {
                var result = await handlerInstance.Invoke();
                return HandleResult(result);
            }
            else if (handlerInfo.NormalizedInterfaceType == typeof(ICommandHandler<>))
            {
                object typedParam;
                try
                {
                    typedParam = JsonConvert.DeserializeObject(body.ToString(), handlerInfo.ArgumentTypes[0]);
                }
                catch (Exception ex)
                {
                    throw new BadInputFormatException(ex);
                }
                if (!await ValidateInput(typedParam)) return BadRequest(ModelState);
                var result = await handlerInstance.Invoke(typedParam);
                return HandleResult(result);
            }
            else if (handlerInfo.NormalizedInterfaceType == typeof(ICommandHandler<,>))
            {
                object typedParam;
                object keyParam;
                try
                {
                    keyParam = key.ConvertValueToType(handlerInfo.ArgumentTypes[0]);
                    typedParam = JsonConvert.DeserializeObject(body.ToString(), handlerInfo.ArgumentTypes[1]);

                }
                catch (Exception ex)
                {
                    throw new BadInputFormatException(ex);
                }

                if (!await ValidateInput(typedParam)) return BadRequest(ModelState);
                var result = await handlerInstance.Invoke(keyParam, typedParam);
                return HandleResult(result);
            }
            else if (handlerInfo.NormalizedInterfaceType == typeof(IGetHandler<>))
            {
                object keyParam;
                try
                {
                    keyParam = key.ConvertValueToType(handlerInfo.ArgumentTypes[0]);
                }
                catch (Exception ex)
                {
                    throw new BadInputFormatException(ex);
                }
                var result = await handlerInstance.Invoke(keyParam, lookup);
                if (result == null) return NotFound();
                if (lookup) return StatusCode(206, result);
                return Ok(result);
            }
            else if (handlerInfo.NormalizedInterfaceType == typeof(IDeleteHandler<>))
            {
                object keyParam;
                try
                {
                    keyParam = key.ConvertValueToType(handlerInfo.ArgumentTypes[0]);
                }
                catch (Exception ex)
                {
                    throw new BadInputFormatException(ex);
                }
                var result = await handlerInstance.Invoke(keyParam);
                return Accepted();
            }
            else
            {
                return NotFound();
            }
        }

        IActionResult HandleResult(object result)
        {
            if (result is IUnderProcessing underProcessing)
            {
                return Accepted(underProcessing.Uri);
            }
            else if (result is IResultWithHeaders resultWithHeaders)
            {
                foreach (var kvp in resultWithHeaders.Headers)
                    Response.Headers.Add(kvp.Key, kvp.Value);
                return Ok(resultWithHeaders.Result);
            }
            else if (result == null) return NoContent();
            return Ok(result);
        }
    }
}

