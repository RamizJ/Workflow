using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL.Models;
using Workflow.DAL.Repositories.Abstract;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.VM.ViewModels;
using Workflow.VM.ViewModels.Statistic;
using static System.Net.HttpStatusCode;

namespace Workflow.Services
{
    public class TotalStatisticService : ITotalStatisticService
    {
        public TotalStatisticService(IGoalsRepository goalsRepository,
            IGoalCompletionStatisticService goalCompletionStatisticService,
            IWorkloadForProjectStatisticService workloadForProjectStatisticService,
            IWorkloadByDaysStatisticService workloadByDaysStatisticService)
        {
            _goalsRepository = goalsRepository;
            _goalCompletionStatisticService = goalCompletionStatisticService;
            _workloadForProjectStatisticService = workloadForProjectStatisticService;
            _workloadByDaysStatisticService = workloadByDaysStatisticService;
        }
        public async Task<VmTotalStatistic> GetTotal(
            ApplicationUser currentUser, 
            StatisticOptions options)
        {
            IQueryable<Goal> query;
            try
            {
                query = _goalsRepository.GetPerformerGoalsForPeriod(options.UserIds,
                    options.DateBegin, options.DateEnd);
            }
            catch (ArgumentException)
            {
                throw new HttpResponseException(BadRequest, "Wrong options");
            }

            var usersGoals = await query
                .Select(g => new Goal
                {
                    PerformerId = g.PerformerId,
                    ProjectId = g.ProjectId,
                    EstimatedPerformingTime = g.EstimatedPerformingTime,
                    State = g.State,
                    StateChangedDate = g.StateChangedDate,
                    ExpectedCompletedDate = g.ExpectedCompletedDate
                })
                .GroupBy(g => g.PerformerId)
                .ToDictionaryAsync(g => g.Key, g => g.ToArray());

            var result = new VmTotalStatistic
            {
                GoalCompletion = _goalCompletionStatisticService.GetGoalCompletion(usersGoals),
                WorkloadByProjects = _workloadForProjectStatisticService.GetWorkloadByProject(usersGoals)
            };

            return result;
        }


        private readonly IGoalsRepository _goalsRepository;
        private readonly IGoalCompletionStatisticService _goalCompletionStatisticService;
        private readonly IWorkloadForProjectStatisticService _workloadForProjectStatisticService;
        private readonly IWorkloadByDaysStatisticService _workloadByDaysStatisticService;
    }
}