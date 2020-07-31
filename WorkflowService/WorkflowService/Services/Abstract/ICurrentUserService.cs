using System.Security.Claims;
using System.Threading.Tasks;
using Workflow.DAL.Models;

namespace WorkflowService.Services.Abstract
{
    /// <summary>
    /// Сервис работы с текущим пользователем
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Получение текущего пользователя
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal claimsPrincipal);
    }
}