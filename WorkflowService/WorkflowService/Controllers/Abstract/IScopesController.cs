using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Controllers.Abstract
{
    /// <summary>
    /// API for working with scopes of goals
    /// </summary>
    public interface IScopesController
    {
        /// <summary>
        /// Get scope of goals
        /// </summary>
        /// <param name="id">Scope id</param>
        /// <returns>Scope data. Returned only if available for authenticated user</returns>
        Task<ActionResult<VmScope>> Get(int id);


        /// <summary>
        /// Get all scopes of goals
        /// </summary>
        /// <returns>Collection of scopes</returns>
        Task<IEnumerable<VmScope>> GetAll();

        /// <summary>
        /// Get scopes of goals with pagination, filtering and sorting
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="filter">Filter string</param>
        /// <param name="filterFields">Fields by which result rows will be filtered. Serparator is "|"</param>
        /// <param name="sort">Sort type. 0 - Unspecified, 1 - Ascending, 2 - Descending</param>
        /// <param name="sortedFields">Fields by which result rows will be sorted. Serparator is "|"</param>
        /// <returns>Collection of scopes</returns>
        Task<IEnumerable<VmScope>> GetRange(int pageNumber, int pageSize,
            string filter, string filterFields,
            SortType sort, string sortedFields);

        /// <summary>
        /// Create scope
        /// </summary>
        /// <param name="scope">New scope</param>
        /// <returns>Scope data with updated scope id</returns>
        Task<ActionResult<VmScope>> Create(VmScope scope);

        /// <summary>
        /// Update scope
        /// </summary>
        /// <param name="scope">Updated scope</param>
        /// <returns>Nothin</returns>
        Task<IActionResult> Update(VmScope scope);

        /// <summary>
        /// Delete scope
        /// </summary>
        /// <param name="id">Scope id</param>
        /// <returns></returns>
        Task<ActionResult<VmScope>> Delete(int id);
    }
}