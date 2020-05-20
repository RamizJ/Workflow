using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Controllers.Abstract
{
    /// <summary>
    /// API for working with goals
    /// </summary>
    public interface IGoalsController
    {
        /// <summary>
        /// Get goal
        /// </summary>
        /// <param name="id">goal id</param>
        /// <returns>goal data. Returned only if available for authenticated user</returns>
        Task<ActionResult<VmGoal>> Get(int id);


        /// <summary>
        /// Get all goals
        /// </summary>
        /// <returns>Collection of goals</returns>
        Task<IEnumerable<VmGoal>> GetAll();

        /// <summary>
        /// Get goals with pagination, filtering and sorting
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="filter">Filter string</param>
        /// <param name="filterFields">Fields by which result rows will be filtered. Serparator is "|"</param>
        /// <param name="sort">Sort type. 0 - Unspecified, 1 - Ascending, 2 - Descending</param>
        /// <param name="sortedFields">Fields by which result rows will be sorted. Serparator is "|"</param>
        /// <returns>Collection of goals</returns>
        Task<IEnumerable<VmGoal>> GetPage(int pageNumber, int pageSize,
            string filter, string filterFields,
            SortType sort, string sortedFields);

        /// <summary>
        /// Get goals collection by ids
        /// </summary>
        /// <param name="goalIds"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoal>> GetRange(int[] goalIds);

        /// <summary>
        /// Create goal
        /// </summary>
        /// <param name="goal">New goal</param>
        /// <returns>goal data with updated goal id</returns>
        Task<ActionResult<VmGoal>> Create(VmGoal goal);

        /// <summary>
        /// Update goal
        /// </summary>
        /// <param name="goal">Updated goal</param>
        /// <returns>Nothin</returns>
        Task<IActionResult> Update(VmGoal goal);

        /// <summary>
        /// Delete goal
        /// </summary>
        /// <param name="id">goal id</param>
        /// <returns></returns>
        Task<ActionResult<VmGoal>> Delete(int id);
    }
}