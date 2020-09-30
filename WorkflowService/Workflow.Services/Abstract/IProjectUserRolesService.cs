using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    public interface IProjectUserRolesService
    {
        Task<ProjectUserRole> Get(int projectId, string userId);

        Task<VmProjectUserRole> Add(VmProjectUserRole viewModel);

        Task Update(VmProjectUserRole viewModel);

        Task Delete(int projectId, string userId);

        Task<bool> IsExist(int projectId, string userId);
    }
}