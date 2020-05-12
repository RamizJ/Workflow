using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    [Authorize]
    [ApiController, Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsersService _usersService;


        public UsersController(UserManager<ApplicationUser> userManager, IUsersService usersService)
        {
            _userManager = userManager;
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _usersService.GetUsers();
            return Ok(users);
        }
    }
}