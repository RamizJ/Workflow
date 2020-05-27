using System;
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
    /// API работы с пользователями системы
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsersService _usersService;

        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="usersService"></param>
        public UsersController(UserManager<ApplicationUser> userManager, IUsersService usersService)
        {
            _userManager = userManager;
            _usersService = usersService;
        }

        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var users = await _usersService.GetUsers();
            return Ok(users);
        }

        /// <summary>
        /// Получение всех пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _usersService.GetUsers();
            return Ok(users);
        }

        /// <summary>
        /// Постраничная загрузка пользователей с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтрация по всем полям если не указаны конкретные поля для фильтрации</param>
        /// <param name="filterFields">Поля фильтрации</param>
        /// <param name="sort">Тип сортировки</param>
        /// <param name="sortFields">Поля сортировки. Сортировка производится по порядку указанных полей</param>
        /// <returns>Коллекция пользователей</returns>
        [HttpGet]
        public async Task<IActionResult> GetPage([FromQuery]int pageNumber, [FromQuery]int pageSize,
            [FromQuery]string filter, [FromQuery]string[] filterFields,
            [FromQuery]SortType sort, [FromQuery]string[] sortFields)
        {
            var users = await _usersService.GetUsers();
            return Ok(users);
        }

        /// <summary>
        /// Получение коллекции пользователей по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы пользователей</param>
        /// <returns></returns>
        [HttpGet]
        public Task<IEnumerable<VmUser>> GetRange([FromQuery]string[] ids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получение участников команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns>Коллекция участников команды</returns>
        [HttpGet]
        public Task<IEnumerable<VmUser>> GetTeamUsers(int teamId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Добавление нового пользователя
        /// </summary>
        /// <param name="user">Параметры пользователя</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<VmUser> Create([FromBody]VmUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Обновление пользователя
        /// </summary>
        /// <param name="user">Параметры пользователя</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Update(VmUser user)
        {
            return NoContent();
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<VmScope>> Delete(string id)
        {
            return BadRequest();
        }
    }
}