using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.Services.Common;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITeamUsersService
    {
        /// <summary>
        /// Получение пользователей команды  
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтр по всем полям</param>
        /// <param name="filterFields">Фильтр по конкретным полям</param>
        /// <param name="sortFields">Сортировка по полям</param>
        /// <param name="withRemoved">Вместе с удаленными</param>
        /// <returns>Коллеция пользователей команды</returns>
        Task<IEnumerable<VmUser>> GetPage(ApplicationUser currentUser, 
            int teamId, int pageNumber, int pageSize,
            string filter, FieldFilter[] filterFields, 
            FieldSort[] sortFields, bool withRemoved = false);

        /// <summary>
        /// Добавить пользователя в команду
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="userId">Добавляемый пользователь</param>
        /// <returns>Добавленный пользователь</returns>
        Task Add(ApplicationUser currentUser, int teamId, string userId);

        /// <summary>
        /// Удаление пользователя из команды
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="userId">Идентификатор удаляемого пользователя</param>
        /// <returns>Удаленный пользователь</returns>
        Task Remove(ApplicationUser currentUser, int teamId, string userId);
    }
}