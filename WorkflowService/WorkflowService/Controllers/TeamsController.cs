using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Common;
using WorkflowService.Controllers.Abstract;

namespace WorkflowService.Controllers
{
    /// <inheritdoc cref="ITeamsController"/>
    [ApiController, Route("api/[controller]/[action]")]
    public class TeamsController : ControllerBase, ITeamsController
    {
        /// <inheritdoc cref="ITeamsController"/>
        [HttpGet("{id}")]
        public Task<ActionResult<VmTeam>> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="ITeamsController"/>
        [HttpGet]
        public Task<IEnumerable<VmTeam>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="ITeamsController"/>
        [HttpGet]
        public Task<IEnumerable<VmTeam>> GetPage([FromQuery]int pageNumber, [FromQuery]int pageSize,
            [FromQuery]string filter, [FromQuery]string filterFields,
            [FromQuery]SortType sort, [FromQuery]string sortedFields)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="ITeamsController"/>
        [HttpGet]
        public Task<IEnumerable<VmTeam>> GetRange([FromQuery]int[] teamIds)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="ITeamsController"/>
        [HttpPost]
        public Task<ActionResult<VmTeam>> Create([FromBody]VmTeam team)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="ITeamsController"/>
        [HttpPut]
        public Task<IActionResult> Update([FromBody]VmTeam team)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="ITeamsController"/>
        [HttpDelete("{id}")]
        public Task<ActionResult<VmTeam>> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}