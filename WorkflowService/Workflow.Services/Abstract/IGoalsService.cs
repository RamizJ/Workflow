﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.Services.Common;
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
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="projectId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="filterFields"></param>
        /// <param name="sortFields"></param>
        /// <param name="withRemoved"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoal>> GetPage(ApplicationUser currentUser, int projectId,
            int pageNumber, int pageSize,
            string filter, FieldFilter[] filterFields, FieldSort[] sortFields, bool withRemoved = false);

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
        /// <param name="goal"></param>
        /// <returns></returns>
        Task<VmGoal> Create(ApplicationUser currentUser, VmGoal goal);

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
        /// <param name="goalId"></param>
        /// <returns></returns>
        Task<VmGoal> Delete(ApplicationUser currentUser, int goalId);

        /// <summary>
        /// Восстановление ранее удаленной задачи
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="goalId">Идентификатор задачи</param>
        /// <returns></returns>
        Task<VmGoal> Restore(ApplicationUser currentUser, int goalId);
    }
}