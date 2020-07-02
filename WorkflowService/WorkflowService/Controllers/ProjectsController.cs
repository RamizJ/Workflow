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
    /// API-методы работы с проектами
    /// </summary>
    //[Authorize]
    [ApiController, Route("api/[controller]/[action]")]
    public class ProjectsController : ControllerBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="projectsService"></param>
        /// <param name="projectTeamsService"></param>
        public ProjectsController(UserManager<ApplicationUser> userManager, 
            IProjectsService projectsService,
            IProjectTeamsService projectTeamsService)
        {
            _userManager = userManager;
            _projectsService = projectsService;
            _projectTeamsService = projectTeamsService;
        }


        /// <summary>
        /// Получение проекта по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор проекта</param>
        /// <returns>Проект. Только если доступен пользователю</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmProject>> Get(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return Ok(await _projectsService.Get(currentUser, id));
        }

        /// <summary>
        /// Постраничная загрузка проектов с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтр по всем полям</param>
        /// <param name="filterFields">Конкретные поля фильтрации</param>
        /// <param name="sortFields">Поля сортировки</param>
        /// <param name="withRemoved">Вместе с удаленными</param>
        /// <returns>Коллекция проектов</returns>
        [HttpGet]
        public async Task<IEnumerable<VmProject>> GetPage([FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] string filter = null, [FromQuery] FieldFilter[] filterFields = null, 
            [FromQuery] FieldSort[] sortFields = null, [FromQuery]bool withRemoved = false)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _projectsService.GetPage(currentUser, pageNumber, pageSize,
                filter, filterFields, sortFields, withRemoved);
        }

        /// <summary>
        /// Постраничная загрузка списка команд проекта с фильтрацией и сортировкой
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтр по всем полям</param>
        /// <param name="filterFields">Конкретные поля фильтрации</param>
        /// <param name="sortFields">Поля сортировки</param>
        /// <param name="withRemoved">Вместе с удаленными</param>
        /// <returns>Коллекция команд</returns>
        [HttpGet]
        public async Task<IEnumerable<VmTeam>> GetTeamsPage([FromQuery] int projectId,
            [FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] string filter = null, [FromQuery] FieldFilter[] filterFields = null,
            [FromQuery] FieldSort[] sortFields = null, [FromQuery]bool withRemoved = false)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _projectTeamsService.GetPage(currentUser, projectId, 
                pageNumber, pageSize, filter, filterFields, sortFields, withRemoved);
        }


        /// <summary>
        /// Получение проектов по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы проектов</param>
        /// <returns>Коллекция проектов</returns>
        [HttpGet]
        public async Task<IEnumerable<VmProject>> GetRange(int[] ids)
        {
            var currentUser = await _userManager.GetUserAsync(User);
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
            var currentUser = await _userManager.GetUserAsync(User);
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
            var currentUser = await _userManager.GetUserAsync(User);
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
            var currentUser = await _userManager.GetUserAsync(User);
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
            var currentUser = await _userManager.GetUserAsync(User);
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
            var currentUser = await _userManager.GetUserAsync(User);
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
            var currentUser = await _userManager.GetUserAsync(User);
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
            var currentUser = await _userManager.GetUserAsync(User);
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
            var currentUser = await _userManager.GetUserAsync(User);
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
            var currentUser = await _userManager.GetUserAsync(User);
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
            var currentUser = await _userManager.GetUserAsync(User);
            var projects = await _projectsService.RestoreRange(currentUser, ids);
            return Ok(projects);
        }



        /// <summary>
        /// Добавление команды в список команд проекта
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns></returns>
        [HttpPatch("{teamId}")]
        public async Task<IActionResult> AddTeam(int projectId, [FromBody] int teamId)
        {
            await _projectTeamsService.Add(projectId, teamId);
            return NoContent();
        }

        /// <summary>
        /// Удаление команды из списка команд проекта
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns></returns>
        [HttpPatch("{teamId}/{projectId}")]
        public async Task<IActionResult> RemoveTeam(int projectId, int teamId)
        {
            await _projectTeamsService.Remove(projectId, teamId);
            return NoContent();
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProjectsService _projectsService;
        private readonly IProjectTeamsService _projectTeamsService;
    }
}