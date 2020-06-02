using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;
using WorkflowService.Common;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Services
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
        public async Task<VmTeam> Create(ApplicationUser currentUser, VmTeam team)
        {
            if (team == null)
                throw new ArgumentNullException(nameof(team));

            if (string.IsNullOrWhiteSpace(team.Name))
                throw new InvalidOperationException("Scope name cannot be empty");

            var model = _vmConverter.ToModel(team);
            model.Id = 0;

            await _dataContext.Teams.AddAsync(model);
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }


        /// <inheritdoc />
        public async Task<VmTeam> Update(ApplicationUser currentUser, VmTeam team)
        {
            if (team == null)
                return null;

            if (string.IsNullOrWhiteSpace(team.Name))
                throw new InvalidOperationException("Scope name cannot be empty");

            var model = _vmConverter.ToModel(team);

            _dataContext.Teams.Update(model);
            await _dataContext.SaveChangesAsync();
            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task<VmTeam> Delete(ApplicationUser currentUser, int teamId)
        {
            var model = await GetQuery(true)
                .FirstOrDefaultAsync(s => s.Id == teamId);

            if (model != null)
            {
                model.IsRemoved = true;
                _dataContext.Teams.Update(model);
                await _dataContext.SaveChangesAsync();
                return _vmConverter.ToViewModel(model);
            }

            return null;
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
            foreach (var word in words)
            {
                query = query
                    .Where(team => team.Name.Contains(word)
                                   || team.Description.Contains(word)
                                   || team.Group.Name.Contains(word));
            }

            return query;
        }

        private IQueryable<Team> FilterByFields(FieldFilter[] filterFields, IQueryable<Team> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields)
            {
                if (field == null)
                    continue;

                var strValue = field.Value?.ToString()?.ToLower();
                if (field.Is(nameof(VmTeam.Name)))
                {
                    query = query.Where(t => t.Name.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmTeam.GroupName)))
                {
                    query = query.Where(t => t.Group.Name.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmTeam.Description)))
                {
                    query = query.Where(t => t.Description.ToLower().Contains(strValue));
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
            }

            return orderedQuery ?? query;
        }


        private readonly DataContext _dataContext;
        private readonly VmTeamConverter _vmConverter;
    }
}