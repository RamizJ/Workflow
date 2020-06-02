using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API for working with groups
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class GroupsController : ControllerBase
    {
        /// <summary>
        /// Get group
        /// </summary>
        /// <param name="id">group id</param>
        /// <returns>group data. Returned only if available for authenticated user</returns>
        [HttpGet("{id}")]
        public Task<ActionResult<VmGroup>> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get all groups
        /// </summary>
        /// <returns>Collection of groups</returns>
        [HttpGet]
        public Task<IEnumerable<VmGroup>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get groups with pagination, filtering and sorting
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="filter">Filter string</param>
        /// <param name="filterFields">Fields by which result rows will be filtered. Serparator is "|"</param>
        /// <param name="sort">Sort type. 0 - Unspecified, 1 - Ascending, 2 - Descending</param>
        /// <param name="sortedFields">Fields by which result rows will be sorted. Serparator is "|"</param>
        /// <returns>Collection of scopes</returns>
        [HttpGet]
        public Task<IEnumerable<VmScope>> GetPage([FromQuery]int pageNumber, [FromQuery]int pageSize,
            [FromQuery]string filter, [FromQuery]string filterFields,
            [FromQuery]SortType sort, [FromQuery]string sortedFields)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get groups collection by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public Task<IEnumerable<VmGroup>> GetRange([FromQuery]int[] ids)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Create group
        /// </summary>
        /// <param name="group">New group</param>
        /// <returns>group data with updated group id</returns>
        [HttpPost]
        public Task<ActionResult<VmGroup>> Create([FromBody]VmGroup group)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Update group
        /// </summary>
        /// <param name="group">Updated group</param>
        /// <returns>Nothin</returns>
        [HttpPut]
        public Task<IActionResult> Update([FromBody]VmGroup group)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Delete group
        /// </summary>
        /// <param name="id">group id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public Task<ActionResult<VmGroup>> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}