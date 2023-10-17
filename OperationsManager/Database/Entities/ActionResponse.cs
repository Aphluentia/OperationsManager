using System.Net;

namespace OperationsManager.Database.Entities
{
    public class ActionResponse
    {
        public Guid TaskId { get; set; }
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
        public static async Task<ActionResponse> FromHttpResponse(HttpResponseMessage response)
        {
            return new ActionResponse
            {

                Code = response.StatusCode,
                Message = await response.Content.ReadAsStringAsync(),
            };
        }
    }
}
