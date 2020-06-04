﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        /// <param name="user">Параметры пользователя</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<VmUserResult> Create([FromBody]VmUser user)
        {
            return await _service.Create(user);
        }

        /// <summary>
        /// Обновление пользователя
        /// </summary>
        /// <param name="user">Параметры пользователя</param>
        /// <returns>Результат</returns>
        [HttpPut]
        public async Task<VmUserResult> Update(VmUser user)
        {
            return await _service.Update(user);
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<VmUserResult> Delete(string id)
        {
            return await _service.Delete(id);
        }


        /// <summary>
        /// Изменение пароля пользователя. Доступно только текущему пользователю
        /// </summary>
        /// <param name="currentPassword">Текущий пароль</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<VmUserResult> ChangePassword([FromQuery]string currentPassword, [FromQuery] string newPassword)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _service.ChangePassword(currentUser, currentPassword, newPassword);
        }

        /// <summary>
        /// Сброс пароля пользователя. Доступно только администраторам
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <returns></returns>
        [HttpPost("{id}"), Authorize(Roles = RoleNames.ADMINISTRATOR_ROLE)]
        public async Task<VmUserResult> ResetPassword(string id, [FromQuery] string newPassword)
        {
            return await _service.ResetPassword(id, newPassword);
        }
    }
}