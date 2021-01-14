using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.Services.Extensions;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;
using static System.Net.HttpStatusCode;

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
        /// <param name="entityStateQueue"></param>
        public UsersService(DataContext dataContext, 
            UserManager<ApplicationUser> userManager,
            IBackgroundTaskQueue<VmEntityStateMessage> entityStateQueue)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _entityStateQueue = entityStateQueue;
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
        public async Task<IEnumerable<VmUser>> GetPage(ApplicationUser currentUser, PageOptions pageOptions)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if (pageOptions == null)
                throw new HttpResponseException(BadRequest, 
                    $"Parameter '{nameof(pageOptions)}' cannot be null");

            var query = GetQuery(pageOptions.WithRemoved);
            query = Filter(pageOptions.Filter, query);
            query = FilterByFields(pageOptions.FilterFields, query);
            query = SortByFields(pageOptions.SortFields, query);
            
            return await query
                .Skip(pageOptions.PageNumber * pageOptions.PageSize)
                .Take(pageOptions.PageSize)
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
                throw new HttpResponseException(BadRequest, result.ToString());

            _entityStateQueue.EnqueueId(user.Id, model.Id, 
                nameof(ApplicationUser), 
                EntityOperation.Create);

            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task Update(VmUser user)
        {
            if (user == null)
                throw new HttpResponseException(BadRequest, nameof(user));

            var model = await _userManager.FindByIdAsync(user.Id);
            if (model == null)
                throw new HttpResponseException(BadRequest, $"User with id='{user.Id}' not found");

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
                throw new HttpResponseException(BadRequest, result.ToString());

            _entityStateQueue.EnqueueId(user.Id, model.Id,
                nameof(ApplicationUser),
                EntityOperation.Update);
        }

        /// <inheritdoc />
        public async Task<VmUser> Delete(ApplicationUser currentUser, string userId)
        {
            var users = await RemoveRestore(currentUser, new[] { userId }, true);
            return users.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmUser>> DeleteRange(ApplicationUser currentUser, IEnumerable<string> ids)
        {
            return await RemoveRestore(currentUser, ids, true);
        }

        /// <inheritdoc />
        public async Task<VmUser> Restore(ApplicationUser currentUser, string userId)
        {
            var users = await RemoveRestore(currentUser, new[] { userId }, false);
            return users.FirstOrDefault();
        }

        public async Task<IEnumerable<VmUser>> RestoreRange(
            ApplicationUser currentUser, IEnumerable<string> ids)
        {
            return await RemoveRestore(currentUser, ids, false);
        }

        /// <inheritdoc />
        public async Task ChangePassword(ApplicationUser currentUser, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(currentUser, currentPassword, newPassword);
            if (!result.Succeeded)
                throw new HttpResponseException(BadRequest, result.ToString());
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
                var strValues = field.Values?.Select(v => v.ToString()?.ToLower()).ToList()
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
                    var values = field.Values.OfType<bool>().ToArray();
                    query = query.Where(u => values.Any(v => v == u.IsRemoved));
                }
            }

            return query;
        }

        private IQueryable<ApplicationUser> SortByFields(FieldSort[] sortFields, IQueryable<ApplicationUser> query)
        {
            if (sortFields == null) return query;

            foreach (var field in sortFields)
            {
                if (field == null) continue;

                var isAcending = field.SortType == SortType.Ascending;

                if (field.Is(nameof(VmUser.Email)))
                    query = query.SortBy(u => u.Email, isAcending);

                else if (field.Is(nameof(VmUser.UserName)))
                    query = query.SortBy(u => u.UserName, isAcending);

                else if (field.Is(nameof(VmUser.Phone)))
                    query = query.SortBy(u => u.PhoneNumber, isAcending);

                else if (field.Is(nameof(VmUser.Position)))
                {
                    query = query.SortBy(u => u.Position.Name, isAcending);
                    query = query.SortBy(u => u.PositionCustom, isAcending);
                }

                else if (field.Is(nameof(VmUser.LastName)))
                    query = query.SortBy(u => u.LastName, isAcending);

                else if (field.Is(nameof(VmUser.FirstName)))
                    query = query.SortBy(u => u.FirstName, isAcending);

                else if (field.Is(nameof(VmUser.MiddleName)))
                    query = query.SortBy(u => u.MiddleName, isAcending);

                else if (field.Is(nameof(VmUser.IsRemoved))) 
                    query = query.SortBy(u => u.IsRemoved, isAcending);
            }

            return query;
        }

        private async Task<IEnumerable<VmUser>> RemoveRestore(ApplicationUser currentUser, 
            IEnumerable<string> userIds, bool isRemoved)
        {
            var query = GetQuery(!isRemoved);
            var models = await query
                .Where(u => userIds.Any(uId => u.Id == uId))
                .ToArrayAsync();

            foreach (var model in models)
            {
                model.IsRemoved = isRemoved;
                _dataContext.Entry(model).State = EntityState.Modified;
            }
            await _dataContext.SaveChangesAsync();

            _entityStateQueue.EnqueueIds(currentUser.Id, userIds,
                nameof(ApplicationUser),
                EntityOperation.Create);
            
            return models.Select(m => _vmConverter.ToViewModel(m));
        }


        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBackgroundTaskQueue<VmEntityStateMessage> _entityStateQueue;
        private readonly VmUserConverter _vmConverter;

    }
}