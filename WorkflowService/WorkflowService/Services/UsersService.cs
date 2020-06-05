using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
        public async Task<VmUserResult> Create(VmUser user)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var model = _vmConverter.ToModel(user);
            model.Id = null;

            var result = await _userManager.CreateAsync(model, user.Password);
            VmUser vmUser = null;
            if (result.Succeeded)
            {
                vmUser = _vmConverter.ToViewModel(model);
            }

            return new VmUserResult(result.Errors.Select(e => e.Description), result.Succeeded)
            {
                Data = vmUser
            };
        }

        /// <inheritdoc />
        public async Task<VmUserResult> Update(VmUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var model = await _userManager.FindByIdAsync(user.Id);
            if (model == null)
                return new VmUserResult($"User with id='{user.Id}' not found");

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
            VmUser vmUser = null;
            if (result.Succeeded)
            {
                vmUser = _vmConverter.ToViewModel(model);
            }

            return new VmUserResult(result.Errors.Select(e => e.Description), result.Succeeded)
            {
                Data = vmUser,
                Succeeded = result.Succeeded
            };
        }

        /// <inheritdoc />
        public async Task<VmUserResult> Delete(string userId)
        {
            var model = await _userManager.FindByIdAsync(userId);
            if (model == null)
                throw new InvalidOperationException($"User with id='{userId}' not found");

            model.IsRemoved = true;
            var result = await _userManager.UpdateAsync(model);

            VmUser vmUser = null;
            if (result.Succeeded)
            {
                vmUser = _vmConverter.ToViewModel(model);
            }

            return new VmUserResult(result.Errors.Select(e => e.Description), result.Succeeded)
            {
                Data = vmUser
            };
        }

        /// <inheritdoc />
        public async Task<VmUserResult> ChangePassword(ApplicationUser currentUser, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(currentUser, currentPassword, newPassword);
            return new VmUserResult(result.Errors.Select(e => e.Description), result.Succeeded);
        }

        /// <inheritdoc />
        public async Task<VmUserResult> ResetPassword(string id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(user, newPassword);
            }
            return new VmUserResult(result.Errors.Select(e => e.Description), result.Succeeded);
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
                else if (field.Is(nameof(VmUser.IsRemoved)))
                {
                    bool.TryParse(field.Value?.ToString(), out var isRemoved);
                    query = query.Where(u => u.IsRemoved == isRemoved);
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


        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly VmUserConverter _vmConverter;

    }
}