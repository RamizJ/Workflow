using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using WorkflowService.Common;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API for working with scopes of goals
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class ScopesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IScopesService _scopesService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="scopesService"></param>
        public ScopesController(UserManager<ApplicationUser> userManager, IScopesService scopesService)
        {
            _userManager = userManager;
            _scopesService = scopesService;
        }


        /// <summary>
        /// Get scope of goals
        /// </summary>
        /// <param name="id">Scope id</param>
        /// <returns>Scope data. Returned only if available for authenticated user</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmScope>> Get(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            return Ok(await _scopesService.GetScope(user, id));
        }


        /// <summary>
        /// Get all scopes of goals
        /// </summary>
        /// <returns>Collection of scopes</returns>
        [HttpGet]
        public async Task<IEnumerable<VmScope>> GetAll()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _scopesService.GetAll(user);
        }

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
        [HttpGet]
        public async Task<IEnumerable<VmScope>> GetPage([FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] string filter, [FromQuery] string[] filterFields,
            [FromQuery] SortType sort, [FromQuery] string[] sortedFields)
        {
            var user = await _userManager.GetUserAsync(User);
            return await _scopesService.GetPage(user, pageNumber, pageSize,
                filter, filterFields, sort, sortedFields);
        }

        /// <summary>
        /// Get scopes collection by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<VmScope>> GetRange(int[] ids)
        {
            var user = await _userManager.GetUserAsync(User);
            return await _scopesService.GetRange(user, ids);
        }

        /// <summary>
        /// Create scope
        /// </summary>
        /// <param name="scope">New scope</param>
        /// <returns>Scope data with updated scope id</returns>
        [HttpPost]
        public async Task<ActionResult<VmScope>> Create([FromBody] VmScope scope)
        {
            var user = await _userManager.GetUserAsync(User);
            return await _scopesService.CreateScope(user, scope);
        }

        /// <summary>
        /// Update scope
        /// </summary>
        /// <param name="scope">Updated scope</param>
        /// <returns>Nothin</returns>
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]VmScope scope)
        {
            var user = await _userManager.GetUserAsync(User);
            var updatedScope = await _scopesService.UpdateScope(user, scope);
            if (updatedScope == null)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete scope
        /// </summary>
        /// <param name="id">Scope id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<VmScope>> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var deletedScope = await _scopesService.DeleteScope(user, id);
            if (deletedScope == null)
                return NotFound();

            return Ok(deletedScope);
        }
    }
}