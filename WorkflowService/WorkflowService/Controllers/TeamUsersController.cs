using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using WorkflowService.Common;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamUsersController : ControllerBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="teamsService"></param>
        public TeamUsersController(UserManager<ApplicationUser> userManager, ITeamUsersService teamsService)
        {
            _userManager = userManager;
            _teamUsersService = teamsService;
        }


        /// <summary>
        /// Постраничная загрузка пользователей команды с фильтрацией и сортировкой
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтр по всем полям</param>
        /// <param name="filterFields">Конкретные поля фильтрации</param>
        /// <param name="sortFields">Поля сортировки</param>
        /// <param name="withRemoved">Вместе с удаленными</param>
        /// <returns>Коллекция пользователей команды</returns>
        [HttpGet]
        public async Task<IEnumerable<VmUser>> GetPage([FromQuery] int teamId, 
            [FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] string filter, [FromQuery] FieldFilter[] filterFields,
            [FromQuery] FieldSort[] sortFields, [FromQuery] bool withRemoved = false)
        {
            var user = await _userManager.GetUserAsync(User);
            return await _teamUsersService.GetPage(user, teamId, pageNumber, pageSize,
                filter, filterFields, sortFields, withRemoved);
        }

        /// <summary>
        /// Добавление пользователя в команду
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="userId">Идентификатор добавляемого пользователя</param>
        /// <returns>Добавленный пользователь</returns>
        [HttpPost("{teamId}")]
        public async Task<IActionResult> Add(int teamId, [FromBody] string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            await _teamUsersService.Add(currentUser, teamId, userId);
            return NoContent();
        }

        /// <summary>
        /// Удаление пользователя из команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Удаленный пользователь</returns>
        [HttpDelete("{teamId/userId}")]
        public async Task<IActionResult> Remove(int teamId, string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            await _teamUsersService.Remove(currentUser, teamId, userId);
            return NoContent();
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITeamUsersService _teamUsersService;
    }
}
