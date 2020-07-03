using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;

namespace Workflow.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public CurrentUserService(DataContext dataContext, 
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<ApplicationUser> Get(ClaimsPrincipal claimsPrincipal)
        {
            if (_configuration.GetValue<bool>(ConfigKeys.IS_API_TEST_MODE))
            {
                var admins = await _userManager.GetUsersInRoleAsync(RoleNames.ADMINISTRATOR_ROLE);
                return admins.FirstOrDefault();
            }

            return await _userManager.GetUserAsync(claimsPrincipal);
        }
    }
}