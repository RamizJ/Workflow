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
        /// Получение проектов команды
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
        Task<IEnumerable<VmProject>> GetTeamProjectsPage(ApplicationUser currentUser,
            int teamId, int pageNumber, int pageSize,
            string filter, FieldFilter[] filterFields,
            FieldSort[] sortFields, bool withRemoved = false);

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
        Task<IEnumerable<VmTeam>> GetProjectTeamsPage(ApplicationUser currentUser,
            int projectId, int pageNumber, int pageSize,
            string filter, FieldFilter[] filterFields,
            FieldSort[] sortFields, bool withRemoved = false);

        /// <summary>
        /// Добавить проект в список проектов команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <returns></returns>
        Task AddProject(int teamId, int projectId);

        /// <summary>
        /// Удаление проекта из списка проектов команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="projectId">Идентификатор удаляемого проекта</param>
        /// <returns></returns>
        Task RemoveProject(int teamId, int projectId);

        /// <summary>
        /// Добавление команды в список команд проект
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns>Добавленный проект</returns>
        Task AddTeam(int projectId, int teamId);

        /// <summary>
        /// Удаление команды из списка команд проекта
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns>Удаленный проект</returns>
        Task RemoveTeam(int projectId, int teamId);
    }
}