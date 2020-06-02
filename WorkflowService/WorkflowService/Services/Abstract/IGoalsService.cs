﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Services.Abstract
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGoalsService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VmGoal> Get(ApplicationUser currentUser, int id);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="scopeId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="filterFields"></param>
        /// <param name="sortFields"></param>
        /// <param name="withRemoved"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoal>> GetPage(ApplicationUser currentUser, int scopeId,
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
        Task<VmGoal> Update(ApplicationUser user, VmGoal goal);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goalId"></param>
        /// <returns></returns>
        Task<VmGoal> Delete(ApplicationUser currentUser, int goalId);
    }
}