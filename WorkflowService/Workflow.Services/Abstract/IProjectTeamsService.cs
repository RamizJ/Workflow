using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.Common;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// Сервис управления командами проектов
    /// </summary>
    public interface IProjectTeamsService
    {
        /// <summary>
        /// Постраничная загрузка команд проекта
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns></returns>
        Task<IEnumerable<VmTeam>> GetPage(ApplicationUser currentUser, int projectId, PageOptions pageOptions);

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