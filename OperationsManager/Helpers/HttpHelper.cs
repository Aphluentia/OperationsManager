using Newtonsoft.Json;
using OperationsManager.Database.Entities;
using System;

namespace OperationsManager.Helpers
{
    public class HttpHelper
    {
        public static async Task<ActionResponse> Post(string url, object body)
        {
            var response = await new HttpClient().PostAsJsonAsync(url, body);
            return await ActionResponse.FromHttpResponse(response);
        }
        public static async Task<string> Get(string url)
        {
            var response = await new HttpClient().GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return "";
            return await response.Content.ReadAsStringAsync();
        }


        public static async Task<ActionResponse> Put(string url, object body)
        {
            var response = await new HttpClient().PutAsJsonAsync(url, body);
            return await ActionResponse.FromHttpResponse(response);
        }

        public static async Task<ActionResponse> Delete(string url)
        {
            var response = await new HttpClient().DeleteAsync(url);
            return await ActionResponse.FromHttpResponse(response);
        }
      

    }
}
