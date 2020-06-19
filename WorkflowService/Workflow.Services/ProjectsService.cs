using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
        public async Task<IEnumerable<VmProject>> GetPage(ApplicationUser user, 
            int pageNumber, int pageSize, 
            string filter, FieldFilter[] filterFields, FieldSort[] sortFields,
            bool withRemoved = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var query = await GetQuery(user, withRemoved);
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
        public async Task<VmProject> Create(ApplicationUser user, VmProjectForm projectForm)
        {
            var project = projectForm?.Project;
            if (project == null)
                throw new ArgumentNullException(nameof(VmProjectForm.Project));

            if (string.IsNullOrWhiteSpace(project.Name))
                throw new InvalidOperationException("Cannot create project. The name cannot be empty");

            var model = _vmConverter.ToModel(project);
            model.Id = 0;
            model.OwnerId = user.Id;
            model.ProjectTeams = projectForm.TeamIds?
                .Select(tId => new ProjectTeam(0, tId))
                .ToList();

            await _dataContext.Projects.AddAsync(model);
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task Update(ApplicationUser user, VmProjectForm projectForm)
        {
            await UpdateProject(user, projectForm);
        }

        public async Task UpdateRange(ApplicationUser currentUser, 
            IEnumerable<VmProjectForm> projectForms)
        {
            foreach (var projectForm in projectForms)
            {
                try
                {
                    await UpdateProject(currentUser, projectForm);
                }
                catch
                {
                    //Ignored
                }
            }
        }

        /// <inheritdoc />
        public async Task<VmProject> Delete(ApplicationUser user, int projectId)
        {
            return await RemoveRestore(user, projectId, true);
        }

        /// <inheritdoc />
        public async Task DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            await RemoveRestore(currentUser, ids, true);
        }

        public async Task<VmProject> Restore(ApplicationUser currentUser, int projectId)
        {
            return await RemoveRestore(currentUser, projectId, false);
        }

        /// <inheritdoc />
        public async Task RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            await RemoveRestore(currentUser, ids, false);
        }

        private async Task<VmProject> RemoveRestore(ApplicationUser user, int projectId, 
            bool isRemoved)
        {
            var query = await GetQuery(user, true);
            var model = await query.FirstOrDefaultAsync(s => s.Id == projectId);
            if (model == null)
                throw new InvalidOperationException($"Project with id='{projectId}' not found for current user");

            model.IsRemoved = isRemoved;
            _dataContext.Entry(model).State = EntityState.Modified;

            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        private async Task RemoveRestore(ApplicationUser user, IEnumerable<int> projectIds, bool isRemoved)
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
            }

            return query;
        }

        private IQueryable<Project> SortByFields(FieldSort[] sortFields, IQueryable<Project> query)
        {
            if (sortFields == null) return query;

            IOrderedQueryable<Project> orderedQuery = null;
            foreach (var field in sortFields)
            {
                if (field == null)
                    continue;

                if (field.Is(nameof(VmProject.Name)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(s => s.Name)
                            : query.OrderByDescending(s => s.Name);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Name)
                            : orderedQuery.ThenByDescending(s => s.Name);
                    }
                }
                else if (field.Is(nameof(VmProject.GroupName)))
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
                else if (field.Is(nameof(VmProject.CreationDate)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(s => s.CreationDate)
                            : query.OrderByDescending(s => s.CreationDate);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.CreationDate)
                            : orderedQuery.ThenByDescending(s => s.CreationDate);
                    }
                }
                else if (field.Is(nameof(VmProject.IsRemoved)))
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
                else if (field.Is(nameof(VmProject.OwnerFio)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(s => s.Owner.LastName).ThenBy(s => s.Owner.FirstName).ThenBy(s => s.Owner.MiddleName)
                            : query.OrderByDescending(s => s.Owner.LastName).ThenBy(s => s.Owner.FirstName).ThenBy(s => s.Owner.MiddleName);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Owner.LastName).ThenBy(s => s.Owner.FirstName).ThenBy(s => s.Owner.MiddleName)
                            : orderedQuery.ThenByDescending(s => s.Owner.LastName).ThenBy(s => s.Owner.FirstName).ThenBy(s => s.Owner.MiddleName);
                    }
                }
            }

            return orderedQuery ?? query;
        }

        private async Task UpdateProject(ApplicationUser user, VmProjectForm projectForm)
        {
            var project = projectForm?.Project;
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (string.IsNullOrWhiteSpace(project.Name))
                throw new InvalidOperationException("Cannot create project. The name cannot be empty");

            var query = await GetQuery(user, true);
            var model = await query.FirstOrDefaultAsync(p => p.Id == project.Id);
            if (model == null)
                throw new InvalidOperationException("Cannot update project. Project not found");

            model.Name = project.Name;
            model.Description = project.Description;
            model.ProjectTeams = projectForm.TeamIds?
                .Select(tId => new ProjectTeam(model.Id, tId))
                .ToList();
            await _dataContext.SaveChangesAsync();
        }

        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly VmProjectConverter _vmConverter;
    }
}