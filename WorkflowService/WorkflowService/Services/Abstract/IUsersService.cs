using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.VM.ViewModels;

namespace WorkflowService.Services.Abstract
{
    public interface IUsersService
    {
        Task<IEnumerable<VmUser>> GetUsers();
    }
}
