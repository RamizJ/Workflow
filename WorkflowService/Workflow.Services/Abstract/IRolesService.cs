using System.Threading.Tasks;
using Workflow.DAL.Models;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRolesService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> IsAdmin(ApplicationUser user);
    }
}