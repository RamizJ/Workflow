using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Workflow.DAL.Models;
using Workflow.Services.Exceptions;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Services
{
    /// <inheritdoc />
    public class CurrentUserService : ICurrentUserService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        public CurrentUserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <inheritdoc />
        public async Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            if(user == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            return user;
        }


        private readonly UserManager<ApplicationUser> _userManager;
    }
}