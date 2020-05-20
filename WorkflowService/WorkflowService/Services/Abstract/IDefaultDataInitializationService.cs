using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Workflow.DAL.Models;

namespace WorkflowService.Services.Abstract
{
    public interface IDefaultDataInitializationService
    {
        Task InitializeRoles();

        Task InitializeAdmin();
    }
}