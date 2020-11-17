using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class RolesService : IRolesService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        public RolesService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <inheritdoc />
        public async Task<bool> IsAdmin(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, RoleNames.ADMINISTRATOR_ROLE);
        }


        private readonly UserManager<ApplicationUser> _userManager;
    }
}