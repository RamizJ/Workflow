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
    public class UsersService : IUsersService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="userManager"></param>
        public UsersService(DataContext dataContext, UserManager<ApplicationUser> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
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
        public Task<VmUser> GetCurrent(ApplicationUser currentUser)
        {
            if(currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var user = _vmConverter.ToViewModel(currentUser);
            return Task.FromResult(user);
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
        public async Task<VmUser> Create(VmUser user, string password)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var model = _vmConverter.ToModel(user);
            model.Id = null;

            var result = await _userManager.CreateAsync(model, password);
            if (!result.Succeeded) 
                throw new InvalidOperationException(result.ToString());

            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task Update(VmUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var model = await _userManager.FindByIdAsync(user.Id);
            if (model == null)
                throw new InvalidOperationException($"User with id='{user.Id}' not found");

            model.UserName = user.UserName;
            model.NormalizedUserName = user.UserName.ToUpper();
            model.Email = user.Email;
            model.NormalizedEmail = user.Email.ToUpper();
            model.PhoneNumber = user.Phone;
            model.LastName = user.LastName;
            model.FirstName = user.FirstName;
            model.MiddleName = user.MiddleName;
            model.PositionId = user.PositionId;
            model.PositionCustom = user.Position;

            var result = await _userManager.UpdateAsync(model);
            if (!result.Succeeded) 
                throw new InvalidOperationException(result.ToString());
        }

        /// <inheritdoc />
        public async Task<VmUser> Delete(string userId)
        {
            return await RemoveRestore(userId, true);
        }

        public async Task<VmUser> Restore(ApplicationUser currentUser, string userId)
        {
            return await RemoveRestore(userId, false);
        }

        /// <inheritdoc />
        public async Task ChangePassword(ApplicationUser currentUser, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(currentUser, currentPassword, newPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException(result.ToString());
        }

        /// <inheritdoc />
        public async Task ResetPassword(string id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.RemovePasswordAsync(user);
            if (result.Succeeded) 
                result = await _userManager.AddPasswordAsync(user, newPassword);

            if(!result.Succeeded)
                throw new InvalidOperationException(result.ToString());
        }

        /// <inheritdoc />
        public async Task<bool> IsEmailExist(string email)
        {
            return await _dataContext.Users
                .AnyAsync(u => u.NormalizedEmail == email.ToUpper());
        }

        /// <inheritdoc />
        public async Task<bool> IsUserNameExist(string userName)
        {
            return await _dataContext.Users
                .AnyAsync(u => u.NormalizedUserName == userName.ToUpper());
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
            foreach (var word in words.Select(w => w.ToLower()))
            {
                query = query
                    .Where(s => s.Email.ToLower().Contains(word)
                                || s.PhoneNumber.ToLower().Contains(word)
                                || s.Position.Name.ToLower().Contains(word)
                                || s.LastName.ToLower().Contains(word)
                                || s.FirstName.ToLower().Contains(word)
                                || s.MiddleName.ToLower().Contains(word)
                                || s.LastName.ToLower().Contains(word));
            }

            return query;
        }

        private static IQueryable<ApplicationUser> FilterByFields(FieldFilter[] filterFields,
            IQueryable<ApplicationUser> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields.Where(ff => ff != null))
            {
                var strValues = field.Values?.Select(v => v.ToString().ToLower()).ToList()
                                ?? new List<string>();

                if (field.SameAs(nameof(VmUser.Email)))
                {
                    var queries = strValues.Select(sv => query.Where(u => 
                        u.Email.ToLower().Contains(sv))).ToArray();

                    if (queries.Any()) 
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.Phone)))
                {
                    var queries = strValues.Select(sv => query.Where(u => 
                        u.PhoneNumber.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.Position)))
                {
                    var queries = strValues.Select(sv => query.Where(u =>
                            u.Position.Name.ToLower().Contains(sv)
                            || u.PositionCustom.ToLower().Contains(sv)))
                        .ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.LastName)))
                {
                    var queries = strValues.Select(sv => query.Where(u =>
                        u.LastName.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.MiddleName)))
                {
                    var queries = strValues.Select(sv => query.Where(u =>
                        u.MiddleName.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.FirstName)))
                {
                    var queries = strValues.Select(sv => query.Where(u =>
                        u.FirstName.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.IsRemoved)))
                {
                    var vals = field.Values.OfType<bool>().ToArray();
                    query = query.Where(u => vals.Any(v => v == u.IsRemoved));
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

        private async Task<VmUser> RemoveRestore(string userId, bool isRemoved)
        {
            var model = await _userManager.FindByIdAsync(userId);
            if (model == null)
                throw new InvalidOperationException($"User with id='{userId}' not found");

            model.IsRemoved = isRemoved;
            var result = await _userManager.UpdateAsync(model);

            if (!result.Succeeded)
                throw new InvalidOperationException(result.ToString());

            return _vmConverter.ToViewModel(model);
        }


        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly VmUserConverter _vmConverter;

    }
}