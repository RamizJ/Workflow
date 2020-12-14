using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;
using Workflow.VM.ViewModels.Statistic;

namespace Workflow.Services
{
    public class WorkloadByDaysStatisticService : IWorkloadByDaysStatisticService
    {
        public async Task<IDictionary<string, VmWorkloadByDaysStatistic>> GetWorkloadByDays(
            ApplicationUser currentUser, 
            StatisticOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}