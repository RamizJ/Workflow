using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API аутентификации пользователей в системе
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        /// <summary>
        /// Constructor
        /// </summary>ы
        /// <param name="service"></param>
        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Вход в систему
        /// </summary>
        /// <param name="input">Данные аутентификации</param>
        /// <returns>JWT-токен и данные пользователя</returns>
        [HttpPost]
        public async Task<ActionResult<VmAuthOutput>> Login(VmAuthInput input)
        {
            var output = await _service.Login(input);
            if (output == null)
                return Unauthorized();

            return Ok(output);
        }

        /// <summary>
        /// Выход пользователя из системы
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
