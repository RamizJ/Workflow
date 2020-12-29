using System;
using System.Collections.Generic;
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
    public class WorkloadForProjectStatisticService : IWorkloadForProjectStatisticService
    {
        public WorkloadForProjectStatisticService(
            DataContext dataContext,
            IGoalsRepository goalsRepository)
        {
            _dataContext = dataContext;
            _goalsRepository = goalsRepository;
        }

        public async Task<VmWorkloadByProjectsStatistic> GetWorkloadByProject(
            ApplicationUser currentUser, 
            StatisticOptions options)
        {
            IQueryable<Goal> query;
            try
            {
                query = _goalsRepository.GetPerformerGoals(_dataContext.Goals, options.UserIds);
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
                    EstimatedPerformingTime = g.EstimatedPerformingTime,
                })
                .ToArrayAsync();

            var usersGoals = goalsArray
                .GroupBy(g => g.PerformerId)
                .ToDictionary(g => g.Key, g => g.ToArray());

            return GetWorkloadByProject(usersGoals);
        }

        public VmWorkloadByProjectsStatistic GetWorkloadByProject(IDictionary<string, Goal[]> usersGoals)
        {
            var result = new VmWorkloadByProjectsStatistic();
            foreach (var userGoals in usersGoals)
            {
                var projectsHoursDictionary = new Dictionary<int, double>();
                foreach (var userGoal in userGoals.Value)
                {
                    if (projectsHoursDictionary.TryGetValue(userGoal.ProjectId, out var projectHours))
                        projectsHoursDictionary[userGoal.ProjectId] = projectHours + 1;
                    else
                        projectsHoursDictionary[userGoal.ProjectId] = 1;
                }

                var projectHoursForUser = new VmProjectHoursForUser(userGoals.Key);
                foreach (var projectHours in projectsHoursDictionary)
                {
                    result.TotalHours += projectHours.Value;
                    projectHoursForUser.HoursForProject
                        .Add(new VmHoursForProject(projectHours.Key, projectHours.Value));
                }
                
                result.ProjectHoursForUsers.Add(projectHoursForUser);
            }

            return result;
        }


        private readonly DataContext _dataContext;
        private readonly IGoalsRepository _goalsRepository;
    }
}