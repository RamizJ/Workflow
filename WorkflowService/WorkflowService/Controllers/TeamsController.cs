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
    /// API-методы работы с командами
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class TeamsController : ControllerBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="teamsService"></param>
        public TeamsController(UserManager<ApplicationUser> userManager, ITeamsService teamsService)
        {
            _userManager = userManager;
            _teamsService = teamsService;
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
        /// <param name="team">Новая команда</param>
        /// <returns>Команда</returns>
        [HttpPost]
        public async Task<ActionResult<VmTeam>> Create([FromBody]VmTeam team)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _teamsService.Create(currentUser, team);
        }

        /// <summary>
        /// Обновление команды
        /// </summary>
        /// <param name="team">Команда</param>
        /// <returns>NotFound(404) - если команда не найдена. NoContent(204) - если обновление прошло успешно</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]VmTeam team)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var updatedScope = await _teamsService.Update(currentUser, team);
            if (updatedScope == null)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Удаление команды
        /// </summary>
        /// <param name="id">Идентификатор команды</param>
        /// <returns>Удаленная команда</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<VmTeam>> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var deletedScope = await _teamsService.Delete(currentUser, id);
            if (deletedScope == null)
                return NotFound();

            return Ok(deletedScope);
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITeamsService _teamsService;
    }
}