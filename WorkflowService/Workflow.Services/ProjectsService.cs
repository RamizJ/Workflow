using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.Share.Extensions;
using Workflow.VM.Common;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;
using static System.Net.HttpStatusCode;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class ProjectsService : IProjectsService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="userManager"></param>
        public ProjectsService(DataContext dataContext, UserManager<ApplicationUser> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _vmConverter = new VmProjectConverter();
        }


        /// <inheritdoc />
        public async Task<VmProject> Get(ApplicationUser user, int id)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var query = await GetQuery(user, true);
            var project = await query.FirstOrDefaultAsync(s => s.Id == id);

            return _vmConverter.ToViewModel(project);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmProject>> GetPage(ApplicationUser currentUser, PageOptions pageOptions)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if(pageOptions == null)
                throw new HttpResponseException(BadRequest,
                    $"Parameter '{nameof(pageOptions)}' cannot be null");

            var query = await GetQuery(currentUser, pageOptions.WithRemoved);
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
        public async Task<IEnumerable<VmProject>> GetRange(ApplicationUser user, int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return null;

            var query = await GetQuery(user, true);
            return await query.Where(s => ids.Any(id => s.Id == id))
                .Select(s => _vmConverter.ToViewModel(s))
                .ToArrayAsync();
        }


        /// <inheritdoc />
        public async Task<VmProject> Create(ApplicationUser user, VmProject project)
        {
            var model = await CreateProject(user, project);
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task<VmProject> CreateByForm(ApplicationUser user, VmProjectForm projectForm)
        {
            var model = await CreateProject(user, projectForm?.Project);
            model.ProjectTeams = projectForm?.TeamIds?
                .Select(tId => new ProjectTeam(0, tId))
                .ToList();

            await _dataContext.SaveChangesAsync();
            return _vmConverter.ToViewModel(model);
        }


        /// <inheritdoc />
        public async Task Update(ApplicationUser user, VmProject project)
        {
            await UpdateProjects(user, new[] { project });
        }

        /// <inheritdoc />
        public async Task UpdateByForm(ApplicationUser user, VmProjectForm projectForm)
        {
            await UpdateProjects(user, new[] {projectForm?.Project}, project =>
            {
                project.ProjectTeams = projectForm?.TeamIds?
                    .Select(tId => new ProjectTeam(project.Id, tId))
                    .ToList();
            });
        }

        /// <inheritdoc />
        public async Task UpdateRange(ApplicationUser currentUser, IEnumerable<VmProject> projects)
        {
            await UpdateProjects(currentUser, projects.ToArray());
            await _dataContext.SaveChangesAsync();
        }

        

        /// <inheritdoc />
        public async Task UpdateByFormRange(ApplicationUser currentUser,
            IEnumerable<VmProjectForm> projectForms)
        {
            var forms = projectForms.ToArray();
            var projects = forms
                .Select(pf => pf.Project)
                .Where(p => p != null)
                .ToArray();

            await UpdateProjects(currentUser, projects, project =>
            {
                var form = forms.First(pf => pf.Project.Id == project.Id);
                project.ProjectTeams = form?.TeamIds?
                    .Select(tId => new ProjectTeam(project.Id, tId))
                    .ToList();
            });
        }

        /// <inheritdoc />
        public async Task<VmProject> Delete(ApplicationUser user, int projectId)
        {
            var removedProjects = await RemoveRestore(user, new[] {projectId}, true);
            return removedProjects.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmProject>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, true);
        }

        /// <inheritdoc />
        public async Task<VmProject> Restore(ApplicationUser currentUser, int projectId)
        {
            var projects = await RemoveRestore(currentUser, new[] { projectId }, false);
            return projects.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmProject>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, false);
        }

        /// <inheritdoc />
        public async Task<VmProjectStatistic> GetStatistic(int projectId, ProjectStatisticOptions options)
        {
            var query = _dataContext.Goals
                .Where(g => g.ProjectId == projectId);

            if (options != null)
            {
                query = query.Where(g => g.CreationDate >= options.DateBegin
                                         && g.CreationDate <= options.DateEnd);
            }

            var statistic = await query.GroupBy(g => g.State)
                .Select(x => new
                {
                    State = x.Key,
                    Count = x.Count()
                })
                .ToArrayAsync();

            var vm = new VmProjectStatistic
            {
                GoalsCountForState =
                {
                    [(int) GoalState.New] = statistic.FirstOrDefault(s => s.State == GoalState.New)?.Count ?? 0,
                    [(int) GoalState.Perform] = statistic.FirstOrDefault(s => s.State == GoalState.Perform)?.Count ?? 0,
                    [(int) GoalState.Delay] = statistic.FirstOrDefault(s => s.State == GoalState.Delay)?.Count ?? 0,
                    [(int) GoalState.Testing] = statistic.FirstOrDefault(s => s.State == GoalState.Testing)?.Count ?? 0,
                    [(int) GoalState.Succeed] = statistic.FirstOrDefault(s => s.State == GoalState.Succeed)?.Count ?? 0,
                    [(int) GoalState.Rejected] = statistic.FirstOrDefault(s => s.State == GoalState.Rejected)?.Count ?? 0
                }
            };
            return vm;
        }


        private async Task<IEnumerable<VmProject>> RemoveRestore(ApplicationUser user, 
            IEnumerable<int> projectIds, bool isRemoved)
        {
            var query = await GetQuery(user, !isRemoved);
            var models = await query
                .Where(p => projectIds.Any(pId => p.Id == pId))
                .ToArrayAsync();

            foreach (var model in models)
            {
                model.IsRemoved = isRemoved;
                _dataContext.Entry(model).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
            return models.Select(m => _vmConverter.ToViewModel(m));
        }

        private async Task<IQueryable<Project>> GetQuery(ApplicationUser currentUser, bool withRemoved)
        {
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, RoleNames.ADMINISTRATOR_ROLE);
            var query = _dataContext.Projects
                .Include(s => s.Owner)
                .Include(s => s.ProjectTeams)
                .Include(s => s.Group)
                .Where(s => isAdmin
                            || s.OwnerId == currentUser.Id
                            || s.ProjectTeams
                                .Any(pt => pt.Team.CreatorId == currentUser.Id
                                           || pt.Team.TeamUsers
                                               .Any(tu => tu.UserId == currentUser.Id)));

            if (!withRemoved)
                query = query.Where(s => s.IsRemoved == false);

            return query;
        }

        private static IQueryable<Project> Filter(string filter, IQueryable<Project> query)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            var words = filter.Split(" ");
            foreach (var word in words.Select(w => w.ToLower()))
            {
                bool isDate = DateTime.TryParse(word, out var creationDate);
                query = query
                    .Where(p => p.Name.ToLower().Contains(word)
                                || p.Description.ToLower().Contains(word)
                                || p.Group.Name.ToLower().Contains(word)
                                || isDate && p.CreationDate == creationDate
                                || p.Owner.FirstName.ToLower().Contains(word)
                                || p.Owner.MiddleName.ToLower().Contains(word)
                                || p.Owner.LastName.ToLower().Contains(word));
            }

            return query;
        }

        private static IQueryable<Project> FilterByFields(FieldFilter[] filterFields, IQueryable<Project> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields.Where(ff => ff != null))
            {
                var strValues = field.Values?
                                    .Select(v => v?.ToString()?.ToLower())
                                    .Where(v => v != null)
                                    .ToList()
                                ?? new List<string>();

                if (field.SameAs(nameof(VmProject.Name)))
                {
                    var queries = strValues.Select(sv => query.Where(p =>
                        p.Name.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmProject.GroupName)))
                {
                    var queries = strValues.Select(sv => query.Where(p =>
                        p.Group.Name.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmProject.IsRemoved)))
                {
                    var values = field.Values.OfType<bool>().ToArray();
                    query = query.Where(p => values.Any(v => v == p.IsRemoved));
                }
                else if (field.SameAs(nameof(VmProject.CreationDate)))
                {
                    var values = field.Values.OfType<DateTime>().ToArray();
                    query = query.Where(p => values.Any(v => v == p.CreationDate));
                }
                else if (field.SameAs(nameof(VmProject.ExpectedCompletedDate)))
                {
                    var values = field.Values.OfType<DateTime>().ToArray();
                    query = query.Where(p => values.Any(v => v == p.ExpectedCompletedDate));
                }
                else if (field.SameAs(nameof(VmProject.OwnerFio)))
                {
                    var names = strValues.SelectMany(sv => sv.Split(" "));

                    var queries = names.Select(name => query.Where(p =>
                            p.Owner.LastName.ToLower().Contains(name)
                            || p.Owner.FirstName.ToLower().Contains(name)
                            || p.Owner.MiddleName.ToLower().Contains(name)))
                        .ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmProject.IsRemoved)))
                {
                    var values = field.Values.OfType<bool>().ToArray();
                    query = query.Where(p => values.Any(v => v == p.IsRemoved));
                }
            }

            return query;
        }

        private IQueryable<Project> SortByFields(FieldSort[] sortFields, IQueryable<Project> query)
        {
            if (sortFields == null) return query;

            foreach (var field in sortFields.Where(sf => sf != null))
            {
                var isAcending = field.SortType == SortType.Ascending;

                if (field.Is(nameof(VmProject.Name)))
                    query = query.SortBy(p => p.Name, isAcending);

                else if (field.Is(nameof(VmProject.GroupName)))
                    query = query.SortBy(p => p.Group.Name, isAcending);

                else if (field.Is(nameof(VmProject.CreationDate)))
                    query = query.SortBy(p => p.CreationDate, isAcending);

                else if (field.Is(nameof(VmProject.ExpectedCompletedDate)))
                    query = query.SortBy(p => p.ExpectedCompletedDate, isAcending);

                else if (field.Is(nameof(VmProject.IsRemoved)))
                    query = query.SortBy(p => p.IsRemoved, isAcending);

                else if (field.Is(nameof(VmProject.OwnerFio)))
                    query = query
                        .SortBy(p => p.Owner.LastName, isAcending)
                        .SortBy(p => p.Owner.FirstName, isAcending)
                        .SortBy(p => p.Owner.MiddleName, isAcending);
            }

            return query;
        }

        private async Task<Project> CreateProject(ApplicationUser user, VmProject project)
        {
            if (project == null)
                throw new HttpResponseException(BadRequest,
                    $"Parameter '{nameof(project)}' cannot be null");

            if (string.IsNullOrWhiteSpace(project.Name))
                throw new HttpResponseException(BadRequest, 
                    "Cannot create project. The name cannot be empty");

            var model = _vmConverter.ToModel(project);
            model.Id = 0;
            model.CreationDate = DateTime.Now;
            model.OwnerId = user.Id;
            await _dataContext.Projects.AddAsync(model);

            return model;
        }

        private async Task UpdateProjects(ApplicationUser currentUser, 
            ICollection<VmProject> projects, Action<Project> updateAction = null)
        {
            var projectIds = projects
                .Where(p => p != null)
                .Select(p => p.Id)
                .ToArray();

            var query = await GetQuery(currentUser, true);
            var models = await query
                .Where(p => projectIds.Any(pId => pId == p.Id))
                .ToArrayAsync();

            foreach (var model in models)
            {
                var project = projects.First(p => p.Id == model.Id);
                if (string.IsNullOrWhiteSpace(project.Name))
                    throw new HttpResponseException(BadRequest, 
                        "Cannot update project. The name cannot be empty");

                model.Name = project.Name;
                model.Description = project.Description;

                updateAction?.Invoke(model);

                _dataContext.Entry(model).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
        }


        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly VmProjectConverter _vmConverter;
    }
}