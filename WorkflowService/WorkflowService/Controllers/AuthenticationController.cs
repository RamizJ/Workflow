using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Login to the system
        /// </summary>
        /// <param name="input">Authentication data</param>
        /// <returns>Token and user info</returns>
        [HttpPost]
        public async Task<ActionResult<VmAuthOutput>> Login(VmAuthInput input)
        {
            var output = await _service.Login(input);
            if (output == null)
                return Unauthorized();

            return Ok(output);
        }

        /// <summary>
        /// Logout from the system
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _service.Logout();
            return Ok();
        }


        private readonly IAuthenticationService _service;
    }
}
