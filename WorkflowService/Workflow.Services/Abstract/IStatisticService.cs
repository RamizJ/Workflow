using System.Threading.Tasks;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    public interface IStatisticService
    {
        Task<ProjectStatistic> GetStatistic(int projectId, ProjectStatisticOptions options);
    }
}