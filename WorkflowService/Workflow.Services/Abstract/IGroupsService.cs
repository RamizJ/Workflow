using System.Collections.Generic;
using System.Threading.Tasks;
using PageLoading;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// Сервис групп проектов
    /// </summary>
    public interface IGroupsService
    {
        /// <summary>
        /// Получение группы по идентификатору
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<VmGroup> Get(ApplicationUser user, int id);

        /// <summary>
        /// Получение страницы групп
        /// </summary>
        /// <param name="user">Текущий пользователь</param>
        /// <param name="parentGroupId">Идентификатор родительской группы</param>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns></returns>
        public Task<IEnumerable<VmGroup>> GetPage(ApplicationUser user, int? parentGroupId, PageOptions pageOptions);

        /// <summary>
        /// Загрузка иерархии групп проектов с проектами 
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns></returns>
        public Task<IEnumerable<VmGroup>> GetAll(ApplicationUser currentUser);

        /// <summary>
        /// Создание группы
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="group">Создаваемая группа</param>
        /// <returns></returns>
        Task<VmGroup> Create(ApplicationUser currentUser, VmGroup group);

        /// <summary>
        /// Обновление текущей группы
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="group">Обновляемая группа</param>
        /// <returns></returns>
        Task Update(ApplicationUser currentUser, VmGroup group);


        /// <summary>
        /// Восстановление ранее удаленной задачи
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="id">Идентификатор группы</param>
        /// <returns></returns>
        Task<VmGroup> Restore(ApplicationUser currentUser, int id);

        /// <summary>
        /// Обновление группы проектов
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        Task UpdateRange(ApplicationUser currentUser, IEnumerable<VmGroup> groups);

        /// <summary>
        /// Удаление группы проектов
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VmGroup> Delete(ApplicationUser currentUser, int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGroup>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ids"></param>,
        /// <returns></returns>
        Task<IEnumerable<VmGroup>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="groupId"></param>
        /// <param name="projectIds"></param>
        Task AddProjects(ApplicationUser currentUser, int groupId, ICollection<int> projectIds);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="groupId"></param>
        /// <param name="projectIds"></param>
        Task RemoveProjects(ApplicationUser currentUser, int groupId, ICollection<int> projectIds);
    }
}