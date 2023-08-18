using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OperationsManager.Helpers;

namespace OperationsManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        [HttpGet]
        public ICollection<string> GetLogs(){
            return LogHelper.GetLogs();
        }
    }
}
