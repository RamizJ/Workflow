using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class ProjectTeamsService : IProjectTeamsService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="vmTeamRoleConverter"></param>
        public ProjectTeamsService(DataContext dataContext, 
            IViewModelConverter<ProjectTeam, VmProjectTeamRole> vmTeamRoleConverter)
        {
            _dataContext = dataContext;
            _vmTeamRoleConverter = vmTeamRoleConverter;
            _vmConverter = new VmTeamConverter();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmTeam>> GetPage(ApplicationUser currentUser, 
            int projectId, PageOptions pageOptions)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if (pageOptions == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest,
                    $"Parameter '{nameof(pageOptions)}' cannot be null");

            var query = GetQuery(projectId, pageOptions.WithRemoved);
            query = Filter(pageOptions.Filter, query);
            query = FilterByFields(pageOptions.FilterFields, query);
            query = SortByFields(pageOptions.SortFields, query);

            return await query
                .Skip(pageOptions.PageNumber * pageOptions.PageSize)
                .Take(pageOptions.PageSize)
                .Select(pt => _vmConverter.ToViewModel(pt.Team))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task Add(int projectId, int teamId)
        {
            await _dataContext.ProjectTeams.AddAsync(new ProjectTeam(projectId, teamId));
            await _dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task Remove(int projectId, int teamId)
        {
            _dataContext.ProjectTeams.Remove(new ProjectTeam(projectId, teamId));
            await _dataContext.SaveChangesAsync();
        }

        public async Task<VmProjectTeamRole> GetRole(int projectId, int teamId)
        {
            var projectTeam = await _dataContext.ProjectTeams
                .FirstOrDefaultAsync(pt => pt.ProjectId == projectId
                                           && pt.TeamId == teamId);

            if (projectTeam == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return _vmTeamRoleConverter.ToViewModel(projectTeam);
        }

        public async Task UpdateTeamRole(VmProjectTeamRole role)
        {
            var projectTeam = _vmTeamRoleConverter.ToModel(role);

            try
            {
                _dataContext.Entry(projectTeam).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, 
                    $"Cannot remove team with id: {projectTeam.TeamId} " +
                    $"from project with id: {projectTeam.ProjectId}");
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        public async Task UpdateTeamsRoles(IEnumerable<VmProjectTeamRole> roles)
        {
            var models = roles.Select(_vmTeamRoleConverter.ToModel);

            try
            {
                foreach (var model in models) 
                    _dataContext.Entry(model).State = EntityState.Modified;

                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
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
                var strValues = field.Values?.Select(v => v.ToString()?.ToLower()).ToList()
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
            }

            return query;
        }

        private IQueryable<ProjectTeam> SortByFields(FieldSort[] sortFields, IQueryable<ProjectTeam> query)
        {
            if (sortFields == null) return query;

            foreach (var field in sortFields.Where(sf => sf != null))
            {
                var isAcending = field.SortType == SortType.Ascending;

                if (field.Is(nameof(VmTeam.Name)))
                    query = query.SortBy(pt => pt.Team.Name, isAcending);

                else if (field.Is(nameof(VmTeam.Description)))
                    query = query.SortBy(pt => pt.Team.Description, isAcending);

                else if (field.Is(nameof(VmTeam.IsRemoved)))
                    query = query.SortBy(pt => pt.Team.IsRemoved, isAcending);
            }

            return query;
        }


        private readonly DataContext _dataContext;
        private readonly IViewModelConverter<ProjectTeam, VmProjectTeamRole> _vmTeamRoleConverter;
        private readonly VmTeamConverter _vmConverter;
    }
}