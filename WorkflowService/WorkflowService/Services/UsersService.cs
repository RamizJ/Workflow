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
    public class UsersService : IUsersService
    {
        private readonly DataContext _dataContext;
        private readonly VmUserConverter _vmConverter;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        public UsersService(DataContext dataContext)
        {
            _dataContext = dataContext;
            _vmConverter = new VmUserConverter();
        }


        /// <inheritdoc />
        public async Task<VmUser> Get(ApplicationUser currentUser, string userId)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var user = await GetQuery(true)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return _vmConverter.ToViewModel(user);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmUser>> GetAll(ApplicationUser currentUser, bool withRemoved = false)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var users = await GetQuery(withRemoved)
                .Select(u => _vmConverter.ToViewModel(u))
                .ToArrayAsync();

            return users;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmUser>> GetPage(ApplicationUser currentUser,
            int pageNumber, int pageSize, string filter, FieldFilter[] filterFields,
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
                .Select(u => _vmConverter.ToViewModel(u))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmUser>> GetRange(ApplicationUser currentUser, string[] ids)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if (ids == null || ids.Length == 0)
                return null;

            return await GetQuery(true)
                .Where(s => ids.Any(id => s.Id == id))
                .Select(s => _vmConverter.ToViewModel(s))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<VmUser> Create(ApplicationUser currentUser, VmUser user)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new InvalidOperationException("User email cannot be empty");

            var model = _vmConverter.ToModel(user);
            model.Id = null;

            await _dataContext.Users.AddAsync(model);
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task<VmUser> Update(ApplicationUser currentUser, VmUser user)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if (user == null)
                return null;

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new InvalidOperationException("User email name cannot be empty");

            var model = _vmConverter.ToModel(user);

            _dataContext.Entry(model).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task<VmUser> Delete(ApplicationUser currentUser, string userId)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var model = await GetQuery(true)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (model == null) 
                return null;

            model.IsRemoved = true;
            _dataContext.Users.Update(model);
            await _dataContext.SaveChangesAsync();
            return _vmConverter.ToViewModel(model);
        }


        private IQueryable<ApplicationUser> GetQuery(bool withRemoved)
        {
            var query = _dataContext.Users
                .Include(u => u.Position)
                .AsQueryable();

            if (!withRemoved)
                query = query.Where(u => u.IsRemoved == false);

            return query;
        }


        private static IQueryable<ApplicationUser> Filter(string filter, IQueryable<ApplicationUser> query)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            var words = filter.Split(" ");
            foreach (var word in words)
            {
                query = query
                    .Where(s => s.Email.Contains(word)
                                || s.PhoneNumber.Contains(word)
                                || s.Position.Name.Contains(word)
                                || s.LastName.Contains(word)
                                || s.FirstName.Contains(word)
                                || s.MiddleName.Contains(word)
                                || s.LastName.Contains(word));
            }

            return query;
        }

        private static IQueryable<ApplicationUser> FilterByFields(FieldFilter[] filterFields,
            IQueryable<ApplicationUser> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields)
            {
                if (field == null)
                    continue;

                var strValue = field.Value?.ToString()?.ToLower();
                if (field.Is(nameof(VmUser.Email)))
                {
                    query = query.Where(u => u.Email.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmUser.Phone)))
                {
                    query = query.Where(u => u.PhoneNumber.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmUser.Position)))
                {
                    query = query.Where(u => u.Position.Name.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmUser.LastName)))
                {
                    query = query.Where(u => u.LastName.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmUser.MiddleName)))
                {
                    query = query.Where(u => u.MiddleName.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmUser.FirstName)))
                {
                    query = query.Where(u => u.FirstName.ToLower().Contains(strValue));
                }
            }

            return query;
        }

        private IQueryable<ApplicationUser> SortByFields(FieldSort[] sortFields, IQueryable<ApplicationUser> query)
        {
            if (sortFields == null) return query;

            IOrderedQueryable<ApplicationUser> orderedQuery = null;
            foreach (var field in sortFields)
            {
                if (field == null)
                    continue;

                if (field.Is(nameof(VmUser.Email)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(u => u.Email)
                            : query.OrderByDescending(u => u.Email);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(u => u.Email)
                            : orderedQuery.ThenByDescending(u => u.Email);
                    }
                }
                else if (field.Is(nameof(VmUser.Phone)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(u => u.PhoneNumber)
                            : query.OrderByDescending(u => u.PhoneNumber);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(u => u.PhoneNumber)
                            : orderedQuery.ThenByDescending(u => u.PhoneNumber);
                    }
                }
                else if (field.Is(nameof(VmUser.Position)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(u => u.Position.Name)
                            : query.OrderByDescending(u => u.Position.Name);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(u => u.Position.Name)
                            : orderedQuery.ThenByDescending(u => u.Position.Name);
                    }
                }
                else if (field.Is(nameof(VmUser.LastName)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(u => u.LastName)
                            : query.OrderByDescending(u => u.LastName);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(u => u.LastName)
                            : orderedQuery.ThenByDescending(u => u.LastName);
                    }
                }
                else if (field.Is(nameof(VmUser.FirstName)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(u => u.FirstName)
                            : query.OrderByDescending(u => u.FirstName);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(u => u.FirstName)
                            : orderedQuery.ThenByDescending(u => u.FirstName);
                    }
                }
                else if (field.Is(nameof(VmUser.MiddleName)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(u => u.MiddleName)
                            : query.OrderByDescending(u => u.MiddleName);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(u => u.MiddleName)
                            : orderedQuery.ThenByDescending(u => u.MiddleName);
                    }
                }
            }

            return orderedQuery ?? query;
        }
    }
}