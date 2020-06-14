using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.Services.Common;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// Сервис управления командами проектов
    /// </summary>
    public interface IProjectTeamsService
    {
        /// <summary>
        /// Получение команд проекта
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтр по всем полям</param>
        /// <param name="filterFields">Фильтр по конкретным полям</param>
        /// <param name="sortFields">Сортировка по полям</param>
        /// <param name="withRemoved">Вместе с удаленными</param>
        /// <returns>Коллеция пользователей команды</returns>
        Task<IEnumerable<VmTeam>> GetPage(ApplicationUser currentUser,
            int projectId, int pageNumber, int pageSize,
            string filter, FieldFilter[] filterFields,
            FieldSort[] sortFields, bool withRemoved = false);

        /// <summary>
        /// Добавление команды в список команд проект
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns>Добавленный проект</returns>
        Task Add(int projectId, int teamId);

        /// <summary>
        /// Удаление команды из списка команд проекта
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns>Удаленный проект</returns>
        Task Remove(int projectId, int teamId);
    }
}