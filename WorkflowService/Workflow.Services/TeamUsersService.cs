using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class TeamUsersService : ITeamUsersService
    {
        public TeamUsersService(DataContext dataContext)
        {
            _dataContext = dataContext;
            _vmConverter = new VmUserConverter();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmUser>> GetPage(ApplicationUser currentUser, 
            int teamId, int pageNumber, int pageSize, string filter,
            FieldFilter[] filterFields, FieldSort[] sortFields, bool withRemoved = false)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = GetQuery(teamId, withRemoved);
            query = Filter(filter, query);
            query = FilterByFields(filterFields, query);
            query = SortByFields(sortFields, query);

            return await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .Select(tu => _vmConverter.ToViewModel(tu.User))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task Add(int teamId, string userId)
        {
            await _dataContext.TeamUsers.AddAsync(new TeamUser(teamId, userId));
            await _dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task Remove(int teamId, string userId)
        {
            _dataContext.TeamUsers.Remove(new TeamUser(teamId, userId));
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

            foreach (var field in filterFields)
            {
                if (field == null)
                    continue;

                var strValue = field.Value?.ToString()?.ToLower();
                if (field.Is(nameof(VmUser.Email)))
                {
                    query = query.Where(tu => tu.User.Email.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmUser.Phone)))
                {
                    query = query.Where(tu => tu.User.PhoneNumber.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmUser.Position)))
                {
                    query = query.Where(tu => tu.User.Position.Name.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmProject.OwnerFio)))
                {
                    var names = strValue?.Split();
                    if (names == null || names.Length == 0)
                        continue;

                    foreach (var name in names)
                    {
                        query = query.Where(tu => tu.User.FirstName.ToLower().Contains(name)
                                                 || tu.User.MiddleName.ToLower().Contains(name)
                                                 || tu.User.LastName.ToLower().Contains(name));
                    }
                }
            }

            return query;
        }

        private IQueryable<TeamUser> SortByFields(FieldSort[] sortFields, IQueryable<TeamUser> query)
        {
            if (sortFields == null) return query;

            IOrderedQueryable<TeamUser> orderedQuery = null;
            foreach (var field in sortFields)
            {
                if (field == null)
                    continue;

                if (field.Is(nameof(VmUser.Email)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(tu => tu.User.Email)
                            : query.OrderByDescending(tu => tu.User.Email);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(tu => tu.User.Email)
                            : orderedQuery.ThenByDescending(tu => tu.User.Email);
                    }
                }
                else if (field.Is(nameof(VmUser.Phone)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(tu => tu.User.PhoneNumber)
                            : query.OrderByDescending(tu => tu.User.PhoneNumber);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(tu => tu.User.PhoneNumber)
                            : orderedQuery.ThenByDescending(tu => tu.User.PhoneNumber);
                    }
                }
                else if (field.Is(nameof(VmUser.Position)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(tu => tu.User.Position.Name)
                            : query.OrderByDescending(tu => tu.User.Position.Name);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(tu => tu.User.Position.Name)
                            : orderedQuery.ThenByDescending(tu => tu.User.Position.Name);
                    }
                }
                else if (field.Is(nameof(VmUser.LastName)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(tu => tu.User.LastName)
                            : query.OrderByDescending(u => u.User.LastName);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(u => u.User.LastName)
                            : orderedQuery.ThenByDescending(u => u.User.LastName);
                    }
                }
                else if (field.Is(nameof(VmUser.FirstName)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(u => u.User.FirstName)
                            : query.OrderByDescending(u => u.User.FirstName);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(u => u.User.FirstName)
                            : orderedQuery.ThenByDescending(u => u.User.FirstName);
                    }
                }
                else if (field.Is(nameof(VmUser.MiddleName)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(u => u.User.MiddleName)
                            : query.OrderByDescending(u => u.User.MiddleName);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(u => u.User.MiddleName)
                            : orderedQuery.ThenByDescending(u => u.User.MiddleName);
                    }
                }
            }

            return orderedQuery ?? query;
        }


        private readonly DataContext _dataContext;
        private readonly VmUserConverter _vmConverter;
    }
}