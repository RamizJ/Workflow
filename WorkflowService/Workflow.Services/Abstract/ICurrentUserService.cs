using System.Security.Claims;
using System.Threading.Tasks;
using Workflow.DAL.Models;

namespace Workflow.Services.Abstract
{
    public interface ICurrentUserService
    {
        Task<ApplicationUser> Get(ClaimsPrincipal claimsPrincipal);
    }
}