
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

using System.Threading.Tasks;

namespace SW.CqApi.Client
{
    internal static  class HttpContentExtensions
    {
        async public static Task<T> ReadAsAsync<T>(this HttpContent httpContent)
        {
            var resutlStr = await httpContent.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(resutlStr);
        }

    }
}
