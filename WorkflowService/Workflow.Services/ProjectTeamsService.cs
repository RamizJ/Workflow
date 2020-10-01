using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.Share.Extensions;
using Workflow.VM.Common;
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
            await _dataContext.ProjectTeamRoles.AddAsync(new ProjectTeamRole(projectId, teamId));
            await _dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task Remove(int projectId, int teamId)
        {
            _dataContext.ProjectTeams.Remove(new ProjectTeam(projectId, teamId));

            bool isTeamRoleExist = await _dataContext.ProjectTeamRoles
                .AsNoTracking()
                .AnyAsync(ptr => ptr.ProjectId == projectId && ptr.TeamId == teamId);

            if (isTeamRoleExist)
            {
                _dataContext.ProjectTeamRoles.Remove(new ProjectTeamRole(projectId, teamId));
            }
                
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

            foreach (var field in sortFields.Where(sf => sf != null))
            {
                var isAcending = field.SortType == SortType.Ascending;

                if (field.Is(nameof(VmTeam.Name)))
                    query = query.SortBy(pt => pt.Team.Name, isAcending);

                else if (field.Is(nameof(VmTeam.Description)))
                    query = query.SortBy(pt => pt.Team.Description, isAcending);

                else if (field.Is(nameof(VmTeam.GroupName))) 
                    query = query.SortBy(pt => pt.Team.Group.Name, isAcending);

                else if (field.Is(nameof(VmTeam.IsRemoved)))
                    query = query.SortBy(pt => pt.Team.IsRemoved, isAcending);
            }

            return query;
        }
    }
}