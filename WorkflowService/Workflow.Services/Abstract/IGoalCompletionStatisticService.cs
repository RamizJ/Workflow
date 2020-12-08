using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using Workflow.VM.ViewModels.Statistic;

namespace Workflow.Services.Abstract
{
    public interface IGoalCompletionStatisticService
    {
        Task<IDictionary<string, VmGoalCompletionStatistic>> GetGoalCompletion(
            ApplicationUser currentUser,
            StatisticOptions options);
    }
}