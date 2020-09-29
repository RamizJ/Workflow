using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// Сервис управления пользователями команды
    /// </summary>
    public interface ITeamUsersService
    {
        /// <summary>
        /// Получение пользователей команды  
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns>Коллеция пользователей команды</returns>
        Task<IEnumerable<VmUser>> GetPage(ApplicationUser currentUser,
            int teamId, PageOptions pageOptions);

        /// <summary>
        /// Добавить пользователя в команду
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="userId"></param>
        /// <returns>Добавленный пользователь</returns>
        Task Add(int teamId, string userId);

        /// <summary>
        /// Добавление пользователей в команду
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="userIds">Идентификаторы пользователей</param>
        /// <returns></returns>
        Task AddRange(int teamId, IEnumerable<string> userIds);

        /// <summary>
        /// Удаление пользователя из команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="userId">Идентификатор удаляемого пользователя</param>
        /// <returns>Удаленный пользователь</returns>
        Task Remove(int teamId, string userId);

        /// <summary>
        /// Удаление пользователей из команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="userIds">Идентификаторы пользователей</param>
        /// <returns>Удаленный пользователь</returns>
        Task RemoveRange(int teamId, IEnumerable<string> userIds);
    }
}