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
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class TeamUsersService : ITeamUsersService
    {
        public TeamUsersService(DataContext dataContext, 
            IViewModelConverter<ApplicationUser, VmUser> vmUserConverter)
        {
            _dataContext = dataContext;
            _vmUserConverter = vmUserConverter;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmUser>> GetPage(ApplicationUser currentUser, 
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

            var teamUsers = await query
                .Skip(pageOptions.PageNumber * pageOptions.PageSize)
                .Take(pageOptions.PageSize)
                .ToArrayAsync();

            var viewModels = teamUsers.Select(tu => _vmUserConverter.ToViewModel(tu.User));

            return viewModels;
        }

        /// <inheritdoc />
        public async Task Add(int teamId, string userId)
        {
            await _dataContext.TeamUsers.AddAsync(new TeamUser(teamId, userId));
            await _dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task AddRange(int teamId, IEnumerable<string> userIds)
        {
            var teamUsers = userIds.Select(uId => new TeamUser(teamId, uId));

            await _dataContext.TeamUsers.AddRangeAsync(teamUsers);
            await _dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task Remove(int teamId, string userId)
        {
            try
            {
                var model = new TeamUser(teamId, userId);
                _dataContext.Entry(model).State = EntityState.Deleted;
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, e.Message);
            }
        }

        public async Task RemoveRange(int teamId, IEnumerable<string> userIds)
        {
            foreach (var userId in userIds)
            {
                var model = new TeamUser(teamId, userId);
                _dataContext.Entry(model).State = EntityState.Deleted;
            }

            await _dataContext.SaveChangesAsync();
        }


        private IQueryable<TeamUser> GetQuery(int teamId, in bool withRemoved)
        {
            var query = _dataContext.TeamUsers.AsNoTracking()
                .Include(tu => tu.User)
                .Include(tu => tu.Team)
                .Where(tu => tu.TeamId == teamId);

            if (!withRemoved)
                query = query.Where(tu => tu.User.IsRemoved == false);

            return query;
        }

        private IQueryable<TeamUser> Filter(string filter, IQueryable<TeamUser> query)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            var words = filter.Split(" ");
            foreach (var word in words.Select(w => w.ToLower()))
            {
                query = query
                    .Where(tu => tu.User.Email.ToLower().Contains(word)
                                 || tu.User.PhoneNumber.ToLower().Contains(word)
                                 || tu.User.Position.Name.ToLower().Contains(word)
                                 || tu.User.FirstName.ToLower().Contains(word)
                                 || tu.User.MiddleName.ToLower().Contains(word)
                                 || tu.User.LastName.ToLower().Contains(word));
            }

            return query;
        }

        private IQueryable<TeamUser> FilterByFields(FieldFilter[] filterFields, IQueryable<TeamUser> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields.Where(ff => ff != null))
            {
                var strValues = field.Values?.Select(v => v.ToString()?.ToLower()).ToList()
                                ?? new List<string>();

                if (field.SameAs(nameof(VmUser.Email)))
                {
                    var queries = strValues.Select(sv => query.Where(tu =>
                        tu.User.Email.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.Phone)))
                {
                    var queries = strValues.Select(sv => query.Where(tu =>
                        tu.User.PhoneNumber.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.Position)))
                {
                    var queries = strValues.Select(sv => query.Where(tu =>
                            tu.User.Position.Name.ToLower().Contains(sv)
                            || tu.User.PositionCustom.ToLower().Contains(sv)))
                        .ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.LastName)))
                {
                    var queries = strValues.Select(sv => query.Where(tu =>
                            tu.User.LastName.ToLower().Contains(sv)))
                        .ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.FirstName)))
                {
                    var queries = strValues.Select(sv => query.Where(tu =>
                            tu.User.FirstName.ToLower().Contains(sv)))
                        .ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmUser.MiddleName)))
                {
                    var queries = strValues.Select(sv => query.Where(tu =>
                            tu.User.MiddleName.ToLower().Contains(sv)))
                        .ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
            }

            return query;
        }

        private IQueryable<TeamUser> SortByFields(FieldSort[] sortFields, IQueryable<TeamUser> query)
        {
            if (sortFields == null) return query;

            foreach (var field in sortFields.Where(sf => sf != null))
            {
                var isAcending = field.SortType == SortType.Ascending;

                if (field.Is(nameof(VmUser.Email)))
                    query = query.SortBy(tu => tu.User.Email, isAcending);

                else if (field.Is(nameof(VmUser.Phone)))
                    query = query.SortBy(tu => tu.User.PhoneNumber, isAcending);

                else if (field.Is(nameof(VmUser.Position)))
                    query = query.SortBy(tu => tu.User.Position.Name + tu.User.PositionCustom, isAcending);

                else if (field.Is(nameof(VmUser.LastName)))
                    query = query.SortBy(tu => tu.User.LastName, isAcending);

                else if (field.Is(nameof(VmUser.FirstName)))
                    query = query.SortBy(tu => tu.User.FirstName, isAcending);

                else if (field.Is(nameof(VmUser.MiddleName)))
                    query = query.SortBy(tu => tu.User.MiddleName, isAcending);
            }

            return query;
        }


        private readonly DataContext _dataContext;
        private readonly IViewModelConverter<ApplicationUser, VmUser> _vmUserConverter;
    }
}