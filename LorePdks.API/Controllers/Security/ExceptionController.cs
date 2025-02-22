using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;

namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/Exception")]
    [ApiController]
    public class ExceptionController : ControllerBase
    {

        [HttpPost]
        [Route("OYSExceptionCacher")]
        public IActionResult OYSExceptionCacher(AppException request)
        {
            var response = new ServiceResponse<object>(request);
            return Ok(response);

        }
    }
}
