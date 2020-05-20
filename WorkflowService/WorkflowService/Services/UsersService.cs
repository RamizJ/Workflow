using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Services
{
    public class UsersService : IUsersService
    {
        public Task<IEnumerable<VmUser>> GetUsers()
        {
            throw new System.NotImplementedException();
        }
    }
}