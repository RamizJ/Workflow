using System.Threading.Tasks;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    public interface IAuthenticationService
    {
        Task<VmAuthOutput> Login(VmAuthInput authInput);

        Task Logout();
    }
}
