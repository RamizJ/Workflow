using System.Threading.Tasks;
using Workflow.VM.ViewModels;

namespace WorkflowService.Services.Abstract
{
    public interface IAuthenticationService
    {
        Task<VmAuthOutput> Login(VmAuthInput authInput);

        Task Logout();
    }
}
