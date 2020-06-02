using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Services.Abstract
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
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="filterFields"></param>
        /// <param name="sortFields"></param>
        /// <param name="withRemoved"></param>
        /// <returns></returns>
        Task<IEnumerable<VmUser>> GetPage(ApplicationUser currentUser, int pageNumber, int pageSize,
            string filter, FieldFilter[] filterFields, FieldSort[] sortFields, bool withRemoved = false);


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
        /// <param name="currentUser">Текущий пользователь </param>
        /// <param name="user">Новый пользователь</param>
        /// <returns></returns>
        Task<VmUser> Create(ApplicationUser currentUser, VmUser user);

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="user">Параметры пользователя</param>
        /// <returns></returns>
        Task<VmUser> Update(ApplicationUser currentUser, VmUser user);

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<VmUser> Delete(ApplicationUser currentUser, string userId);
    }
}
