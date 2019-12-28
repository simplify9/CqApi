using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using SW.PrimitiveTypes;

namespace SW.Searchy
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchyController : ControllerBase
    {
        readonly IServiceProvider serviceProvider;
        public SearchyController(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;

        [HttpGet]
        public IActionResult List()
        {
            var svcs = serviceProvider.GetServices<ISearchyService>().Select(e => e.Serves).ToList() ;
            return new OkObjectResult(svcs);
        }

        ISearchyService GetService(string modelName)
        {
            var svcs = serviceProvider.GetServices<ISearchyService>().Where(e => e.Serves.ToLower().EndsWith(modelName.ToLower()));
            return svcs.SingleOrDefault();
        }

        [HttpPost("{serviceName}")]
        public async Task<IActionResult> Search(string serviceName, [FromBody]SearchyRequest request)
        {
            var svc = GetService(serviceName);

            if (svc == null) return new NotFoundObjectResult(serviceName);
            return new OkObjectResult(await svc.Search(request));
        }

        [HttpGet("{serviceName}/filter")]
        public IActionResult GetFilterConfigs( string serviceName)
        {
            var svc = GetService(serviceName);
            if (svc == null) return new NotFoundObjectResult(serviceName);
            return new OkObjectResult(svc.FilterConfigs);
        }
    }
}