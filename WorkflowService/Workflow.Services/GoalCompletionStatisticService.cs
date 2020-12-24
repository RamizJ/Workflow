using System;
using System.Collections.Generic;
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
    /// <inheritdoc />
    public class GoalCompletionStatisticService : IGoalCompletionStatisticService
    {
        public GoalCompletionStatisticService(IGoalsRepository goalsRepository)
        {
            _goalsRepository = goalsRepository;
        }

        /// <inheritdoc />
        public async Task<VmGoalCompletionStatistic> GetGoalCompletion(
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
                    State = g.State,
                    StateChangedDate = g.StateChangedDate,
                    ExpectedCompletedDate = g.ExpectedCompletedDate
                })
                .GroupBy(g => g.PerformerId)
                .ToDictionaryAsync(g => g.Key, g => g.ToArray());

            return GetGoalCompletion(usersGoals);
        }

        public VmGoalCompletionStatistic GetGoalCompletion(IDictionary<string, Goal[]> userGoals)
        {
            if(userGoals == null)
                throw new HttpResponseException(BadRequest, "Wrong options");

            var result = new VmGoalCompletionStatistic();
            foreach (var userGoal in userGoals)
            {
                var userGoalsCompletion = new VmUserGoalsCompletion
                {
                    UserId = userGoal.Key,
                    GoalsCompletion = new VmGoalsCompletion
                    {
                        CompletedOnTime = userGoal.Value.Count(g => g.State == GoalState.Succeed
                                                                    && g.StateChangedDate <= g.ExpectedCompletedDate),
                        CompletedNotOnTime = userGoal.Value.Count(g => g.State == GoalState.Succeed
                                                                       && g.StateChangedDate > g.ExpectedCompletedDate),
                        InProcess = userGoal.Value.Count(g => g.State == GoalState.Perform),
                        NotCompleted = userGoal.Value.Count(g => g.State != GoalState.Succeed
                                                                 && g.State != GoalState.Perform)
                    }
                };
                result.UserGoalsCompletions.Add(userGoalsCompletion);
            }

            return result;
        }


        private readonly IGoalsRepository _goalsRepository;
    }
}