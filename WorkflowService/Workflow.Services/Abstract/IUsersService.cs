﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PageLoading;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// Сервис работы с пользователями
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<VmUser> Get(ApplicationUser currentUser, string userId);

        /// <summary>
        /// Получение информации по текущему пользователю
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns>Параметры текущего пользователя</returns>
        Task<VmUser> GetCurrent(ApplicationUser currentUser);

        /// <summary>
        /// Постраничная загрузка записей с фильтрацией и сортировкой
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns>Коллекция пользователей</returns>
        Task<IEnumerable<VmUser>> GetPage(ApplicationUser currentUser, PageOptions pageOptions);

        /// <summary>
        /// Получение пользователей по идентификаторам
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="ids">Идентификаторы пользователей</param>
        /// <returns></returns>
        Task<IEnumerable<VmUser>> GetRange(ApplicationUser currentUser, string[] ids);

        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <param name="user">Новый пользователь</param>
        /// <param name="password">Пароль должен содержать не менее 6 символов, включая цифры</param>
        /// <returns></returns>
        Task<VmUser> Create(VmUser user, string password);

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        /// <param name="user">Параметры пользователя</param>
        /// <returns></returns>
        Task Update(VmUser user);

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<VmUser> Delete(ApplicationUser currentUser, string userId);

        /// <summary>
        /// Удаление пользователей
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="ids">Идентификаторы пользователей</param>
        /// <returns></returns>
        Task<IEnumerable<VmUser>> DeleteRange(ApplicationUser currentUser, IEnumerable<string> ids);


        /// <summary>
        /// Восстановление ранее удаленного пользователя
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<VmUser> Restore(ApplicationUser currentUser, string userId);

        /// <summary>
        /// Восстановление пользователей
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="ids">Идентификаторы пользователей</param>
        /// <returns></returns>
        Task<IEnumerable<VmUser>> RestoreRange(ApplicationUser currentUser, IEnumerable<string> ids);

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="currentPassword">Старый пароль</param>
        /// <param name="newPassword">Новый пароль</param>
        Task ChangePassword(ApplicationUser currentUser, string currentPassword, string newPassword);

        /// <summary>
        /// Сброс пароля пользователя 
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="newPassword">Новый пароль. Пароль должен содержать не менее 6 символов, включая цифры</param>
        /// <returns></returns>
        Task ResetPassword(string id, string newPassword);

        /// <summary>
        /// Проверка существования пользователя с указанным email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> IsEmailExist(string email);

        /// <summary>
        /// Проверка существования пользователя с указанным именем пользователя
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<bool> IsUserNameExist(string userName);
    }
}
