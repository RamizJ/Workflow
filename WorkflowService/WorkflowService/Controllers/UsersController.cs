using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PageLoading;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API работы с пользователями системы
    /// </summary>
    //[Authorize]
    [ApiController, Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="currentUserService"></param>
        public UsersController(IUsersService service, ICurrentUserService currentUserService)
        {
            _service = service;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmUser>> Get(string id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var user = await _service.Get(currentUser, id);
            return Ok(user);
        }

        /// <summary>
        /// Получение параметров текущего пользователя
        /// </summary>
        /// <returns>Параметры текущего пользователя</returns>
        [HttpGet]
        public async Task<ActionResult<VmUser>> GetCurrent()
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var user = await _service.GetCurrent(currentUser);
            return Ok(user);
        }

        /// <summary>
        /// Постраничная загрузка пользователей с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns>Коллекция пользователей</returns>
        [HttpPost]
        public async Task<IEnumerable<VmUser>> GetPage([FromBody]PageOptions pageOptions)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var users = await _service.GetPage(currentUser, pageOptions);
            return users;
        }

        /// <summary>
        /// Получение коллекции пользователей по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы пользователей</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<VmUser>> GetRange([FromQuery]string[] ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _service.GetRange(currentUser, ids);
        }

        /// <summary>
        /// Проверка существования пользователя с указанным email
        /// </summary>
        /// <param name="email">Email пользователя</param>
        /// <returns></returns>
        [HttpGet("{email}")]
        public async Task<ActionResult<bool>> IsEmailExist([FromQuery]string email)
        {
            var isExist = await _service.IsEmailExist(email);
            return Ok(isExist);
        }

        /// <summary>
        /// Проверка существования пользователя с указанным именем пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        [HttpGet("{userName}")]
        public async Task<ActionResult<bool>> IsUserNameExist(string userName)
        {
            var isExist = await _service.IsUserNameExist(userName);
            return Ok(isExist);
        }

        /// <summary>
        /// Добавление нового пользователя
        /// </summary>
        /// <param name="user">Параметры пользователя. Пароль должен содержать не менее 6 символов, включая цифры</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<VmUser>> Create([FromBody] VmNewUser user)
        {
            var result = await _service.Create(user, user.Password);
            return Ok(result);
        }

        /// <summary>
        /// Обновление пользователя
        /// </summary>
        /// <param name="user">Параметры пользователя</param>
        /// <returns>Результат</returns>
        [HttpPut]
        public async Task<IActionResult> Update(VmUser user)
        {
            await _service.Update(user);
            return NoContent();
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<VmUser>> Delete(string id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var result = await _service.Delete(currentUser, id);
            return Ok(result);
        }

        /// <summary>
        /// Удаление пользователей
        /// </summary>
        /// <param name="ids">Идентификаторы пользователей</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<VmUser>> DeleteRange([FromBody] IEnumerable<string> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var users = await _service.DeleteRange(currentUser, ids);
            return Ok(users);
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<VmUser>> Restore(string id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var user = await _service.Restore(currentUser, id);
            return Ok(user);
        }

        /// <summary>
        /// Восстановление пользователей
        /// </summary>
        /// <param name="ids">Идентификаторы пользователей</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<VmUser>> RestoreRange([FromBody] IEnumerable<string> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var users = await _service.RestoreRange(currentUser, ids);
            return Ok(users);
        }

        /// <summary>
        /// Восстановление пользователей
        /// </summary>
        /// <param name="ids">Идентификаторы пользователей</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<VmUser>> Restore(IEnumerable<string> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var user = await _service.RestoreRange(currentUser, ids);
            return Ok(user);
        }

        /// <summary>
        /// Изменение пароля пользователя. Доступно только текущему пользователю
        /// </summary>
        /// <param name="currentPassword">Текущий пароль</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> ChangePassword([FromQuery]string currentPassword, [FromQuery] string newPassword)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _service.ChangePassword(currentUser, currentPassword, newPassword);
            return NoContent();
        }

        /// <summary>
        /// Сброс пароля пользователя. Доступно только администраторам
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <returns></returns>
        [HttpPatch("{id}"), Authorize(Roles = RoleNames.ADMINISTRATOR_ROLE)]
        public async Task<IActionResult> ResetPassword(string id, [FromQuery] string newPassword)
        {
            await _service.ResetPassword(id, newPassword);
            return NoContent();
        }



        private readonly IUsersService _service;
        private readonly ICurrentUserService _currentUserService;
    }
}