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
    /// API-методы работы с множествами задач
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class ScopesController : ControllerBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="scopesService"></param>
        public ScopesController(UserManager<ApplicationUser> userManager, IScopesService scopesService)
        {
            _userManager = userManager;
            _scopesService = scopesService;
        }


        /// <summary>
        /// Получение множества задач по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор области задач</param>
        /// <returns>Множество задач. Только если доступно пользователю</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmScope>> Get(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return Ok(await _scopesService.Get(currentUser, id));
        }

        /// <summary>
        /// Постраничная загрузка множеств задач с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтр по всем полям</param>
        /// <param name="filterFields">Конкретные поля фильтрации</param>
        /// <param name="sortFields">Поля сортировки</param>
        /// <param name="withRemoved">Вместе с удаленными</param>
        /// <returns>Коллекция множеств задач</returns>
        [HttpGet]
        public async Task<IEnumerable<VmScope>> GetPage([FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] string filter = null, [FromQuery] FieldFilter[] filterFields = null, 
            [FromQuery] FieldSort[] sortFields = null, [FromQuery]bool withRemoved = false)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _scopesService.GetPage(currentUser, pageNumber, pageSize,
                filter, filterFields, sortFields, withRemoved);
        }

        /// <summary>
        /// Получение множеств задач по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы множеств</param>
        /// <returns>Коллекция множеств</returns>
        [HttpGet]
        public async Task<IEnumerable<VmScope>> GetRange(int[] ids)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _scopesService.GetRange(currentUser, ids);
        }

        /// <summary>
        /// Создание множества задач
        /// </summary>
        /// <param name="scope">Параметры множества</param>
        /// <returns>Созданное множество</returns>
        [HttpPost]
        public async Task<ActionResult<VmScope>> Create([FromBody] VmScope scope)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _scopesService.Create(currentUser, scope);
        }

        /// <summary>
        /// Обновление множества задач
        /// </summary>
        /// <param name="scope">Updated scope</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]VmScope scope)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var updatedScope = await _scopesService.Update(currentUser, scope);
            if (updatedScope == null)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Удаление множества задач
        /// </summary>
        /// <param name="id">Идентификатор множества</param>
        /// <returns>Удаленное множество</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<VmScope>> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var deletedScope = await _scopesService.Delete(currentUser, id);
            if (deletedScope == null)
                return NotFound();

            return Ok(deletedScope);
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IScopesService _scopesService;
    }
}