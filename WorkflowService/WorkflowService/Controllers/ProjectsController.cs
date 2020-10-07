using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API-методы работы с проектами
    /// </summary>
    [Authorize]
    [ApiController, Route("api/[controller]/[action]")]
    public class ProjectsController : ControllerBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="currentUserService"></param>
        /// <param name="projectsService"></param>
        /// <param name="teamsService"></param>
        /// <param name="userRolesService"></param>
        /// <param name="statisticService"></param>
        public ProjectsController(ICurrentUserService currentUserService, 
            IProjectsService projectsService,
            IProjectTeamsService teamsService,
            IProjectUserRolesService userRolesService,
            IStatisticService statisticService)
        {
            _currentUserService = currentUserService;
            _projectsService = projectsService;
            _teamsService = teamsService;
            _userRolesService = userRolesService;
            _statisticService = statisticService;
        }


        /// <summary>
        /// Получение проекта по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор проекта</param>
        /// <returns>Проект. Только если доступен пользователю</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmProject>> Get(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return Ok(await _projectsService.Get(currentUser, id));
        }

        /// <summary>
        /// Постраничная загрузка проектов с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns>Коллекция проектов</returns>
        [HttpPost]
        public async Task<IEnumerable<VmProject>> GetPage([FromBody] PageOptions pageOptions)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _projectsService.GetPage(currentUser, pageOptions);
        }

        /// <summary>
        /// Постраничная загрузка списка команд проекта с фильтрацией и сортировкой
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns>Коллекция команд</returns>
        [HttpPost]
        public async Task<IEnumerable<VmTeam>> GetTeamsPage([FromQuery]int projectId, 
            [FromBody]PageOptions pageOptions)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _teamsService.GetPage(currentUser, projectId, pageOptions);
        }

        /// <summary>
        /// Получение 
        /// </summary>
        /// <param name="ids">Идентификаторы проектов</param>
        /// <returns>Коллекция проектов</returns>
        [HttpGet]
        public async Task<IEnumerable<VmProject>> GetRange(int[] ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _projectsService.GetRange(currentUser, ids);
        }

        /// <summary>
        /// Создание проекта
        /// </summary>
        /// <param name="project">Проект</param>
        /// <returns>Результат выполнения операции</returns>
        [HttpPost]
        public async Task<ActionResult<VmProject>> Create([FromBody] VmProject project)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var result = await _projectsService.Create(currentUser, project);
            return Ok(result);
        }

        /// <summary>
        /// Создание проекта по форме
        /// </summary>
        /// <param name="projectForm">Форма создаваемого проекта</param>
        /// <returns>Результат выполнения операции</returns>
        [HttpPost]
        public async Task<ActionResult<VmProject>> CreateByForm([FromBody] VmProjectForm projectForm)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var result = await _projectsService.CreateByForm(currentUser, projectForm);
            return Ok(result);
        }

        /// <summary>
        /// Обновление проекта
        /// </summary>
        /// <param name="project">Обновляемый проект</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]VmProject project)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _projectsService.Update(currentUser, project);
            return NoContent();
        }

        /// <summary>
        /// Обновление проекта по форме
        /// </summary>
        /// <param name="projectForm">Форма обновляемого проекта</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateByForm([FromBody] VmProjectForm projectForm)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _projectsService.UpdateByForm(currentUser, projectForm);
            return NoContent();
        }

        /// <summary>
        /// Обновление проектов
        /// </summary>
        /// <param name="projects">Обновляемый проект</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<VmProject> projects)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _projectsService.UpdateRange(currentUser, projects);
            return NoContent();
        }

        /// <summary>
        /// Обновление проектов по формам
        /// </summary>
        /// <param name="projectForms">Формы обновляемых проектов</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<VmProjectForm> projectForms)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _projectsService.UpdateByFormRange(currentUser, projectForms);
            return NoContent();
        }

        /// <summary>
        /// Удаление проекта
        /// </summary>
        /// <param name="id">Идентификатор проекта</param>
        /// <returns>Результат выполнения операции</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<VmProject>> Delete(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var result = await _projectsService.Delete(currentUser, id);
            return Ok(result);
        }

        /// <summary>
        /// Удаление проектов
        /// </summary>
        /// <param name="ids">Идентификаторы проектов</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> DeleteRange([FromBody]IEnumerable<int> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var projects = await _projectsService.DeleteRange(currentUser, ids);
            return Ok(projects);
        }

        /// <summary>
        /// Восстановление проекта
        /// </summary>
        /// <param name="id">Идентификатор проекта</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<VmProject>> Restore(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var result = await _projectsService.Restore(currentUser, id);
            return Ok(result);
        }

        /// <summary>
        /// Восстановление проектов
        /// </summary>
        /// <param name="ids">Идентификаторы проектов</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> RestoreRange([FromBody] IEnumerable<int> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var projects = await _projectsService.RestoreRange(currentUser, ids);
            return Ok(projects);
        }



        /// <summary>
        /// Добавление команды в список команд проекта
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns></returns>
        [HttpPatch("{projectId}/{teamId}")]
        public async Task<IActionResult> AddTeam(int projectId, int teamId)
        {
            await _teamsService.Add(projectId, teamId);
            return NoContent();
        }

        /// <summary>
        /// Удаление команды из списка команд проекта
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns></returns>
        [HttpPatch("{projectId}/{teamId}")]
        public async Task<IActionResult> RemoveTeam(int projectId, int teamId)
        {
            await _teamsService.Remove(projectId, teamId);
            return NoContent();
        }

        /// <summary>
        /// Получение ролей команды в проекте
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("{projectId}/{teamId}")]
        public async Task<ActionResult<VmProjectTeamRole>> GetTeamRole(int projectId, int teamId)
        {
            var teamRole = await _teamsService.GetRole(projectId, teamId);
            return Ok(teamRole);
        }

        /// <summary>
        /// Обновление ролей команды в проекте
        /// </summary>
        /// <param name="projectTeamRole">Роли команды в проекте</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<NoContentResult> UpdateTeamRole([FromBody]VmProjectTeamRole projectTeamRole)
        {
            await _teamsService.UpdateTeamRole(projectTeamRole);
            return NoContent();
        }

        /// <summary>
        /// Обновление ролей команд в проектах
        /// </summary>
        /// <param name="roles">Роли команд в проектах</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<NoContentResult> UpdateTeamsRoles([FromBody] IEnumerable<VmProjectTeamRole> roles)
        {
            await _teamsService.UpdateTeamsRoles(roles);
            return NoContent();
        }

        /// <summary>
        /// Получение ролей пользователя в проекте
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet("{projectId}/{userId}")] 
        public async Task<ActionResult<VmProjectUserRole>> GetUserRole(int projectId, string userId)
        {
            var userRole = await _userRolesService.Get(projectId, userId);
            return Ok(userRole);
        }

        /// <summary>
        /// Получение ролей пользователей команды в проекте
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns></returns>
        [HttpGet("{projectId}/{teamId}")]
        public async Task<IEnumerable<VmProjectUserRole>> GetUsersRoles(int projectId, int teamId)
        {
            return await _userRolesService.GetForTeam(projectId, teamId);
        }

        /// <summary>
        /// Обновление ролей пользователя
        /// </summary>
        /// <param name="userRole">Роли пользователя в проекте</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<NoContentResult> UpdateUserRole([FromBody]VmProjectUserRole userRole)
        {
            await _userRolesService.Update(userRole);
            return NoContent();
        }

        /// <summary>
        /// Обновление ролей пользователей
        /// </summary>
        /// <param name="userRoles">Роли пользователей в проекте</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<NoContentResult> UpdateUsersRoles([FromBody] IEnumerable<VmProjectUserRole> userRoles)
        {
            await _userRolesService.UpdateRange(userRoles);
            return NoContent();
        }


        /// <summary>
        /// Получение статистики по проекту
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="options">Параметры статистики</param>
        /// <returns></returns>
        [HttpPost("{projectId}")]
        public async Task<ActionResult<ProjectStatistic>> GetProjectStatistic(int projectId, 
            [FromBody] ProjectStatisticOptions options)
        {
            var statistic = await _statisticService.GetStatistic(projectId, options);
            return Ok(statistic);
        }


        private readonly ICurrentUserService _currentUserService;
        private readonly IProjectsService _projectsService;
        private readonly IProjectTeamsService _teamsService;
        private readonly IProjectUserRolesService _userRolesService;
        private readonly IStatisticService _statisticService;
    }
}