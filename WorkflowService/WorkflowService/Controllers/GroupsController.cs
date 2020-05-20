using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Controllers.Abstract;

namespace WorkflowService.Controllers
{
    /// <inheritdoc cref="IGroupsController"/>
    [ApiController, Route("api/[controller]/[action]")]
    public class GroupsController : ControllerBase, IGroupsController
    {
        /// <inheritdoc cref="IGroupsController"/>
        [HttpGet("{id}")]
        public Task<ActionResult<VmGroup>> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGroupsController"/>
        [HttpGet]
        public Task<IEnumerable<VmGroup>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGroupsController"/>
        [HttpGet]
        public Task<IEnumerable<VmGroup>> GetRange([FromQuery]int[] groupIds)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGroupsController"/>
        [HttpPost]
        public Task<ActionResult<VmGroup>> Create([FromBody]VmGroup group)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGroupsController"/>
        [HttpPut]
        public Task<IActionResult> Update([FromBody]VmGroup group)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGroupsController"/>
        [HttpDelete("{id}")]
        public Task<ActionResult<VmGroup>> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}