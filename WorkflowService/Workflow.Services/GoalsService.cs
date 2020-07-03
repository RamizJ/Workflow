using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.VM.Common;
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

        public async Task<IEnumerable<VmGoal>> GetPage(ApplicationUser currentUser, int? projectId, PageOptions pageOptions)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if (pageOptions == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = await GetQuery(currentUser, pageOptions.WithRemoved);
            query = query.Where(x => projectId == null || x.ProjectId == projectId);
            query = Filter(pageOptions.Filter, query);
            query = FilterByFields(pageOptions.FilterFields, query);
            query = SortByFields(pageOptions.SortFields, query);

            return await query
                .Skip(pageOptions.PageNumber * pageOptions.PageSize)
                .Take(pageOptions.PageSize)
                .Select(g => _vmConverter.ToViewModel(g))
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
            var model = await CreateGoal(currentUser, goal);
            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task<VmGoal> CreateByForm(ApplicationUser currentUser, VmGoalForm goalForm)
        {
            if(goalForm == null)
                throw new ArgumentNullException(nameof(goalForm));

            var childGoals = await GetChildGoals(currentUser, goalForm);
            var model = await CreateGoal(currentUser, goalForm.Goal, goal =>
            {
                goal.Observers = goalForm.ObserverIds?
                    .Select(observerId => new GoalObserver(goal.Id, observerId))
                    .ToList();

                goal.ChildGoals = childGoals;
            });
            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task Update(ApplicationUser currentUser, VmGoal goal)
        {
            await UpdateGoals(currentUser, new[] {goal});
        }

        public async Task UpdateByForm(ApplicationUser currentUser, VmGoalForm goalForm)
        {
            if(goalForm == null)
                throw new ArgumentNullException(nameof(goalForm));

            var childGoals = await GetChildGoals(currentUser, goalForm);
            await UpdateGoals(currentUser, new[] { goalForm.Goal }, goal =>
            {
                goal.Observers = goalForm.ObserverIds?
                    .Select(observerId => new GoalObserver(goal.Id, observerId))
                    .ToList();

                goal.ChildGoals = childGoals;
            });
        }

        public async Task UpdateRange(ApplicationUser currentUser, IEnumerable<VmGoal> goals)
        {
            await UpdateGoals(currentUser, goals?.ToArray());
        }

        public async Task UpdateByFormRange(ApplicationUser currentUser, IEnumerable<VmGoalForm> goalForms)
        {
            if (goalForms == null)
                throw new ArgumentNullException(nameof(goalForms));

            var forms = goalForms.ToArray();
            var goals = forms.Select(f => f.Goal).ToArray();
            await UpdateGoals(currentUser, goals, async goal =>
            {
                var form = forms.First(gf => gf.Goal.Id == goal.Id);
                goal.Observers = form.ObserverIds?
                    .Select(observerId => new GoalObserver(goal.Id, observerId))
                    .ToList();
                goal.ChildGoals = await GetChildGoals(currentUser, form);
            });
        }

        /// <inheritdoc />
        public async Task<VmGoal> Delete(ApplicationUser currentUser, int goalId)
        {
            var goals = await RemoveRestore(currentUser, new[] { goalId }, true);
            return goals.FirstOrDefault();
        }

        public async Task<IEnumerable<VmGoal>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, true);
        }

        public async Task<IEnumerable<VmGoal>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, false);
        }

        public async Task<VmGoal> Restore(ApplicationUser currentUser, int goalId)
        {
            var goals = await RemoveRestore(currentUser, new[] { goalId }, false);
            return goals.FirstOrDefault();
        }

        private async Task<IQueryable<Goal>> GetQuery(ApplicationUser currentUser, bool withRemoved, bool withChildren = false)
        {
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, RoleNames.ADMINISTRATOR_ROLE);
            var query = _dataContext.Goals.AsNoTracking()
                .Include(x => x.Owner)
                .Include(x => x.Observers)
                .Include(x => x.Performer)
                .Include(x => x.Project)
                .Where(x => isAdmin
                            || x.Project.OwnerId == currentUser.Id
                            || x.Project.ProjectTeams
                                .SelectMany(pt => pt.Team.TeamUsers)
                                .Any(tu => tu.UserId == currentUser.Id));

            if (!withRemoved)
                query = query.Where(x => x.IsRemoved == false);

            if (withChildren)
                query = query.Include(g => g.ChildGoals);

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
                    var values = field.Values.OfType<int>().ToArray();
                    query = query.Where(g => values.Any(v => v == g.GoalNumber));
                }
                else if (field.SameAs(nameof(VmGoal.State)))
                {
                    var values = field.Values.Select(v =>
                    {
                        GoalState? state = null;
                        if (Enum.TryParse<GoalState>(v.ToString(), out var s))
                            state = s;

                        return state;
                    }).Where(s => s != null).Cast<int>().ToArray();

                    query = query.Where(g => values.Any(v => v == (int) g.State));
                }
                else if (field.SameAs(nameof(VmGoal.Priority)))
                {
                    var values = field.Values.Select(v =>
                    {
                        GoalPriority? priority = null;
                        if (Enum.TryParse<GoalPriority>(v.ToString(), out var p))
                            priority = p;
                        return priority;
                    }).Where(s => s != null).Cast<int>().ToArray();

                    query = query.Where(g => values.Any(v => v == (int) g.Priority));
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

        private async Task<Goal> CreateGoal(ApplicationUser currentUser, VmGoal goal,
            Action<Goal> createAction = null)
        {
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));

            if (string.IsNullOrWhiteSpace(goal.Title))
                throw new InvalidOperationException("Goal title cannot be empty");

            var model = _vmConverter.ToModel(goal);
            model.Id = 0;
            model.OwnerId = currentUser.Id;

            createAction?.Invoke(model);

            await _dataContext.Goals.AddAsync(model);
            await _dataContext.SaveChangesAsync();
            return model;
        }

        private async Task UpdateGoals(ApplicationUser currentUser,
            ICollection<VmGoal> goals, Action<Goal> updateAction = null)
        {
            if (goals == null)
                throw new ArgumentNullException(nameof(goals));

            var goalIds = goals
                .Where(g => !string.IsNullOrWhiteSpace(g.Title))
                .Select(g => g.Id);

            var query = await GetQuery(currentUser, true);
            var models = await query
                .Where(g => goalIds.Any(gId => g.Id == gId))
                .ToArrayAsync();

            foreach (var model in models)
            {
                if (string.IsNullOrWhiteSpace(model.Title))
                    throw new InvalidOperationException("Cannot update goal. The title cannot be empty");

                var goal = goals.First(t => t.Id == model.Id);
                model.Title = goal.Title;
                model.Description = goal.Description;
                model.State = goal.State;
                model.Priority = goal.Priority;
                model.GoalNumber = goal.GoalNumber;
                model.ParentGoalId = goal.ParentGoalId;
                model.PerformerId = goal.PerformerId;

                updateAction?.Invoke(model);

                _dataContext.Entry(model).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
        }

        private async Task<IEnumerable<VmGoal>> RemoveRestore(ApplicationUser currentUser, 
            IEnumerable<int> ids, bool isRemoved)
        {
            var query = await GetQuery(currentUser, !isRemoved);
            var models = await query
                .Where(t => ids.Any(tId => t.Id == tId))
                .ToArrayAsync();

            foreach (var model in models)
            {
                model.IsRemoved = isRemoved;
                _dataContext.Entry(model).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
            return models.Select(m => _vmConverter.ToViewModel(m));
        }

        private async Task<List<Goal>> GetChildGoals(ApplicationUser currentUser, VmGoalForm goalForm)
        {
            var childIds = goalForm.ChildGoalIds ?? new List<int>();
            var query = await GetQuery(currentUser, true);
            var childGoals = await query
                .Where(g => childIds.Any(cId => g.Id == cId))
                .ToListAsync();
            return childGoals;
        }

        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly VmGoalConverter _vmConverter;
    }
}