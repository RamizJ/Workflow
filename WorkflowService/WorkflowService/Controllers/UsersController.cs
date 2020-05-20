using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using WorkflowService.Common;
using WorkflowService.Controllers.Abstract;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    /// <inheritdoc cref="IUsersController" />
    [ApiController, Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase, IUsersController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsersService _usersService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="usersService"></param>
        public UsersController(UserManager<ApplicationUser> userManager, IUsersService usersService)
        {
            _userManager = userManager;
            _usersService = usersService;
        }

        /// <inheritdoc cref="IUsersController" />
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var users = await _usersService.GetUsers();
            return Ok(users);
        }

        /// <inheritdoc cref="IUsersController" />
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _usersService.GetUsers();
            return Ok(users);
        }

        /// <inheritdoc cref="IUsersController" />
        [HttpGet]
        public async Task<IActionResult> GetPage([FromQuery]int pageNumber, [FromQuery]int pageSize,
            [FromQuery]string filter, [FromQuery]string filterFields,
            [FromQuery]SortType sort, [FromQuery]string sortedFields)
        {
            var users = await _usersService.GetUsers();
            return Ok(users);
        }

        /// <inheritdoc cref="IUsersController" />
        [HttpGet]
        public Task<IEnumerable<VmUser>> GetRange([FromQuery]string[] userIds)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IUsersController" />
        [HttpPost]
        public async Task<VmUser> Create([FromBody]VmUser user)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IUsersController" />
        [HttpPost]
        public async Task<ActionResult> Update(VmUser user)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IUsersController" />
        [HttpDelete]
        public async Task<ActionResult<VmScope>> Delete(string userId)
        {
            throw new NotImplementedException();
        }
    }
}