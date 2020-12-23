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
    public class WorkloadForProjectStatisticService : IWorkloadForProjectStatisticService
    {
        public WorkloadForProjectStatisticService(IGoalsRepository goalsRepository)
        {
            _goalsRepository = goalsRepository;
        }

        public async Task<VmWorkloadByProjectsStatistic> GetWorkloadByProject(
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
                })
                .GroupBy(g => g.PerformerId)
                .ToDictionaryAsync(g => g.Key, g => g.ToArray());

            return GetWorkloadByProject(usersGoals);
        }

        public VmWorkloadByProjectsStatistic GetWorkloadByProject(IDictionary<string, Goal[]> usersGoals)
        {
            var result = new VmWorkloadByProjectsStatistic();
            foreach (var userGoals in usersGoals)
            {
                var projectHoursForUser = new VmProjectHoursForUser(userGoals.Key);
                foreach (var projectHours in userGoals.Value)
                {
                    double hours = projectHours.EstimatedPerformingTime?.TotalHours ?? 0;
                    result.TotalHours += hours;
                    var hoursForProject = new VmHoursForProject(projectHours.ProjectId, hours);
                    projectHoursForUser.HoursForProject.Add(hoursForProject);
                }
                result.ProjectHoursForUsers.Add(projectHoursForUser);
            }

            return result;
        }


        private readonly IGoalsRepository _goalsRepository;
    }
}