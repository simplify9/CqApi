using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SW.CqApi.Client;

namespace SW.CqApi.UnitTests.Resources.Cars
{
    class ApiClient
    {
        private readonly HttpClient httpClient;

        public ApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task Create(CreateCommand command)
        {
            await httpClient.RunCommand(command);
        }
    }
}
