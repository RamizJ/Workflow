using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Services.Abstract
{
    /// <summary>
    /// Сервис команд
    /// </summary>
    public interface ITeamsService
    {
        /// <summary>
        /// Получение команды по идентификатору
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VmTeam> Get(ApplicationUser currentUser, int id);

        /// <summary>
        /// Получение всех команд
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="withRemoved">Вместе с удаленными</param>
        /// <returns></returns>
        Task<IEnumerable<VmTeam>> GetAll(ApplicationUser currentUser, bool withRemoved = false);

        /// <summary>
        /// Получение страницы команд
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтр по всем поляи</param>
        /// <param name="filterFields">Поля фильтрации</param>
        /// <param name="sortFields">Поля сортировки</param>
        /// <param name="withRemoved">Получить вместе с удаленными</param>
        /// <returns></returns>
        Task<IEnumerable<VmTeam>> GetPage(ApplicationUser currentUser, int pageNumber, int pageSize,
            string filter, FieldFilter[] filterFields, FieldSort[] sortFields, bool withRemoved = false);

        /// <summary>
        /// Получение команд по идентификаторам
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="ids">Идентификаторы команд</param>
        /// <returns></returns>
        Task<IEnumerable<VmTeam>> GetRange(ApplicationUser currentUser, int[] ids);

        /// <summary>
        /// Создание команды
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="team">Команда</param>
        /// <returns>Результат выполнения операции</returns>
        Task<VmTeamResult> Create(ApplicationUser currentUser, VmTeam team);

        /// <summary>
        /// Обновление команды
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="team">Обновленная команда</param>
        /// <returns>Результат выполнения операции</returns>
        Task<VmTeamResult> Update(ApplicationUser currentUser, VmTeam team);

        /// <summary>
        /// Удаление команды
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="teamId">Идентификатор команды</param>
        /// <returns>Результат выполнения операции</returns>
        Task<VmTeamResult> Delete(ApplicationUser currentUser, int teamId);
    }
}