using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using Workflow.VM.ViewModels.Statistic;

namespace Workflow.Services.Abstract
{
    public interface IGoalCompletionStatisticService
    {
        Task<VmGoalCompletionStatistic> GetGoalCompletion(
            ApplicationUser currentUser,
            StatisticOptions options);


        VmGoalCompletionStatistic GetGoalCompletion(
            IDictionary<string, Goal[]> userGoals);
    }
}