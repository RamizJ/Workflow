using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    public interface IProjectTeamRolesService
    {
        Task<VmProjectTeamRole> Get(int projectId, int teamId);

        Task<VmProjectTeamRole> Add(VmProjectTeamRole viewModel);

        Task Update(VmProjectTeamRole viewModel);

        Task Delete(int projectId, int teamId);

        Task<bool> IsExist(int projectId, int teamId);
    }
}