using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Controllers.Abstract
{
    /// <summary>
    /// API for working with teams
    /// </summary>
    public interface ITeamsController
    {
        /// <summary>
        /// Get team
        /// </summary>
        /// <param name="id">team id</param>
        /// <returns>team data. Returned only if available for authenticated user</returns>
        Task<ActionResult<VmTeam>> Get(int id);


        /// <summary>
        /// Get all teams
        /// </summary>
        /// <returns>Collection of teams</returns>
        Task<IEnumerable<VmTeam>> GetAll();

        /// <summary>
        /// Get teams with pagination, filtering and sorting
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="filter">Filter string</param>
        /// <param name="filterFields">Fields by which result rows will be filtered. Serparator is "|"</param>
        /// <param name="sort">Sort type. 0 - Unspecified, 1 - Ascending, 2 - Descending</param>
        /// <param name="sortedFields">Fields by which result rows will be sorted. Serparator is "|"</param>
        /// <returns>Collection of teams</returns>
        Task<IEnumerable<VmTeam>> GetPage(int pageNumber, int pageSize,
            string filter, string filterFields,
            SortType sort, string sortedFields);

        /// <summary>
        /// Get teams collection by ids
        /// </summary>
        /// <param name="teamIds"></param>
        /// <returns></returns>
        Task<IEnumerable<VmTeam>> GetRange(int[] teamIds);

        /// <summary>
        /// Create team
        /// </summary>
        /// <param name="team">New team</param>
        /// <returns>team data with updated team id</returns>
        Task<ActionResult<VmTeam>> Create(VmTeam team);

        /// <summary>
        /// Update team
        /// </summary>
        /// <param name="team">Updated team</param>
        /// <returns>Nothin</returns>
        Task<IActionResult> Update(VmTeam team);

        /// <summary>
        /// Delete team
        /// </summary>
        /// <param name="id">team id</param>
        /// <returns></returns>
        Task<ActionResult<VmTeam>> Delete(int id);
    }
}