using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    public interface IProjectUserRolesService
    {
        Task<ProjectUserRole> Get(int projectId, string userId);

        Task<VmProjectUserRole> Add(VmProjectUserRole viewModel);
        Task AddRange(IEnumerable<VmProjectUserRole> viewModels);
        Task AddForTeam(int projectId, int teamId);

        Task Update(VmProjectUserRole viewModel);
        Task UpdateRange(IEnumerable<VmProjectUserRole> viewModels);

        Task Delete(int projectId, string userId);
        Task DeleteRange(int projectId, IEnumerable<string> userIds);

        Task<bool> IsExist(int projectId, string userId);
    }
}