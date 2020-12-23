using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using Workflow.VM.ViewModels.Statistic;

namespace Workflow.Services.Abstract
{
    public interface IStatisticService
    {
        Task<VmProjectStatistic> GetStatisticForProject(int projectId, StatisticOptions options);

        Task<VmProjectStatistic> GetStatisticForUser(string userId, StatisticOptions options);

        Task<VmProjectStatistic> GetStatisticForUserAndProject(string userId, int projectId, StatisticOptions options);
    }
}