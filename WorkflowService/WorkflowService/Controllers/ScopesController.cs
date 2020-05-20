using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using WorkflowService.Common;
using WorkflowService.Controllers.Abstract;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    /// <inheritdoc cref="IScopesController"/>
    [ApiController, Route("api/[controller]/[action]")]
    public class ScopesController : ControllerBase, IScopesController
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

        [HttpGet("{id}")]
        public async Task<ActionResult<VmScope>> Get(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            return Ok(await _scopesService.GetScope(user, id));
        }

        [HttpGet]
        public async Task<IEnumerable<VmScope>> GetAll()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _scopesService.GetScopes(user);
        }

        [HttpGet]
        public async Task<IEnumerable<VmScope>> GetRange([FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] string filter, [FromQuery] string filterFields,
            [FromQuery] SortType sort, [FromQuery] string sortedFields)
        {
            var user = await _userManager.GetUserAsync(User);
            return await _scopesService.GetScopes(user, pageNumber, pageSize,
                filter, filterFields, sort, sortedFields);
        }

        [HttpPost]
        public async Task<ActionResult<VmScope>> Create([FromBody] VmScope scope)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody]VmScope scope)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<VmScope>> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}