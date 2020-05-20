using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Common;
using WorkflowService.Controllers.Abstract;

namespace WorkflowService.Controllers
{
    /// <inheritdoc cref="IGoalsController" />
    [ApiController, Route("api/[controller]/[action]")]
    public class GoalsController : ControllerBase, IGoalsController
    {
        /// <inheritdoc cref="IGoalsController" />
        [HttpGet("{id}")]
        public Task<ActionResult<VmGoal>> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGoalsController" />
        [HttpGet]
        public Task<IEnumerable<VmGoal>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGoalsController" />
        [HttpGet]
        public Task<IEnumerable<VmGoal>> GetPage([FromQuery]int pageNumber, [FromQuery]int pageSize, 
            [FromQuery]string filter, [FromQuery]string filterFields, 
            [FromQuery]SortType sort, [FromQuery]string sortedFields)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGoalsController" />
        [HttpGet]
        public Task<IEnumerable<VmGoal>> GetRange([FromQuery]int[] goalIds)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGoalsController" />
        [HttpPost]
        public Task<ActionResult<VmGoal>> Create([FromBody]VmGoal goal)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGoalsController" />
        [HttpPut]
        public Task<IActionResult> Update([FromBody]VmGoal goal)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGoalsController" />
        [HttpDelete("{id}")]
        public Task<ActionResult<VmGoal>> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}