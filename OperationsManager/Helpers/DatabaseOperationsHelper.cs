using Newtonsoft.Json;

namespace OperationsManager.Helpers
{
    public class DatabaseOperationsHelper
    {
        public static async Task<bool> Post(string url, object body)
        {
            var response = await new HttpClient().PostAsJsonAsync(url, body);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return true;
        }

        public static async Task<bool> Put(string url, object body)
        {
            var response = await new HttpClient().PutAsJsonAsync(url, body);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return true;
        }

        public static async Task<bool> Delete(string url)
        {
            var response = await new HttpClient().DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return true;
        }
      

    }
}
