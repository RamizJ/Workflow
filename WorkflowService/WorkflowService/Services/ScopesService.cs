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
    public class ScopesService : IScopesService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataContext">Контекст БД</param>
        public ScopesService(DataContext dataContext)
        {
            _dataContext = dataContext;
            _vmConverter = new VmScopeConverter();
        }


        /// <inheritdoc />
        public async Task<VmScope> Get(ApplicationUser user, int id)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var scope = await GetQuery(user, true)
                .FirstOrDefaultAsync(s => s.Id == id);

            return _vmConverter.ToViewModel(scope);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmScope>> GetAll(ApplicationUser user, bool withRemoved = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var query = GetQuery(user, withRemoved);
            var scopes = await query
                .Select(s => _vmConverter.ToViewModel(s))
                .ToArrayAsync();

            return scopes;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmScope>> GetPage(ApplicationUser user, 
            int pageNumber, int pageSize, 
            string filter, FieldFilter[] filterFields, FieldSort[] sortFields,
            bool withRemoved = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var query = GetQuery(user, withRemoved);
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
        public async Task<IEnumerable<VmScope>> GetRange(ApplicationUser user, int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return null;
            
            return await GetQuery(user, true)
                .Where(s => ids.Any(id => s.Id == id))
                .Select(s => _vmConverter.ToViewModel(s))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<VmScope> Create(ApplicationUser user, VmScope scope)
        {
            if(scope == null)
                throw new ArgumentNullException(nameof(scope));

            if(string.IsNullOrWhiteSpace(scope.Name))
                throw new InvalidOperationException("Scope name cannot be empty");

            var model = _vmConverter.ToModel(scope);
            model.Id = 0;
            model.OwnerId = user.Id;

            await _dataContext.Scopes.AddAsync(model);
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task<VmScope> Update(ApplicationUser user, VmScope scope)
        {
            if (scope == null)
                return null;

            if (string.IsNullOrWhiteSpace(scope.Name))
                throw new InvalidOperationException("Scope name cannot be empty");

            var model = _vmConverter.ToModel(scope);

            var isExistForUser = await GetQuery(user, true)
                .AnyAsync(s => s.Id == scope.Id);

            if (isExistForUser)
            {
                _dataContext.Entry(model).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
                return _vmConverter.ToViewModel(model);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<VmScope> Delete(ApplicationUser user, int scopeId)
        {
            var model = await GetQuery(user, true)
                .FirstOrDefaultAsync(s => s.Id == scopeId);

            if (model != null)
            {
                model.IsRemoved = true;
                _dataContext.Scopes.Update(model);
                await _dataContext.SaveChangesAsync();
                return _vmConverter.ToViewModel(model);
            }

            return null;
        }


        private IQueryable<Scope> GetQuery(ApplicationUser user, bool withRemoved)
        {
            var query = _dataContext.Scopes
                .Include(s => s.Owner)
                .Include(s => s.Team)
                .Include(s => s.Group)
                .Where(s => s.OwnerId == user.Id
                            || s.Team.TeamUsers.Any(tu => tu.UserId == user.Id));

            if (!withRemoved)
                query = query.Where(s => s.IsRemoved == false);

            return query;
        }

        private static IQueryable<Scope> Filter(string filter, IQueryable<Scope> query)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            var words = filter.Split(" ");
            foreach (var word in words)
            {
                query = query
                    .Where(s => s.Name.Contains(word)
                                || s.Group.Name.Contains(word)
                                || s.Team.Name.Contains(word)
                                || s.Owner.FirstName.Contains(word)
                                || s.Owner.MiddleName.Contains(word)
                                || s.Owner.LastName.Contains(word));
            }

            return query;
        }

        private static IQueryable<Scope> FilterByFields(FieldFilter[] filterFields, IQueryable<Scope> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields)
            {
                if(field == null)
                    continue;

                var strValue = field.Value?.ToString()?.ToLower();
                if (field.Is(nameof(VmScope.Name)))
                {
                    query = query.Where(s => s.Name.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmScope.GroupName)))
                {
                    query = query.Where(s => s.Group.Name.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmScope.TeamName)))
                {
                    query = query.Where(s => s.Team.Name.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmScope.OwnerFio)))
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

        private IQueryable<Scope> SortByFields(FieldSort[] sortFields, IQueryable<Scope> query)
        {
            if (sortFields == null) return query;

            IOrderedQueryable<Scope> orderedQuery = null;
            foreach (var field in sortFields)
            {
                if (field == null)
                    continue;

                if (field.Is(nameof(VmScope.Name)))
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
                else if (field.Is(nameof(VmScope.GroupName)))
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
                else if (field.Is(nameof(VmScope.TeamName)))
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
                else if (field.Is(nameof(VmScope.OwnerFio)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(s => s.Owner.LastName).ThenBy(s => s.Owner.FirstName).ThenBy(s => s.Owner.MiddleName)
                            : query.OrderByDescending(s => s.Team.Name).ThenBy(s => s.Owner.FirstName).ThenBy(s => s.Owner.MiddleName);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Owner.LastName).ThenBy(s => s.Owner.FirstName).ThenBy(s => s.Owner.MiddleName)
                            : orderedQuery.ThenByDescending(s => s.Team.Name).ThenBy(s => s.Owner.FirstName).ThenBy(s => s.Owner.MiddleName);
                    }
                }
            }

            return orderedQuery ?? query;
        }


        private readonly DataContext _dataContext;
        private readonly VmScopeConverter _vmConverter;
    }
}