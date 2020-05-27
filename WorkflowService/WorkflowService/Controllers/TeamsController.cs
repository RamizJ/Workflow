using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API for working with teams
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class TeamsController : ControllerBase
    {
        /// <summary>
        /// Get team
        /// </summary>
        /// <param name="id">team id</param>
        /// <returns>team data. Returned only if available for authenticated user</returns>
        [HttpGet("{id}")]
        public Task<ActionResult<VmTeam>> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get all teams
        /// </summary>
        /// <returns>Collection of teams</returns>
        [HttpGet]
        public Task<IEnumerable<VmTeam>> GetAll()
        {
            throw new System.NotImplementedException();
        }

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
        [HttpGet]
        public Task<IEnumerable<VmTeam>> GetPage([FromQuery]int pageNumber, [FromQuery]int pageSize,
            [FromQuery]string filter, [FromQuery]string filterFields,
            [FromQuery]SortType sort, [FromQuery]string sortedFields)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get teams collection by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public Task<IEnumerable<VmTeam>> GetRange([FromQuery]int[] ids)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get user teams
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Collection users of team</returns>
        [HttpGet("{userId}")]
        public Task<IEnumerable<VmTeam>> GetUserTeams(string userId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Create team
        /// </summary>
        /// <param name="team">New team</param>
        /// <returns>team data with updated team id</returns>
        [HttpPost]
        public Task<ActionResult<VmTeam>> Create([FromBody]VmTeam team)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Update team
        /// </summary>
        /// <param name="team">Updated team</param>
        /// <returns></returns>
        [HttpPut]
        public Task<IActionResult> Update([FromBody]VmTeam team)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Delete team
        /// </summary>
        /// <param name="id">team id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public Task<ActionResult<VmTeam>> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}