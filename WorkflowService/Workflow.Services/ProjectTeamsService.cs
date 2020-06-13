using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Common;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class ProjectTeamsService : IProjectTeamsService
    {
        private readonly DataContext _dataContext;
        private readonly VmTeamConverter _vmConverter;


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataContext"></param>
        public ProjectTeamsService(DataContext dataContext)
        {
            _dataContext = dataContext;
            _vmConverter = new VmTeamConverter();
        }

        public async Task<IEnumerable<VmTeam>> GetPage(ApplicationUser currentUser, 
            int projectId, int pageNumber, int pageSize, string filter,
            FieldFilter[] filterFields, FieldSort[] sortFields, bool withRemoved = false)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = GetQuery(projectId, withRemoved);
            query = Filter(filter, query);
            query = FilterByFields(filterFields, query);
            query = SortByFields(sortFields, query);

            return await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .Select(pt => _vmConverter.ToViewModel(pt.Team))
                .ToArrayAsync();
        }

        public async Task Add(int projectId, int teamId)
        {
            await _dataContext.ProjectTeams.AddAsync(new ProjectTeam(projectId, teamId));
            await _dataContext.SaveChangesAsync();
        }

        public async Task Remove(int projectId, int teamId)
        {
            _dataContext.ProjectTeams.Remove(new ProjectTeam(projectId, teamId));
            await _dataContext.SaveChangesAsync();
        }

        private IQueryable<ProjectTeam> GetQuery(int projectId, in bool withRemoved)
        {
            var query = _dataContext.ProjectTeams.AsNoTracking()
                .Include(tu => tu.Project)
                .Include(tu => tu.Team)
                .Where(tu => tu.ProjectId == projectId);

            if (!withRemoved)
                query = query.Where(pt => pt.Team.IsRemoved == false);

            return query;
        }

        private IQueryable<ProjectTeam> Filter(string filter, IQueryable<ProjectTeam> query)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            var words = filter.Split(" ");
            foreach (var word in words.Select(w => w.ToLower()))
            {
                query = query
                    .Where(pt => pt.Team.Name.ToLower().Contains(word)
                                 || pt.Team.Description.ToLower().Contains(word)
                                 || pt.Team.Group.Name.ToLower().Contains(word)
                                 || pt.Team.Creator.FirstName.ToLower().Contains(word)
                                 || pt.Team.Creator.MiddleName.ToLower().Contains(word)
                                 || pt.Team.Creator.LastName.ToLower().Contains(word));
            }

            return query;
        }

        private IQueryable<ProjectTeam> FilterByFields(FieldFilter[] filterFields, IQueryable<ProjectTeam> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields.Where(ff => ff != null))
            {
                var strValues = field.Values?.Select(v => v.ToString().ToLower()).ToList()
                                ?? new List<string>();

                if (field.SameAs(nameof(VmTeam.Name)))
                {
                    var queries = strValues.Select(sv => query.Where(pt =>
                        pt.Team.Name.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmTeam.Description)))
                {
                    var queries = strValues.Select(sv => query.Where(pt =>
                        pt.Team.Description.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmTeam.GroupName)))
                {
                    var queries = strValues.Select(sv => query.Where(pt =>
                        pt.Team.Group.Name.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
            }

            return query;
        }

        private IQueryable<ProjectTeam> SortByFields(FieldSort[] sortFields, IQueryable<ProjectTeam> query)
        {
            if (sortFields == null) return query;

            IOrderedQueryable<ProjectTeam> orderedQuery = null;
            foreach (var field in sortFields.Where(sf => sf != null))
            {
                if (field.Is(nameof(VmTeam.Name)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(pt => pt.Team.Name)
                            : query.OrderByDescending(pt => pt.Team.Name);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(pt => pt.Team.Name)
                            : orderedQuery.ThenByDescending(pt => pt.Team.Name);
                    }
                }
                else if (field.Is(nameof(VmTeam.Description)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(pt => pt.Team.Description)
                            : query.OrderByDescending(pt => pt.Team.Description);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(pt => pt.Team.Description)
                            : orderedQuery.ThenByDescending(pt => pt.Team.Description);
                    }
                }
                else if (field.Is(nameof(VmTeam.GroupName)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(pt => pt.Team.Group.Name)
                            : query.OrderByDescending(pt => pt.Team.Group.Name);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(tu => tu.Team.Group.Name)
                            : orderedQuery.ThenByDescending(tu => tu.Team.Group.Name);
                    }
                }
            }

            return orderedQuery ?? query;
        }
    }
}