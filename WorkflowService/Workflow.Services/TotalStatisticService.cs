using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
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
        public TotalStatisticService(
            DataContext dataContext,
            IGoalsRepository goalsRepository,
            IGoalCompletionStatisticService goalCompletionStatisticService,
            IWorkloadForProjectStatisticService workloadForProjectStatisticService,
            IWorkloadByDaysStatisticService workloadByDaysStatisticService)
        {
            _dataContext = dataContext;
            _goalsRepository = goalsRepository;
            _goalCompletionStatisticService = goalCompletionStatisticService;
            _workloadForProjectStatisticService = workloadForProjectStatisticService;
            _workloadByDaysStatisticService = workloadByDaysStatisticService;
        }
        public async Task<VmTotalStatistic> GetTotal(
            ApplicationUser currentUser, 
            StatisticOptions options)
        {
            IQueryable<Goal> query = _dataContext.Goals;
            try
            {
                query = _goalsRepository.GetPerformerGoals(query, options.UserIds);
                query = _goalsRepository.GetGoalsForProjects(query, options.ProjectIds);
                query = _goalsRepository.GetGoalsForPeriod(query, options.DateBegin, options.DateEnd);
            }
            catch (ArgumentException)
            {
                throw new HttpResponseException(BadRequest, "Wrong options");
            }

            var goalsArray = await query
                .Select(g => new Goal
                {
                    PerformerId = g.PerformerId,
                    ProjectId = g.ProjectId,
                    Project = g.Project,
                    State = g.State,
                    StateChangedDate = g.StateChangedDate,
                    ExpectedCompletedDate = g.ExpectedCompletedDate,
                    EstimatedPerformingHours = g.EstimatedPerformingHours,
                    ActualPerformingHours = g.ActualPerformingHours
                })
                .ToArrayAsync();

            var usersGoals = goalsArray
                .GroupBy(g => g.PerformerId)
                .ToDictionary(g => g.Key, g => g.ToArray());

            var result = new VmTotalStatistic
            {
                GoalCompletion = _goalCompletionStatisticService.GetGoalCompletion(usersGoals),
                WorkloadByProjects = _workloadForProjectStatisticService.GetWorkloadByProject(usersGoals)
            };

            return result;
        }


        private readonly DataContext _dataContext;
        private readonly IGoalsRepository _goalsRepository;
        private readonly IGoalCompletionStatisticService _goalCompletionStatisticService;
        private readonly IWorkloadForProjectStatisticService _workloadForProjectStatisticService;
        private readonly IWorkloadByDaysStatisticService _workloadByDaysStatisticService;
    }
}