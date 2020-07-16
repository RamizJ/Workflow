using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// Сервис работы с задачами
    /// </summary>
    public interface IGoalsService
    {
        /// <summary>
        /// Получение задачи пользователь
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="id">Идентификатор задачи</param>
        /// <returns>Задача</returns>
        Task<VmGoal> Get(ApplicationUser currentUser, int id);

        /// <summary>
        /// Постраничная загрузка записей с фильтрацией и сортировкой
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns></returns>
        Task<IEnumerable<VmGoal>> GetPage(ApplicationUser currentUser, int? projectId, PageOptions pageOptions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoal>> GetRange(ApplicationUser currentUser, int[] ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<int> GetTotalProjectGoalsCount(ApplicationUser currentUser, int projectId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="projectId"></param>
        /// <param name="goalState"></param>
        /// <returns></returns>
        Task<int> GetProjectGoalsByStateCount(ApplicationUser currentUser, int projectId, GoalState goalState);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        Task<VmGoal> Create(ApplicationUser currentUser, VmGoal goal);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goalForm"></param>
        /// <returns></returns>
        Task<VmGoal> CreateByForm(ApplicationUser currentUser, VmGoalForm goalForm);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        Task Update(ApplicationUser user, VmGoal goal);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goalForm"></param>
        /// <returns></returns>
        Task UpdateByForm(ApplicationUser currentUser, VmGoalForm goalForm);


        /// <summary>
        /// Восстановление ранее удаленной задачи
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="goalId">Идентификатор задачи</param>
        /// <returns></returns>
        Task<VmGoal> Restore(ApplicationUser currentUser, int goalId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goals"></param>
        /// <returns></returns>
        Task UpdateRange(ApplicationUser currentUser, IEnumerable<VmGoal> goals);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goalForms"></param>
        /// <returns></returns>
        Task UpdateByFormRange(ApplicationUser currentUser, IEnumerable<VmGoalForm> goalForms);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goalId"></param>
        /// <returns></returns>
        Task<VmGoal> Delete(ApplicationUser currentUser, int goalId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoal>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoal>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goalId"></param>
        /// <param name="withRemoved"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoal>> GetChildGoals(ApplicationUser currentUser, int goalId, bool withRemoved);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goalId"></param>
        /// <returns></returns>
        Task<VmGoal> GetParentGoal(ApplicationUser currentUser, int goalId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="parentGoalId"></param>
        /// <param name="childGoalIds"></param>
        /// <returns></returns>
        Task AddChildGoals(ApplicationUser currentUser, int? parentGoalId, IEnumerable<int> childGoalIds);
    }
}