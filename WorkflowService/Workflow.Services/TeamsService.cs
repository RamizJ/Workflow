using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Share.Extensions;
using Workflow.VM.Common;
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

            var team = await GetQuery(currentUser, true)
                .FirstOrDefaultAsync(s => s.Id == id);

            return _vmConverter.ToViewModel(team);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmTeam>> GetAll(ApplicationUser currentUser, bool withRemoved = false)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = GetQuery(currentUser, withRemoved);
            var teams = await query
                .Select(t => _vmConverter.ToViewModel(t))
                .ToArrayAsync();

            return teams;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmTeam>> GetPage(ApplicationUser currentUser, PageOptions pageOptions)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if (pageOptions == null)
                throw new ArgumentNullException(nameof(pageOptions));

            var query = GetQuery(currentUser, pageOptions.WithRemoved);
            query = Filter(pageOptions.Filter, query);
            query = FilterByFields(pageOptions.FilterFields, query);
            query = SortByFields(pageOptions.SortFields, query);

            return await query
                .Skip(pageOptions.PageNumber * pageOptions.PageSize)
                .Take(pageOptions.PageSize)
                .Select(s => _vmConverter.ToViewModel(s))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmTeam>> GetRange(ApplicationUser currentUser, int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return null;

            return await GetQuery(currentUser, true)
                .Where(s => ids.Any(id => s.Id == id))
                .Select(s => _vmConverter.ToViewModel(s))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<VmTeam> Create(ApplicationUser currentUser, VmTeam team)
        {
            var model = await CreateTeam(currentUser, team);
            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task<VmTeamForm> CreateByForm(ApplicationUser currentUser, VmTeamForm teamForm)
        {
            var model = await CreateTeam(currentUser, teamForm?.Team, team =>
            {
                team.TeamProjects = teamForm?.ProjectIds?
                    .Select(pId => new ProjectTeam(pId, team.Id))
                    .ToList();
                team.TeamUsers = teamForm?.UserIds?
                    .Select(uId => new TeamUser(team.Id, uId))
                    .ToList();
            });

            return new VmTeamForm
            {
                Team = _vmConverter.ToViewModel(model),
                ProjectIds = teamForm?.ProjectIds,
                UserIds = teamForm?.UserIds
            };
        }


        /// <inheritdoc />
        public async Task Update(ApplicationUser currentUser, VmTeam team)
        {
            await UpdateTeams(currentUser, new[] { team });
        }

        /// <inheritdoc />
        public async Task UpdateByForm(ApplicationUser currentUser, VmTeamForm teamForm)
        {
            await UpdateTeams(currentUser, new[] {teamForm?.Team}, team =>
            {
                team.TeamProjects = teamForm?.ProjectIds?
                    .Select(pId => new ProjectTeam(pId, team.Id))
                    .ToList();
                team.TeamUsers = teamForm?.UserIds?
                    .Select(uId => new TeamUser(team.Id, uId))
                    .ToList();
            });
        }

        /// <inheritdoc />
        public async Task UpdateRange(ApplicationUser currentUser, IEnumerable<VmTeam> vmTeams)
        {
            await UpdateTeams(currentUser, vmTeams.ToArray());
        }

        /// <inheritdoc />
        public async Task UpdateByFormRange(ApplicationUser currentUser, IEnumerable<VmTeamForm> teamForms)
        {
            var forms = teamForms.ToArray();
            var teams = forms.Select(f => f.Team).ToArray();
            await UpdateTeams(currentUser, teams, team =>
            {
                var form = forms.First(f => f.Team.Id == team.Id);
                team.TeamProjects = form?.ProjectIds?
                    .Select(pId => new ProjectTeam(pId, team.Id))
                    .ToList();
                team.TeamUsers = form?.UserIds?
                    .Select(uId => new TeamUser(team.Id, uId))
                    .ToList();
            });
        }

        /// <inheritdoc />
        public async Task<VmTeam> Delete(ApplicationUser currentUser, int teamId)
        {
            var teams = await RemoveRestore(currentUser, new[] { teamId }, true);
            return teams.FirstOrDefault();
        }

        public async Task<IEnumerable<VmTeam>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, true);
        }

        /// <inheritdoc />
        public async Task<VmTeam> Restore(ApplicationUser currentUser, int teamId)
        {
            var teams = await RemoveRestore(currentUser, new[] {teamId}, false);
            return teams.FirstOrDefault();
        }

        public async Task<IEnumerable<VmTeam>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, false);
        }

        private async Task<IEnumerable<VmTeam>> RemoveRestore(ApplicationUser currentUser, 
            IEnumerable<int> teamIds, bool isRemoved)
        {
            var query = GetQuery(currentUser, !isRemoved);
            var models = await query
                .Where(t => teamIds.Any(tId => t.Id == tId))
                .ToArrayAsync();

            foreach (var model in models)
            {
                model.IsRemoved = isRemoved;
                _dataContext.Entry(model).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
            return models.Select(m => _vmConverter.ToViewModel(m));
        }

        private IQueryable<Team> GetQuery(ApplicationUser currentUser, bool withRemoved)
        {
            var query = _dataContext.Teams
                .Include(t => t.Group)
                .Include(t => t.TeamUsers)
                .Include(t => t.TeamProjects)
                .Where(t => t.Creator.Id == currentUser.Id
                            || t.TeamUsers.Any(tu => tu.UserId == currentUser.Id))
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
                                   || team.Group.Name.ToLower().Contains(word)
                                   || team.Creator.FirstName.ToLower().Contains(word)
                                   || team.Creator.MiddleName.ToLower().Contains(word)
                                   || team.Creator.LastName.ToLower().Contains(word));
            }

            return query;
        }

        private IQueryable<Team> FilterByFields(FieldFilter[] filterFields, IQueryable<Team> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields.Where(ff => ff != null))
            {
                var strValues = field.Values?
                                    
                                    .Select(v => v.ToString()?.ToLower()).ToList()
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
                    var values = field.Values.OfType<bool>().ToArray();
                    query = query.Where(t => values.Any(v => v == t.IsRemoved));
                }
            }

            return query;
        }

        private IQueryable<Team> SortByFields(FieldSort[] sortFields, IQueryable<Team> query)
        {
            if (sortFields == null) return query;

            foreach (var field in sortFields)
            {
                if (field == null) continue;

                var isAcending = field.SortType == SortType.Ascending;

                if (field.Is(nameof(VmTeam.Name)))
                    query = query.SortBy(t => t.Name, isAcending);

                else if (field.Is(nameof(VmTeam.Description)))
                    query = query.SortBy(t => t.Description, isAcending);

                else if (field.Is(nameof(VmTeam.GroupName)))
                    query = query.SortBy(t => t.Group.Name, isAcending);

                else if (field.Is(nameof(VmTeam.IsRemoved))) 
                    query = query.SortBy(t => t.IsRemoved, isAcending);
            }

            return query;
        }

        private async Task<Team> CreateTeam(ApplicationUser currentUser, VmTeam team, 
            Action<Team> createAction = null)
        {
            if (team == null)
                throw new ArgumentNullException(nameof(team));

            if (string.IsNullOrWhiteSpace(team.Name))
                throw new InvalidOperationException("Team name cannot be empty");

            var model = _vmConverter.ToModel(team);
            model.Id = 0;
            model.CreatorId = currentUser.Id;

            createAction?.Invoke(model);

            await _dataContext.Teams.AddAsync(model);
            await _dataContext.SaveChangesAsync();
            return model;
        }

        private async Task UpdateTeams(ApplicationUser currentUser, 
            ICollection<VmTeam> teams, Action<Team> updateAction = null)
        {
            if (teams == null)
                throw new ArgumentNullException(nameof(teams));

            var teamIds = teams
                .Where(t => !string.IsNullOrWhiteSpace(t.Name))
                .Select(t => t.Id);

            var query = GetQuery(currentUser, true);
            var models = await query
                .Where(t => teamIds.Any(tId => t.Id == tId))
                .ToArrayAsync();

            foreach (var model in models)
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                    throw new InvalidOperationException("Cannot update team. The name cannot be empty");

                var team = teams.First(t => t.Id == model.Id);
                model.Name = team.Name;
                model.Description = team.Description;
                model.GroupId = team.GroupId;
                updateAction?.Invoke(model);
                _dataContext.Entry(model).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
        }


        private readonly DataContext _dataContext;
        private readonly VmTeamConverter _vmConverter;
    }
}