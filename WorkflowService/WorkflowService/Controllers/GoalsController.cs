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
    /// API-методы работы с задачами
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class GoalsController : ControllerBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public GoalsController(UserManager<ApplicationUser> userManager, IGoalsService goalsService)
        {
            _userManager = userManager;
            _goalsService = goalsService;
        }

        /// <summary>
        /// Получить задачу по идентификатору, если она доступна пользователю
        /// </summary>
        /// <param name="id">Идентификатор задачи</param>
        /// <returns>Задача</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmGoal>> Get(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            return Ok(await _goalsService.Get(user, id));
        }

        /// <summary>
        /// Постраничная загрузка задач с фильтрацией и сортировкой
        /// </summary>
        /// <param name="scopeId">Идентификатор проекта</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтр по всем полям</param>
        /// <param name="filterFields">Конкретные поля фильтрации</param>
        /// <param name="sortFields">Поля сортировки</param>
        /// <param name="withRemoved">Вместе с удаленными</param>
        /// <returns>Коллеция задач</returns>
        [HttpGet]
        public async Task<IEnumerable<VmGoal>> GetPage([FromQuery]int scopeId, 
            [FromQuery]int pageNumber, [FromQuery]int pageSize, 
            [FromQuery]string filter = null, [FromQuery]FieldFilter[] filterFields = null, 
            [FromQuery]FieldSort[] sortFields = null, [FromQuery] bool withRemoved = false)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _goalsService.GetPage(currentUser, scopeId, 
                pageNumber, pageSize, filter, filterFields, sortFields, withRemoved);
        }

        /// <summary>
        /// Получение задач по идентификаторам. Возвращаются только задачи доступные пользователю
        /// </summary>
        /// <param name="ids">Идентификаторы задач</param>
        /// <returns>Коллекция задач</returns>
        [HttpGet]
        public async Task<IEnumerable<VmGoal>> GetRange([FromQuery]int[] ids)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _goalsService.GetRange(currentUser, ids);
        }

        /// <summary>
        /// Создание задачи
        /// </summary>
        /// <param name="goal">Создаваемая задача</param>
        /// <returns>Созданная задача</returns>
        [HttpPost]
        public async Task<ActionResult<VmGoal>> Create([FromBody]VmGoal goal)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _goalsService.Create(currentUser, goal);
        }

        /// <summary>
        /// Обновление задачи
        /// </summary>
        /// <param name="goal">Обновляемая задача</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]VmGoal goal)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            await _goalsService.Update(currentUser, goal);
            return NoContent();
        }

        /// <summary>
        /// Удаление задачи
        /// </summary>
        /// <param name="id">Идентификатор удаляемой задачи</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<VmGoal>> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var deletedScope = await _goalsService.Delete(currentUser, id);
            if (deletedScope == null)
                return NotFound();

            return Ok(deletedScope);
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGoalsService _goalsService;
    }
}