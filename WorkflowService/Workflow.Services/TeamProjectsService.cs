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
using Workflow.VM.ViewModels;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class TeamProjectsService : ITeamProjectsService
    {
        private readonly DataContext _dataContext;
        private readonly VmProjectConverter _vmConverter;


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataContext"></param>
        public TeamProjectsService(DataContext dataContext)
        {
            _dataContext = dataContext;
            _vmConverter = new VmProjectConverter();
        }


        /// <inheritdoc />
        public async Task<IEnumerable<VmProject>> GetPage(ApplicationUser currentUser, 
            int teamId, PageOptions pageOptions)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if (pageOptions == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest,
                    $"Parameter '{nameof(pageOptions)}' cannot be null");

            var query = GetQuery(teamId, pageOptions.WithRemoved);
            query = Filter(pageOptions.Filter, query);
            query = FilterByFields(pageOptions.FilterFields, query);
            query = SortByFields(pageOptions.SortFields, query);

            return await query
                .Skip(pageOptions.PageNumber * pageOptions.PageSize)
                .Take(pageOptions.PageSize)
                .Select(pt => _vmConverter.ToViewModel(pt.Project))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task Add(int teamId, int projectId)
        {
            try
            {
                await _dataContext.ProjectTeams.AddAsync(new ProjectTeam(projectId, teamId));
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                bool isExist = await _dataContext.ProjectTeams
                    .AnyAsync(pt =>
                        pt.ProjectId == projectId
                        && pt.TeamId == teamId);

                throw new HttpResponseException(isExist 
                    ? HttpStatusCode.Conflict
                    : HttpStatusCode.BadRequest);
            }
        }

        /// <inheritdoc />
        public async Task Remove(int teamId, int projectId)
        {
            try
            {
                _dataContext.ProjectTeams.Remove(new ProjectTeam(projectId, teamId));
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                bool isExist = await _dataContext.ProjectTeams
                    .AnyAsync(pt =>
                        pt.ProjectId == projectId
                        && pt.TeamId == teamId);

                throw new HttpResponseException(isExist 
                    ? HttpStatusCode.BadRequest 
                    : HttpStatusCode.NotFound);
            }
        }

        private IQueryable<ProjectTeam> GetQuery(int teamId, in bool withRemoved)
        {
            var query = _dataContext.ProjectTeams.AsNoTracking()
                .Include(tp => tp.Project)
                .Include(tp => tp.Team)
                .Where(tp => tp.TeamId == teamId);

            if (!withRemoved)
                query = query.Where(pt => pt.Project.IsRemoved == false);

            return query;
        }

        private IQueryable<ProjectTeam> Filter(string filter, IQueryable<ProjectTeam> query)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            var words = filter.Split(" ");
            foreach (var word in words.Select(w => w.ToLower()))
            {
                bool isDate = DateTime.TryParse(word, out var creationDate);
                query = query
                    .Where(pt => pt.Project.Name.ToLower().Contains(word)
                                 || pt.Project.Description.ToLower().Contains(word)
                                 || pt.Project.Group.Name.ToLower().Contains(word)
                                 || isDate && pt.Project.CreationDate == creationDate
                                 || pt.Project.Owner.FirstName.ToLower().Contains(word)
                                 || pt.Project.Owner.MiddleName.ToLower().Contains(word)
                                 || pt.Project.Owner.LastName.ToLower().Contains(word));
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

                if (field.SameAs(nameof(VmProject.Name)))
                {
                    var queries = strValues.Select(sv => query.Where(pt =>
                        pt.Project.Name.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmProject.Description)))
                {
                    var queries = strValues.Select(sv => query.Where(pt =>
                        pt.Project.Description.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmProject.GroupName)))
                {
                    var queries = strValues.Select(sv => query.Where(pt =>
                        pt.Project.Group.Name.ToLower().Contains(sv))).ToArray();

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
                if (field.Is(nameof(VmProject.Name)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(pt => pt.Project.Name)
                            : query.OrderByDescending(pt => pt.Project.Name);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(pt => pt.Project.Name)
                            : orderedQuery.ThenByDescending(pt => pt.Project.Name);
                    }
                }
                else if (field.Is(nameof(VmProject.Description)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(pt => pt.Project.Description)
                            : query.OrderByDescending(pt => pt.Project.Description);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(pt => pt.Project.Description)
                            : orderedQuery.ThenByDescending(pt => pt.Project.Description);
                    }
                }
            }

            return orderedQuery ?? query;
        }
    }
}