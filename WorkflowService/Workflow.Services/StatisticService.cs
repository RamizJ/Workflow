﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;
using Workflow.VM.ViewModels.Statistic;

namespace Workflow.Services
{
    public class StatisticService : IStatisticService
    {
        public StatisticService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<VmProjectStatistic> GetStatisticForProject(int projectId, StatisticOptions options)
        {
            var query = _dataContext.Goals
                .Where(g => g.ProjectId == projectId
                            && !g.IsRemoved);

            return await GetStatistic(query, options);
        }


        public async Task<VmProjectStatistic> GetStatisticForUser(string userId, StatisticOptions options)
        {
            var query = _dataContext.Goals
                .Include(g => g.Project)
                .Where(g => g.PerformerId == userId
                            && !g.IsRemoved);

            return await GetStatistic(query, options);
        }

        public async Task<VmProjectStatistic> GetStatisticForUserAndProject(string userId, int projectId, StatisticOptions options)
        {
            var query = _dataContext.Goals
                .Include(g => g.Project)
                .Where(g => g.PerformerId == userId
                            && g.ProjectId == projectId
                            && !g.IsRemoved);

            return await GetStatistic(query, options);
        }


        private async Task<VmProjectStatistic> GetStatistic(IQueryable<Goal> query, StatisticOptions options)
        {
            if (options != null)
            {
                query = query.Where(g => g.CreationDate >= options.DateBegin
                                         && g.CreationDate <= options.DateEnd);
            }

            var queryData = await query.ToArrayAsync();

            var commonStatistic = queryData.GroupBy(g => g.State)
                .Select(x => new
                {
                    State = x.Key,
                    Count = x.Count()
                })
                .ToArray();

            var byDayStatistic = queryData
                .GroupBy(x => (x.StateChangedDate ?? x.CreationDate).Date)
                .OrderBy(x => x.Key)
                .Select(x => new
                {
                    Date = x.Key,
                    Statistic = x.GroupBy(y => y.State).Select(y => new
                    {
                        State = y.Key,
                        Count = y.Count()
                    })
                })
                .ToArray();

            var vm = new VmProjectStatistic
            {
                GoalsCountForState = new[]
                {
                    commonStatistic.FirstOrDefault(s => s.State == GoalState.New)?.Count ?? 0,
                    commonStatistic.FirstOrDefault(s => s.State == GoalState.Perform)?.Count ?? 0,
                    commonStatistic.FirstOrDefault(s => s.State == GoalState.Delay)?.Count ?? 0,
                    commonStatistic.FirstOrDefault(s => s.State == GoalState.Testing)?.Count ?? 0,
                    commonStatistic.FirstOrDefault(s => s.State == GoalState.Succeed)?.Count ?? 0,
                    commonStatistic.FirstOrDefault(s => s.State == GoalState.Rejected)?.Count ?? 0
                },
                ByDateStatistics = byDayStatistic
                    .Select(x => new ByDateGoalStatistic(x.Date, new[]
                    {
                        x.Statistic.FirstOrDefault(s => s.State == GoalState.New)?.Count ?? 0,
                        x.Statistic.FirstOrDefault(s => s.State == GoalState.Perform)?.Count ?? 0,
                        x.Statistic.FirstOrDefault(s => s.State == GoalState.Delay)?.Count ?? 0,
                        x.Statistic.FirstOrDefault(s => s.State == GoalState.Testing)?.Count ?? 0,
                        x.Statistic.FirstOrDefault(s => s.State == GoalState.Succeed)?.Count ?? 0,
                        x.Statistic.FirstOrDefault(s => s.State == GoalState.Rejected)?.Count ?? 0
                    }))
                    .ToArray()
            };
            return vm;
        }


        private readonly DataContext _dataContext;
    }
}