using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    [ApiController, Route("api/[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<IActionResult> Login(VmAuthInput input)
        {
            var output = await _service.Login(input);
            if (output == null)
                return Unauthorized();

            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _service.Logout();
            return Ok();
        }


        private readonly IAuthenticationService _service;
    }
}
