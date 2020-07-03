using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.Common;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// Сервис управления проектами команд
    /// </summary>
    public interface ITeamProjectsService
    {
        /// <summary>
        /// Получение проектов команды
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="pageOptions"></param>
        /// <returns>Коллеция пользователей команды</returns>
        Task<IEnumerable<VmProject>> GetPage(ApplicationUser currentUser,
            int teamId, PageOptions pageOptions);

        /// <summary>
        /// Добавить проект в список проектов команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <returns></returns>
        Task Add(int teamId, int projectId);

        /// <summary>
        /// Удаление проекта из списка проектов команды
        /// </summary>
        /// <param name="teamId">Идентификатор команды</param>
        /// <param name="projectId">Идентификатор удаляемого проекта</param>
        /// <returns></returns>
        Task Remove(int teamId, int projectId);
    }
}