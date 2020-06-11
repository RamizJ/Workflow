﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Common;
using Workflow.VM.ViewModels;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API работы с пользователями системы
    /// </summary>
    [Authorize]
    [ApiController, Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsersService _service;

        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="service"></param>
        public UsersController(UserManager<ApplicationUser> userManager, IUsersService service)
        {
            _userManager = userManager;
            _service = service;
        }

        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmUser>> Get(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var user = await _service.Get(currentUser, id);
            return Ok(user);
        }

        /// <summary>
        /// Получение параметров текущего пользователя
        /// </summary>
        /// <returns>Параметры текущего пользователя</returns>
        [HttpGet]
        public async Task<ActionResult<VmUser>> GetCurrent()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var user = await _service.GetCurrent(currentUser);
            return Ok(user);
        }

        /// <summary>
        /// Постраничная загрузка пользователей с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтрация по всем полям если не указаны конкретные поля для фильтрации</param>
        /// <param name="filterFields">Поля фильтрации</param>
        /// <param name="sortFields">Поля сортировки. Сортировка производится по порядку указанных полей</param>
        /// <param name="withRemoved">Вместе с удаленными</param>
        /// <returns>Коллекция пользователей</returns>
        [HttpGet]
        public async Task<IEnumerable<VmUser>> GetPage([FromQuery]int pageNumber, [FromQuery]int pageSize,
            [FromQuery]string filter, [FromQuery]FieldFilter[] filterFields, [FromQuery]FieldSort[] sortFields,
            bool withRemoved = false)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var users = await _service.GetPage(currentUser, pageNumber, pageSize, 
                filter, filterFields, sortFields, withRemoved);
            return users;
        }

        /// <summary>
        /// Получение коллекции пользователей по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы пользователей</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<VmUser>> GetRange([FromQuery]string[] ids)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _service.GetRange(currentUser, ids);
        }

        /// <summary>
        /// Добавление нового пользователя
        /// </summary>
        /// <param name="user">Параметры пользователя. Пароль должен содержать не менее 6 символов, включая цифры</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<VmUser>> Create([FromBody] VmNewUser user)
        {
            var result = await _service.Create(user, user.Password);
            return Ok(result);
        }

        /// <summary>
        /// Обновление пользователя
        /// </summary>
        /// <param name="user">Параметры пользователя</param>
        /// <returns>Результат</returns>
        [HttpPut]
        public async Task<IActionResult> Update(VmUser user)
        {
            await _service.Update(user);
            return NoContent();
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<VmUser>> Delete(string id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }


        /// <summary>
        /// Изменение пароля пользователя. Доступно только текущему пользователю
        /// </summary>
        /// <param name="currentPassword">Текущий пароль</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> ChangePassword([FromQuery]string currentPassword, [FromQuery] string newPassword)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            await _service.ChangePassword(currentUser, currentPassword, newPassword);
            return NoContent();
        }

        /// <summary>
        /// Сброс пароля пользователя. Доступно только администраторам
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <returns></returns>
        [HttpPatch("{id}"), Authorize(Roles = RoleNames.ADMINISTRATOR_ROLE)]
        public async Task<IActionResult> ResetPassword(string id, [FromQuery] string newPassword)
        {
            await _service.ResetPassword(id, newPassword);
            return NoContent();
        }
    }
}