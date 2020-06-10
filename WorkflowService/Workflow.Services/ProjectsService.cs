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
        public async Task<VmProject> Create(ApplicationUser user, VmProject project)
        {
            if(project == null)
                throw new ArgumentNullException(nameof(project));

            if (string.IsNullOrWhiteSpace(project.Name))
                throw new InvalidOperationException("Cannot create project. The name cannot be empty");

            var model = _vmConverter.ToModel(project);
            model.Id = 0;
            model.OwnerId = user.Id;

            await _dataContext.Projects.AddAsync(model);
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task Update(ApplicationUser user, VmProject project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (string.IsNullOrWhiteSpace(project.Name))
                throw new InvalidOperationException("Cannot create project. The name cannot be empty");

            var model = _vmConverter.ToModel(project);
            try
            {
                _dataContext.Entry(model).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool isExist = await _dataContext.Teams.AnyAsync(t => t.Id == project.Id);
                throw new InvalidOperationException(isExist 
                    ? "Cannot update project"
                    : "Cannot update project. Project not found");
            }
        }

        /// <inheritdoc />
        public async Task<VmProject> Delete(ApplicationUser user, int projectId)
        {
            var query = await GetQuery(user, true);
            var model = await query.FirstOrDefaultAsync(s => s.Id == projectId);
            if(model == null)
                throw new InvalidOperationException("");

            model.IsRemoved = true;
            _dataContext.Projects.Update(model);
            await _dataContext.SaveChangesAsync();
            return _vmConverter.ToViewModel(model);
        }

        public async Task<VmProject> Restore(ApplicationUser currentUser, int projectId)
        {
            throw new NotImplementedException();
        }


        private async Task<IQueryable<Project>> GetQuery(ApplicationUser currentUser, bool withRemoved)
        {
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, RoleNames.ADMINISTRATOR_ROLE);
            var query = _dataContext.Projects
                .Include(s => s.Owner)
                .Include(s => s.Team)
                .Include(s => s.Group)
                .Where(s => isAdmin
                            || s.OwnerId == currentUser.Id
                            || s.Team.TeamUsers.Any(tu => tu.UserId == currentUser.Id));

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
                query = query
                    .Where(s => s.Name.ToLower().Contains(word)
                                || s.Group.Name.ToLower().Contains(word)
                                || s.Team.Name.ToLower().Contains(word)
                                || s.Owner.FirstName.ToLower().Contains(word)
                                || s.Owner.MiddleName.ToLower().Contains(word)
                                || s.Owner.LastName.ToLower().Contains(word));
            }

            return query;
        }

        private static IQueryable<Project> FilterByFields(FieldFilter[] filterFields, IQueryable<Project> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields)
            {
                if(field == null)
                    continue;

                var strValue = field.Value?.ToString()?.ToLower();
                if (field.Is(nameof(VmProject.Name)))
                {
                    query = query.Where(s => s.Name.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmProject.GroupName)))
                {
                    query = query.Where(s => s.Group.Name.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmProject.TeamName)))
                {
                    query = query.Where(s => s.Team.Name.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmProject.IsRemoved)))
                {
                    bool.TryParse(field.Value?.ToString(), out var isRemoved);
                    query = query.Where(s => s.IsRemoved == isRemoved);
                }
                else if (field.Is(nameof(VmProject.CreationDate)))
                {
                    DateTime.TryParse(field.Value?.ToString(), out var creationDate);
                    query = query.Where(s => s.CreationDate == creationDate);
                }
                else if (field.Is(nameof(VmProject.OwnerFio)))
                {
                    var names = strValue?.Split();
                    if(names == null || names.Length == 0)
                        continue;

                    foreach (var name in names)
                    {
                        query = query.Where(s => s.Owner.FirstName.ToLower().Contains(name)
                                                 || s.Owner.MiddleName.ToLower().Contains(name)
                                                 || s.Owner.LastName.ToLower().Contains(name));
                    }
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
                else if (field.Is(nameof(VmProject.TeamName)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(s => s.Team.Name)
                            : query.OrderByDescending(s => s.Team.Name);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Team.Name)
                            : orderedQuery.ThenByDescending(s => s.Team.Name);
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


        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly VmProjectConverter _vmConverter;
    }
}