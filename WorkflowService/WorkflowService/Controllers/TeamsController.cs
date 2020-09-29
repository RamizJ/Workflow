using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;

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
        /// <param name="currentUserService"></param>
        /// <param name="teamsService"></param>
        /// <param name="teamUsersService"></param>
        /// <param name="teamProjectsService"></param>
        public TeamsController(ICurrentUserService currentUserService,
            ITeamsService teamsService, ITeamUsersService teamUsersService,
            ITeamProjectsService teamProjectsService)
        {
            _currentUserService = currentUserService;
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return Ok(await _teamsService.Get(currentUser, id));
        }

        /// <summary>
        /// Постраничная загрузка команд с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns>Коллекция команд</returns>
        [HttpPost]
        public async Task<IEnumerable<VmTeam>> GetPage([FromBody] PageOptions pageOptions)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _teamsService.GetPage(currentUser, pageOptions);
        }


        /// <summary>
        /// Постраничная загрузка пользователей команды с фильтрацией и сортировкой
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns>Коллекция пользователей команды</returns>
        [HttpPost]
        public async Task<IEnumerable<VmUser>> GetUsersPage([FromQuery] int teamId,
            [FromBody]PageOptions pageOptions)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _teamUsersService.GetPage(currentUser, teamId, pageOptions);
        }

        /// <summary>
        /// Постраничная загрузка проектов команды с фильтрацией и сортировкой
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="pageOptions"></param>
        /// <returns>Коллекция пользователей команды</returns>
        [HttpPost]
        public async Task<IEnumerable<VmProject>> GetProjectsPage([FromQuery] int teamId, 
            [FromBody]PageOptions pageOptions)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _teamProjectsService.GetPage(currentUser, teamId, pageOptions);
        }



        /// <summary>
        /// Получить команд по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы команд</param>
        /// <returns>Коллецция команд</returns>
        [HttpGet]
        public async Task<IEnumerable<VmTeam>> GetRange([FromQuery]int[] ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _teamsService.GetRange(currentUser, ids);
        }


        /// <summary>
        /// Создать команду
        /// </summary>
        /// <param name="team">Новая команда</param>
        /// <returns>Команда</returns>
        [HttpPost]
        public async Task<ActionResult<VmTeam>> Create([FromBody]VmTeam team)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var result = await _teamsService.Create(currentUser, team);
            return Ok(result);
        }

        /// <summary>
        /// Создать команду по форме
        /// </summary>
        /// <param name="teamForm">Форма команды</param>
        /// <returns>Команда</returns>
        [HttpPost]
        public async Task<ActionResult<VmTeam>> CreateByForm([FromBody] VmTeamForm teamForm)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var result = await _teamsService.CreateByForm(currentUser, teamForm);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _teamsService.Update(currentUser, team);
            return NoContent();
        }

        /// <summary>
        /// Обновление команды по форме
        /// </summary>
        /// <param name="teamForm">Форма команды</param>
        /// <returns>Результат выполнения операции</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateByForm([FromBody] VmTeamForm teamForm)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _teamsService.UpdateByForm(currentUser, teamForm);
            return NoContent();
        }

        /// <summary>
        /// Обновление команд
        /// </summary>
        /// <param name="teams">Обновляемые команды</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<VmTeam> teams)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _teamsService.UpdateRange(currentUser, teams);
            return NoContent();
        }

        /// <summary>
        /// Обновление команд по формам
        /// </summary>
        /// <param name="teamForms">Формы команд</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateByFormRange([FromBody] IEnumerable<VmTeamForm> teamForms)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _teamsService.UpdateByFormRange(currentUser, teamForms);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var result = await _teamsService.Delete(currentUser, id);
            return Ok(result);
        }

        /// <summary>
        /// Восстановление команды
        /// </summary>
        /// <param name="id">Идентификатор команды</param>
        /// <returns>Результат выполнения операции</returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<VmTeam>> Restore(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var result = await _teamsService.Restore(currentUser, id);
            return Ok(result);
        }

        /// <summary>
        /// Удаление команд
        /// </summary>
        /// <param name="ids">Идентификаторы команд</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> DeleteRange([FromBody] IEnumerable<int> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var teams = await _teamsService.DeleteRange(currentUser, ids);
            return Ok(teams);
        }

        /// <summary>
        /// Восстановление команд
        /// </summary>
        /// <param name="ids">Идентификаторы команд</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> RestoreRange([FromBody] IEnumerable<int> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var teams = await _teamsService.RestoreRange(currentUser, ids);
            return Ok(teams);
        }


        /// <summary>
        /// Добавление пользователя в команду
        /// </summary>
        /// <param name="teamId">Идентификатор команнды</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpPatch("{teamId}")]
        public async Task<IActionResult> AddUser(int teamId, [FromBody]string userId)
        {
            await _teamUsersService.Add(teamId, userId);
            return NoContent();
        }

        /// <summary>
        /// Добавление пользователей в команду
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="userIds">Идентификаторы пользователей</param>
        /// <returns></returns>
        [HttpPatch("{teamId}")]
        public async Task<IActionResult> AddUsers(int teamId, [FromBody] IEnumerable<string> userIds)
        {
            await _teamUsersService.AddRange(teamId, userIds);
            return NoContent();
        }

        /// <summary>
        /// Удаление пользователя из команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Удаленный пользователь</returns>
        [HttpPatch("{teamId}")]
        public async Task<IActionResult> RemoveUser(int teamId, [FromBody] string userId)
        {
            await _teamUsersService.Remove(teamId, userId);
            return NoContent();
        }


        /// <summary>
        /// Удаление пользователя из команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="userIds">Идентификаторы пользователей</param>
        /// <returns>Удаленный пользователь</returns>
        [HttpPatch("{teamId}")]
        public async Task<IActionResult> RemoveUsers(int teamId, [FromBody] IEnumerable<string> userIds)
        {
            await _teamUsersService.RemoveRange(teamId, userIds);
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


        private readonly ICurrentUserService _currentUserService;
        private readonly ITeamsService _teamsService;
        private readonly ITeamUsersService _teamUsersService;
        private readonly ITeamProjectsService _teamProjectsService;
    }
}