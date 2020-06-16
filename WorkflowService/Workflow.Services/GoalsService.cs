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
    public class GoalsService : IGoalsService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="userManager"></param>
        public GoalsService(DataContext dataContext, 
            UserManager<ApplicationUser> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _vmConverter = new VmGoalConverter();
            _vmAttachmentConverter = new VmAttachmentConverter();
        }


        /// <inheritdoc />
        public async Task<VmGoal> Get(ApplicationUser currentUser, int id)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = await GetQuery(currentUser, true);
            var goal = await query.FirstOrDefaultAsync(s => s.Id == id);
            return _vmConverter.ToViewModel(goal);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoal>> GetPage(ApplicationUser currentUser, 
            int projectId, int pageNumber, int pageSize, string filter, 
            FieldFilter[] filterFields, FieldSort[] sortFields, bool withRemoved = false)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = await GetQuery(currentUser, withRemoved);
            query = query.Where(x => x.ProjectId == projectId);
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
        public async Task<IEnumerable<VmGoal>> GetRange(ApplicationUser currentUser, int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return null;

            var query = await GetQuery(currentUser, true);
            return await query.Where(x => ids.Any(id => x.Id == id))
                .Select(x => _vmConverter.ToViewModel(x))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<VmGoal> Create(ApplicationUser currentUser, VmGoal goal)
        {
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));

            if(string.IsNullOrWhiteSpace(goal.Title))
                throw new InvalidOperationException("Goal title cannot be empty");

            var model = _vmConverter.ToModel(goal);
            model.Id = 0;
            model.OwnerId = currentUser.Id;

            await _dataContext.Goals.AddAsync(model);
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task Update(ApplicationUser currentUser, VmGoal goal)
        {
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));

            if (string.IsNullOrWhiteSpace(goal.Title))
                throw new InvalidOperationException("Goal title cannot be empty");

            //var model = _vmConverter.ToModel(goal);
            var query = await GetQuery(currentUser, true);
            var model = query.FirstOrDefault(x => x.Id == goal.Id);
            if(model == null)
                throw new InvalidOperationException($"Goal with id='{goal.Id}' not found");

            model.Title = goal.Title;
            model.Description = goal.Description;
            model.State = goal.State;
            model.Priority = goal.Priority;
            model.GoalNumber = goal.GoalNumber;
            model.ParentGoalId = goal.ParentGoalId;
            model.PerformerId = goal.PerformerId;

            _dataContext.Entry(model).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<VmGoal> Delete(ApplicationUser currentUser, int goalId)
        {
            return await RemoveRestore(currentUser, goalId, true);
        }

        public async Task<VmGoal> Restore(ApplicationUser currentUser, int goalId)
        {
            return await RemoveRestore(currentUser, goalId, false);
        }

        public async Task<IEnumerable<VmAttachment>> GetAttachments(ApplicationUser currentUser, int goalId)
        {
            var query = await GetQuery(currentUser, true);
            query.Include(g => g.Attachments);
            var goal = await query.FirstOrDefaultAsync(g => g.Id == goalId);

            var attachments = goal?.Attachments?.Select(a => _vmAttachmentConverter.ToViewModel(a));
            return attachments;
        }

        public async Task AddAttachments(ApplicationUser currentUser, 
            int goalId, ICollection<Attachment> attachments)
        {
            var query = await GetQuery(currentUser, true);
            query.Include(g => g.Attachments);
            var goal = await query.FirstOrDefaultAsync(g => g.Id == goalId);

            if(goal == null)
                throw new InvalidOperationException("Cannot add attachments to goal. Goal for current user not found");

            goal.Attachments.AddRange(attachments);
            await _dataContext.SaveChangesAsync();
        }

        public async Task RemoveAttachments(ApplicationUser currentUser, IEnumerable<int> attachmentIds)
        {
            var query = await GetQuery(currentUser, true);
            query.Include(g => g.Attachments);
            var goal = await query.FirstOrDefaultAsync(g => g.Id == );
        }

        private async Task<IQueryable<Goal>> GetQuery(ApplicationUser currentUser, bool withRemoved)
        {
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, RoleNames.ADMINISTRATOR_ROLE);
            var query = _dataContext.Goals.AsNoTracking()
                .Include(x => x.Owner)
                .Include(x => x.Observers)
                .Include(x => x.Performer)
                .Include(x => x.Project)
                //.ThenInclude(x => x.Team)
                //.ThenInclude(x => x.TeamUsers)
                .Where(x => isAdmin
                            || x.Project.OwnerId == currentUser.Id
                            || x.Project.ProjectTeams
                                .SelectMany(pt => pt.Team.TeamUsers)
                                .Any(tu => tu.UserId == currentUser.Id));

            if (!withRemoved)
                query = query.Where(x => x.IsRemoved == false);

            return query;
        }

        private IQueryable<Goal> Filter(string filter, IQueryable<Goal> query)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            var words = filter.Split(" ");
            foreach (var word in words.Select(w => w.ToLower()))
            {
                int.TryParse(word, out int intValue);
                query = query
                    .Where(goal => goal.Title.ToLower().Contains(word)
                                   || goal.Description.ToLower().Contains(word)
                                   || goal.Project.Name.ToLower().Contains(word)
                                   || goal.GoalNumber == intValue
                                   || goal.Owner.FirstName.ToLower().Contains(word)
                                   || goal.Owner.MiddleName.ToLower().Contains(word)
                                   || goal.Owner.LastName.ToLower().Contains(word)
                                   || goal.Performer.FirstName.ToLower().Contains(word)
                                   || goal.Performer.MiddleName.ToLower().Contains(word)
                                   || goal.Performer.LastName.ToLower().Contains(word));
            }

            return query;
        }

        private IQueryable<Goal> FilterByFields(FieldFilter[] filterFields, IQueryable<Goal> query)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields.Where(ff => ff != null))
            {
                var strValues = field.Values?
                                    .Select(v => v?.ToString()?.ToLower())
                                    .Where(v => v != null)
                                    .ToList()
                                ?? new List<string>();

                if (field.SameAs(nameof(VmGoal.Title)))
                {
                    var queries = strValues.Select(sv => query.Where(p =>
                        p.Title.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmGoal.Description)))
                {
                    var queries = strValues.Select(sv => query.Where(p =>
                        p.Description.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmGoal.ProjectName)))
                {
                    var queries = strValues.Select(sv => query.Where(p =>
                        p.Project.Name.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmGoal.GoalNumber)))
                {
                    var vals = field.Values.OfType<int>().ToArray();
                    query = query.Where(g => vals.Any(v => v == g.GoalNumber));
                }
                else if (field.SameAs(nameof(VmGoal.State)))
                {
                    var vals = field.Values.Select(v =>
                    {
                        GoalState? state = null;
                        if (Enum.TryParse<GoalState>(v.ToString(), out var s))
                            state = s;

                        return state;
                    }).Where(s => s != null).Cast<GoalState>().ToArray();

                    query = query.Where(g => vals.Any(v => v == g.State));
                }
                else if (field.SameAs(nameof(VmGoal.Priority)))
                {
                    var vals = field.Values.Select(v =>
                    {
                        GoalPriority? priority = null;
                        if (Enum.TryParse<GoalPriority>(v.ToString(), out var p))
                            priority = p;
                        return priority;
                    }).Where(s => s != null).Cast<GoalPriority>().ToArray();

                    query = query.Where(g => vals.Any(v => v == g.Priority));
                }
                else if (field.SameAs(nameof(VmGoal.OwnerFio)))
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
                else if (field.SameAs(nameof(VmGoal.PerformerFio)))
                {
                    var names = strValues.SelectMany(sv => sv.Split(" "));
                    var queries = names.Select(name => query.Where(p =>
                            p.Performer.LastName.ToLower().Contains(name)
                            || p.Performer.FirstName.ToLower().Contains(name)
                            || p.Performer.MiddleName.ToLower().Contains(name)))
                        .ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
            }

            return query;
        }

        private IQueryable<Goal> SortByFields(FieldSort[] sortFields, IQueryable<Goal> query)
        {
            if (sortFields == null) return query;

            IOrderedQueryable<Goal> orderedQuery = null;
            foreach (var field in sortFields)
            {
                if (field == null)
                    continue;

                if (field.Is(nameof(VmGoal.Title)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(goal => goal.Title)
                            : query.OrderByDescending(goal => goal.Title);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Title)
                            : orderedQuery.ThenByDescending(s => s.Title);
                    }
                }
                else if (field.Is(nameof(VmGoal.Description)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(goal => goal.Description)
                            : query.OrderByDescending(goal => goal.Description);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Description)
                            : orderedQuery.ThenByDescending(s => s.Description);
                    }
                }
                else if (field.Is(nameof(VmGoal.GoalNumber)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(goal => goal.GoalNumber)
                            : query.OrderByDescending(goal => goal.GoalNumber);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(goal => goal.GoalNumber)
                            : orderedQuery.ThenByDescending(goal => goal.GoalNumber);
                    }
                }
                else if (field.Is(nameof(VmGoal.State)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(goal => goal.State)
                            : query.OrderByDescending(goal => goal.State);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(goal => goal.State)
                            : orderedQuery.ThenByDescending(goal => goal.State);
                    }
                }
                else if (field.Is(nameof(VmGoal.OwnerFio)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(s => s.Owner.LastName)
                                .ThenBy(s => s.Owner.FirstName)
                                .ThenBy(s => s.Owner.MiddleName)
                            : query.OrderByDescending(s => s.Owner.LastName)
                                .ThenBy(s => s.Owner.FirstName)
                                .ThenBy(s => s.Owner.MiddleName);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Owner.LastName)
                                .ThenBy(s => s.Owner.FirstName)
                                .ThenBy(s => s.Owner.MiddleName)
                            : orderedQuery.ThenByDescending(s => s.Owner.LastName)
                                .ThenBy(s => s.Owner.FirstName)
                                .ThenBy(s => s.Owner.MiddleName);
                    }
                }
                else if (field.Is(nameof(VmGoal.PerformerFio)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(s => s.Performer.LastName)
                                .ThenBy(s => s.Performer.FirstName)
                                .ThenBy(s => s.Performer.MiddleName)
                            : query.OrderByDescending(s => s.Performer.LastName)
                                .ThenBy(s => s.Performer.FirstName)
                                .ThenBy(s => s.Performer.MiddleName);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(s => s.Performer.LastName)
                                .ThenBy(s => s.Performer.FirstName)
                                .ThenBy(s => s.Performer.MiddleName)
                            : orderedQuery.ThenByDescending(s => s.Performer.LastName)
                                .ThenBy(s => s.Performer.FirstName)
                                .ThenBy(s => s.Performer.MiddleName);
                    }
                }
            }

            return orderedQuery ?? query;
        }

        private async Task<VmGoal> RemoveRestore(ApplicationUser currentUser, int goalId, bool isRemoved)
        {
            var query = await GetQuery(currentUser, true);
            var goal = await query.FirstOrDefaultAsync(g => g.Id == goalId);
            if (goal == null)
                throw new InvalidOperationException($"The goal with id='{goalId}' not found");

            goal.IsRemoved = isRemoved;
            _dataContext.Entry(goal).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(goal);
        }


        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly VmGoalConverter _vmConverter;
        private readonly VmAttachmentConverter _vmAttachmentConverter;
    }
}