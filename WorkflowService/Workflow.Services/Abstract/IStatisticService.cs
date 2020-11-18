using System.Threading.Tasks;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    public interface IStatisticService
    {
        Task<ProjectStatistic> GetStatisticForProject(int projectId, StatisticOptions options);

        Task<ProjectStatistic> GetStatisticForUser(string userId, StatisticOptions options);

        Task<ProjectStatistic> GetStatisticForUserAndProject(string userId, int projectId, StatisticOptions options);
    }
}