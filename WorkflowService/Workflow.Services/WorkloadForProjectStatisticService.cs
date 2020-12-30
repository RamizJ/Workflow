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
            IQueryable<Goal> query = _dataContext.Goals
                .Include(g => g.Project);
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
                    Project = new Project {Name = g.Project.Name},
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
                var userProjectsHours = userGoals.Value
                    .GroupBy(x => (x.ProjectId, x.Project.Name))
                    .ToDictionary(x => x.Key, x => x
                        .Sum(y => y.EstimatedPerformingTime?.TotalHours ?? 0));


                var vmUserProjectsHours = new VmProjectHoursForUser
                {
                    UserId = userGoals.Key,
                    HoursForProject = new List<VmHoursForProject>()
                };
                result.ProjectHoursForUsers.Add(vmUserProjectsHours);

                foreach (var userProjectHours in userProjectsHours)
                {
                    result.TotalHours += userProjectHours.Value;
                    vmUserProjectsHours.HoursForProject.Add(new VmHoursForProject
                    {
                        ProjectId = userProjectHours.Key.ProjectId,
                        ProjectName = userProjectHours.Key.Name,
                        Hours = userProjectHours.Value
                    });
                }
            }

            return result;
        }


        private readonly DataContext _dataContext;
        private readonly IGoalsRepository _goalsRepository;
    }
}