using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Common;
using Workflow.VM.ViewModels;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API-методы работы с командами
    /// </summary>
    //[Authorize]
    [ApiController, Route("api/[controller]/[action]")]
    public class TeamsController : ControllerBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="teamsService"></param>
        /// <param name="teamUsersService"></param>
        /// <param name="teamProjectsService"></param>
        public TeamsController(UserManager<ApplicationUser> userManager, 
            ITeamsService teamsService, ITeamUsersService teamUsersService,
            ITeamProjectsService teamProjectsService)
        {
            _userManager = userManager;
            _teamsService = teamsService;
            _teamUsersService = teamUsersService;
            _teamProjectsService = teamProjectsService;
        }

        /// <summary>
        /// Получить команду по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор команды</param>
        /// <returns>Команда</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmTeam>> Get(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return Ok(await _teamsService.Get(currentUser, id));
        }

        /// <summary>
        /// Постраничная загрузка команд с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтр по всем полям</param>
        /// <param name="filterFields">Поля для фильтрации</param>
        /// <param name="sortFields">Поля для сортировки</param>
        /// <param name="withRemoved">Вместе с удаленными командами</param>
        /// <returns>Коллекция команд</returns>
        [HttpGet]
        public async Task<IEnumerable<VmTeam>> GetPage([FromQuery]int pageNumber, [FromQuery]int pageSize,
            [FromQuery]string filter = null, [FromQuery]FieldFilter[] filterFields = null, 
            [FromQuery]FieldSort[] sortFields = null, [FromQuery] bool withRemoved = false)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _teamsService.GetPage(currentUser, pageNumber, pageSize,
                filter, filterFields, sortFields, withRemoved);
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
        public async Task<IEnumerable<VmUser>> GetUsersPage([FromQuery] int teamId,
            [FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] string filter, [FromQuery] FieldFilter[] filterFields,
            [FromQuery] FieldSort[] sortFields, [FromQuery] bool withRemoved = false)
        {
            var user = await _userManager.GetUserAsync(User);
            return await _teamUsersService.GetPage(user, teamId, pageNumber, pageSize, 
                filter, filterFields, sortFields, withRemoved);
        }

        /// <summary>
        /// Постраничная загрузка проектов команды с фильтрацией и сортировкой
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
        public async Task<IEnumerable<VmProject>> GetProjectsPage([FromQuery] int teamId,
            [FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] string filter, [FromQuery] FieldFilter[] filterFields,
            [FromQuery] FieldSort[] sortFields, [FromQuery] bool withRemoved = false)
        {
            var user = await _userManager.GetUserAsync(User);
            return await _teamProjectsService.GetPage(user, teamId, 
                pageNumber, pageSize, filter, filterFields, sortFields, withRemoved);
        }



        /// <summary>
        /// Получить команд по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы команд</param>
        /// <returns>Коллецция команд</returns>
        [HttpGet]
        public async Task<IEnumerable<VmTeam>> GetRange([FromQuery]int[] ids)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _teamsService.GetRange(currentUser, ids);
        }


        /// <summary>
        /// Создать команду
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="team">Новая команда</param>
        /// <returns>Команда</returns>
        [HttpPost("{projectId}")]
        public async Task<ActionResult<VmTeam>> Create(int projectId, [FromBody]VmTeam team)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _teamsService.Create(currentUser, projectId, team);
            return Ok(result);
        }

        /// <summary>
        /// Обновление команды
        /// </summary>
        /// <param name="team">Команда</param>
        /// <returns>Результат выполнения операции</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]VmTeam team)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            await _teamsService.Update(currentUser, team);
            return NoContent();
        }

        /// <summary>
        /// Удаление команды
        /// </summary>
        /// <param name="id">Идентификатор команды</param>
        /// <returns>Результат выполнения операции</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<VmTeam>> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _teamsService.Delete(currentUser, id);
            return Ok(result);
        }


        /// <summary>
        /// Добавление пользователя в команду
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="userId">Идентификатор добавляемого пользователя</param>
        /// <returns></returns>
        [HttpPatch("{teamId}")]
        public async Task<IActionResult> AddUser(int teamId, [FromBody] string userId)
        {
            await _teamUsersService.Add(teamId, userId);
            return NoContent();
        }

        /// <summary>
        /// Удаление пользователя из команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Удаленный пользователь</returns>
        [HttpPatch("{teamId}/{userId}")]
        public async Task<IActionResult> RemoveUser(int teamId, string userId)
        {
            await _teamUsersService.Remove(teamId, userId);
            return NoContent();
        }


        /// <summary>
        /// Добавление проекта в список проектов команды
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <returns></returns>
        [HttpPatch("{teamId}")]
        public async Task<IActionResult> AddProject(int teamId, [FromBody] int projectId)
        {
            await _teamProjectsService.Add(teamId, projectId);
            return NoContent();
        }

        /// <summary>
        /// Удаление проекта из списка проектов команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <returns></returns>
        [HttpPatch("{teamId}/{projectId}")]
        public async Task<IActionResult> RemoveProject(int teamId, int projectId)
        {
            await _teamProjectsService.Remove(teamId, projectId);
            return NoContent();
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITeamsService _teamsService;
        private readonly ITeamUsersService _teamUsersService;
        private readonly ITeamProjectsService _teamProjectsService;
    }
}