using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using Workflow.VM.ViewModels.Statistic;

namespace Workflow.Services.Abstract
{
    public interface IWorkloadForProjectStatisticService
    {
        Task<VmWorkloadByProjectsStatistic> GetWorkloadByProject(
            ApplicationUser currentUser, 
            StatisticOptions options);

        VmWorkloadByProjectsStatistic GetWorkloadByProject(IDictionary<string, Goal[]> usersGoals);
    }
}