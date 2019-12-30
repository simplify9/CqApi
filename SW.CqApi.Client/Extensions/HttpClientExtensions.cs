using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

using System.Threading.Tasks;

namespace SW.CqApi.Client
{
    public static class HttpClientExtensions
    {

        async public static Task RunCommand<TRequest>(this HttpClient client, TRequest request)
        {
            var responseMessage = await client.PostAsync(client.BaseAddress, request);
            responseMessage.EnsureSuccessStatusCode();
        }

        async public static Task RunCommand<TRequest>(this HttpClient client, object token, TRequest request)
        {
            var responseMessage = await client.PostAsync(new Uri(client.BaseAddress, token.ToString()) , request);
            responseMessage.EnsureSuccessStatusCode();
        }

        async public static Task RunCommand<TRequest>(this HttpClient client, object key, string commandName, TRequest request)
        {
            var responseMessage = await client.PostAsync(new Uri(client.BaseAddress, $"{key.ToString()}/{commandName}"), request);
            responseMessage.EnsureSuccessStatusCode();
        }

        static Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, Uri url, T model)
        {
            var modelStr = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            return client.PostAsync(url, modelStr);
        }
    }
}
