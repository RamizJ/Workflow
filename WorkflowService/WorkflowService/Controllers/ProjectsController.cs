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
    /// <summary>
    /// API-методы работы с проектами
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class ProjectsController : ControllerBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="projectsService"></param>
        public ProjectsController(UserManager<ApplicationUser> userManager, IProjectsService projectsService)
        {
            _userManager = userManager;
            _projectsService = projectsService;
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
        public async Task<ActionResult<VmProjectResult>> Create([FromBody] VmProject project)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _projectsService.Create(currentUser, project);
        }

        /// <summary>
        /// Обновление проектов
        /// </summary>
        /// <param name="project">Updated project</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<VmProjectResult>> Update([FromBody]VmProject project)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var updatedScope = await _projectsService.Update(currentUser, project);
            if (updatedScope == null)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Удаление проектов
        /// </summary>
        /// <param name="id">Идентификатор проекта</param>
        /// <returns>Результат выполнения операции</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<VmProjectResult>> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var deletedScope = await _projectsService.Delete(currentUser, id);
            if (deletedScope == null)
                return NotFound();

            return Ok(deletedScope);
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProjectsService _projectsService;
    }
}