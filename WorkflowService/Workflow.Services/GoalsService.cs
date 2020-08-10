using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.Share.Extensions;
using Workflow.VM.Common;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;
using static System.Net.HttpStatusCode;

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
                throw new HttpResponseException(BadRequest,
                    $"Parameter '{nameof(pageOptions)}' cannot be null");

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
        public async Task<int> GetTotalProjectGoalsCount(ApplicationUser currentUser, int projectId)
        {
            var query = await GetQuery(currentUser, false);
            return await query.CountAsync(g => g.ProjectId == projectId);
        }

        public async Task<int> GetProjectGoalsByStateCount(ApplicationUser currentUser, int projectId, GoalState goalState)
        {
            var query = await GetQuery(currentUser, false);
            return await query.CountAsync(g => g.ProjectId == projectId 
                                               && g.State == goalState);
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
                throw new HttpResponseException(BadRequest,
                    $"Parameter '{nameof(goalForm)}' cannot be null");

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
                throw new HttpResponseException(BadRequest,
                    $"Parameter '{nameof(goalForm)}' cannot be null");

            var childGoals = await GetChildGoals(currentUser, goalForm);
            await UpdateGoals(currentUser, new[] { goalForm.Goal }, goal =>
            {
                goal.Observers = goalForm.ObserverIds?
                    .Select(observerId => new GoalObserver(goal.Id, observerId))
                    .ToList();

                goal.ChildGoals = childGoals;
            });
        }

        /// <inheritdoc />
        public async Task UpdateRange(ApplicationUser currentUser, IEnumerable<VmGoal> goals)
        {
            await UpdateGoals(currentUser, goals?.ToArray());
        }

        /// <inheritdoc />
        public async Task UpdateByFormRange(ApplicationUser currentUser, IEnumerable<VmGoalForm> goalForms)
        {
            if (goalForms == null)
                throw new HttpResponseException(BadRequest, 
                    $"Parameter '{nameof(goalForms)}' cannot be null");

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

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoal>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, true);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoal>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoal>> GetChildGoals(ApplicationUser currentUser, 
            int goalId, bool withRemoved)
        {
            var query = await GetQuery(currentUser, withRemoved, true);
            return query.Where(g => g.ParentGoalId == goalId)
                .Select(g => _vmConverter.ToViewModel(g));
        }

        /// <inheritdoc />
        public async Task<VmGoal> GetParentGoal(ApplicationUser currentUser, int goalId)
        {
            var query = await GetQuery(currentUser, true, true);
            var goal = query.FirstOrDefault(g => g.Id == goalId);
            return _vmConverter.ToViewModel(goal?.ParentGoal);
        }

        /// <inheritdoc />
        public async Task AddChildGoals(ApplicationUser currentUser, 
            int? parentGoalId, IEnumerable<int> childGoalIds)
        {
            var query = await GetQuery(currentUser, true, true);
            var childGoals = await query.Where(g => childGoalIds.Any(cId => cId == g.Id))
                .ToArrayAsync();

            foreach (var childGoal in childGoals) 
                childGoal.ParentGoalId = parentGoalId;

            await _dataContext.BulkUpdateAsync(childGoals);
        }

        /// <inheritdoc />
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
                .Include(x => x.ChildGoals)
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
                bool isIntValue = int.TryParse(word, out int intValue);
                query = query
                    .Where(goal => goal.Title.ToLower().Contains(word)
                                   || goal.Description.ToLower().Contains(word)
                                   || goal.Project.Name.ToLower().Contains(word)
                                   || isIntValue && goal.GoalNumber == intValue
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
                    var queries = strValues.Select(sv => query.Where(g =>
                        g.Title.ToLower().Contains(sv))).ToArray();

                    
                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmGoal.Description)))
                {
                    var queries = strValues.Select(sv => query.Where(g =>
                        g.Description.ToLower().Contains(sv))).ToArray();

                    if (queries.Any())
                        query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));
                }
                else if (field.SameAs(nameof(VmGoal.ProjectName)))
                {
                    var queries = strValues.Select(sv => query.Where(g =>
                        g.Project.Name.ToLower().Contains(sv))).ToArray();

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
                else if (field.SameAs(nameof(VmGoal.IsRemoved)))
                {
                    var values = field.Values.OfType<bool>().ToArray();
                    query = query.Where(g => values.Any(v => v == g.IsRemoved));
                }
            }

            return query;
        }

        private IQueryable<Goal> SortByFields(FieldSort[] sortFields, IQueryable<Goal> query)
        {
            if (sortFields == null) return query;

            foreach (var field in sortFields.Where(sf => sf != null))
            {
                var isAcending = field.SortType == SortType.Ascending;

                if (field.Is(nameof(VmGoal.Title)))
                    query = query.SortBy(g => g.Title, isAcending);

                else if (field.Is(nameof(VmGoal.Description))) 
                    query = query.SortBy(g => g.Description, isAcending);
                
                if (field.Is(nameof(VmGoal.ProjectName))) 
                    query = query.SortBy(g => g.Project.Name, isAcending);
                
                if(field.Is(nameof(VmGoal.CreationDate))) 
                    query = query.SortBy(g => g.CreationDate, isAcending);

                if (field.Is(nameof(VmGoal.ExpectedCompletedDate)))
                    query = query.SortBy(g => g.ExpectedCompletedDate, isAcending);

                else if (field.Is(nameof(VmGoal.GoalNumber)))
                    query = query.SortBy(g => g.GoalNumber, isAcending);

                else if (field.Is(nameof(VmGoal.State)))
                    query = query.SortBy(g => g.State, isAcending);

                else if (field.Is(nameof(VmGoal.Priority)))
                    query = query.SortBy(g => g.Priority, isAcending);

                else if (field.Is(nameof(VmGoal.OwnerFio)))
                    query = query
                        .SortBy(p => p.Owner.LastName, isAcending)
                        .SortBy(p => p.Owner.FirstName, isAcending)
                        .SortBy(p => p.Owner.MiddleName, isAcending);

                else if (field.Is(nameof(VmGoal.PerformerFio)))
                    query = query
                        .SortBy(p => p.Performer.LastName, isAcending)
                        .SortBy(p => p.Performer.FirstName, isAcending)
                        .SortBy(p => p.Performer.MiddleName, isAcending);

                else if (field.Is(nameof(VmGoal.IsRemoved)))
                    query = query.SortBy(g => g.IsRemoved, isAcending);
            }

            return query;
        }

        private async Task<Goal> CreateGoal(ApplicationUser currentUser, VmGoal goal,
            Action<Goal> createAction = null)
        {
            if (goal == null)
                throw new HttpResponseException(BadRequest,
                    $"Parameter '{nameof(goal)}' cannot be null");

            if (string.IsNullOrWhiteSpace(goal.Title))
                throw new HttpResponseException(BadRequest, "Goal title cannot be empty");

            var model = _vmConverter.ToModel(goal);
            model.Id = 0;
            model.CreationDate = DateTime.Now;
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
                throw new HttpResponseException(BadRequest, $"Parameter '{nameof(goals)}' cannot be null");

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
                    throw new HttpResponseException(BadRequest, 
                        "Cannot update goal. The title cannot be empty");

                var goal = goals.First(t => t.Id == model.Id);
                model.ProjectId = goal.ProjectId;
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