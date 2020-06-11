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
    public class TeamsService : ITeamsService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public TeamsService(DataContext dataContext)
        {
            _dataContext = dataContext;
            _vmConverter = new VmTeamConverter();
        }

        /// <inheritdoc />
        public async Task<VmTeam> Get(ApplicationUser currentUser, int id)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var team = await GetQuery(true)
                .FirstOrDefaultAsync(s => s.Id == id);

            return _vmConverter.ToViewModel(team);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmTeam>> GetAll(ApplicationUser currentUser, bool withRemoved = false)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = GetQuery(withRemoved);
            var teams = await query
                .Select(t => _vmConverter.ToViewModel(t))
                .ToArrayAsync();

            return teams;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmTeam>> GetPage(ApplicationUser currentUser, int pageNumber, int pageSize, 
            string filter, FieldFilter[] filterFields,
            FieldSort[] sortFields, bool withRemoved = false)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = GetQuery(withRemoved);
            query = Filter(filter, query);
            query = FilterByFields(filterFields, query);
            query = SortByFields(sortFields, query);

            return await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .Select(s => _vmConverter.ToViewModel(s))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmTeam>> GetRange(ApplicationUser currentUser, int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return null;

            return await GetQuery(true)
                .Where(s => ids.Any(id => s.Id == id))
                .Select(s => _vmConverter.ToViewModel(s))
                .ToArrayAsync();
        }


        /// <inheritdoc />
        public async Task<VmTeam> Create(ApplicationUser currentUser, int projectId, VmTeam team)
        {
            if (team == null)
                throw new ArgumentNullException(nameof(team));

            if (string.IsNullOrWhiteSpace(team.Name))
                throw new InvalidOperationException("Team name cannot be empty");

            var model = _vmConverter.ToModel(team);
            model.Id = 0;
            model.CreatorId = currentUser.Id;
            model.TeamProjects.Add(new ProjectTeam(0, projectId));

            await _dataContext.Teams.AddAsync(model);
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }


        /// <inheritdoc />
        public async Task Update(ApplicationUser currentUser, VmTeam team)
        {
            if (team == null)
                throw new ArgumentNullException(nameof(team));

            if (string.IsNullOrWhiteSpace(team.Name))
                throw new InvalidOperationException("Team name cannot be empty");

            var model = await _dataContext.Teams.FindAsync(team.Id);
            if(model == null)
                throw new InvalidOperationException("Cannot update team. Team not found");

            model.Name = team.Name;
            model.Description = team.Description;
            model.GroupId = team.GroupId;

            _dataContext.Entry(model).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<VmTeam> Delete(ApplicationUser currentUser, int teamId)
        {
            return await ChangeRemoveState(teamId, true);
        }

        public async Task<VmTeam> Restore(ApplicationUser currentUser, int teamId)
        {
            return await ChangeRemoveState(teamId, false);
        }

        private async Task<VmTeam> ChangeRemoveState(int teamId, bool isRemoved)
        {
            var model = await _dataContext.Teams.FindAsync(teamId);
            if (model == null)
                throw new InvalidOperationException(isRemoved 
                    ? "Cannot delete the team. The team not found"
                    : "Cannot restore the team. The team not found");

            model.IsRemoved = isRemoved;
            _dataContext.Entry(model).State = EntityState.Modified;

            await _dataContext.SaveChangesAsync();
            return _vmConverter.ToViewModel(model);
        }

        private IQueryable<Team> GetQuery(bool withRemoved)
        {
            var query = _dataContext.Teams
                .Include(t => t.Group)
                .Include(t => t.TeamUsers)
                .AsQueryable();

            if (!withRemoved)
                query = query.Where(t => t.IsRemoved == false);

            return query;
        }

        private IQueryable<Team> Filter(string filter, IQueryable<Team> query)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            var words = filter.Split(" ");
            foreach (var word in words.Select(w => w.ToLower()))
            {
                query = query
                    .Where(team => team.Name.ToLower().Contains(word)
                                   || team.Description.ToLower().Contains(word)
                                   || team.Group.Name.ToLower().Contains(word));
            }

            return query;
        }

        private IQueryable<Team> FilterByFields(FieldFilter[] filterFields, IQueryable<Team> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields.Where(ff => ff != null))
            {
                var strValues = field.Values?.Select(v => v.ToString().ToLower()).ToList()
                                ?? new List<string>();

                if (field.SameAs(nameof(VmTeam.Name)))
                {
                    var queries = strValues.Select(sv => query.Where(t =>
                        t.Name.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmTeam.GroupName)))
                {
                    var queries = strValues.Select(sv => query.Where(t =>
                        t.Group.Name.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmTeam.Description)))
                {
                    var queries = strValues.Select(sv => query.Where(t =>
                        t.Description.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmTeam.IsRemoved)))
                {
                    var vals = field.Values.OfType<bool>().ToArray();
                    query = query.Where(t => vals.Any(v => v == t.IsRemoved));
                }
            }

            return query;
        }

        private IQueryable<Team> SortByFields(FieldSort[] sortFields, IQueryable<Team> query)
        {
            if (sortFields == null) return query;

            IOrderedQueryable<Team> orderedQuery = null;
            foreach (var field in sortFields)
            {
                if (field == null)
                    continue;

                if (field.Is(nameof(VmTeam.Name)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(t => t.Name)
                            : query.OrderByDescending(t => t.Name);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Name)
                            : orderedQuery.ThenByDescending(s => s.Name);
                    }
                }
                else if (field.Is(nameof(VmTeam.Description)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(t => t.Description)
                            : query.OrderByDescending(t => t.Description);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Description)
                            : orderedQuery.ThenByDescending(s => s.Description);
                    }
                }
                else if (field.Is(nameof(VmTeam.GroupName)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(s => s.Group.Name)
                            : query.OrderByDescending(s => s.Group.Name);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Group.Name)
                            : orderedQuery.ThenByDescending(s => s.Group.Name);
                    }
                }
                else if (field.Is(nameof(VmTeam.IsRemoved)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(s => s.IsRemoved)
                            : query.OrderByDescending(s => s.IsRemoved);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.IsRemoved)
                            : orderedQuery.ThenByDescending(s => s.IsRemoved);
                    }
                }
            }

            return orderedQuery ?? query;
        }


        private readonly DataContext _dataContext;
        private readonly VmTeamConverter _vmConverter;
    }
}