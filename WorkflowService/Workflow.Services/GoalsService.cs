using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.DAL.Repositories.Abstract;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
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
        /// <param name="repository"></param>
        public GoalsService(DataContext dataContext,
            IGoalsRepository repository)
        {
            _dataContext = dataContext;
            _repository = repository;
            _vmConverter = new VmGoalConverter();
        }


        /// <inheritdoc />
        public async Task<VmGoal> Get(ApplicationUser currentUser, int id)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = await GetQuery(currentUser, true);
            query = query.Where(g => g.Id == id);
            var vmGoals = SelectViews(query);
            return await vmGoals.FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoal>> GetPage(ApplicationUser currentUser, 
            int? projectId, PageOptions pageOptions)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if (pageOptions == null)
                throw new HttpResponseException(BadRequest,
                    $"Parameter '{nameof(pageOptions)}' cannot be null");

            var query = await GetQuery(currentUser, pageOptions.WithRemoved);
            query = query.Where(x => (projectId == null || x.ProjectId == projectId)
                                     && x.ParentGoalId == null);
            query = Filter(pageOptions.Filter, query);
            query = FilterByFields(pageOptions.FilterFields, query);
            query = SortByFields(pageOptions.SortFields, query);
            var vmGoals = SelectViews(query);

            return await vmGoals
                .Skip(pageOptions.PageNumber * pageOptions.PageSize)
                .Take(pageOptions.PageSize)
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoal>> GetRange(ApplicationUser currentUser, int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return null;

            var query = await GetQuery(currentUser, true);
            query = query.Where(x => ids.Any(id => x.Id == id));
            var views = SelectViews(query);

            return await views.ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<int> GetTotalProjectGoalsCount(ApplicationUser currentUser, int projectId)
        {
            var query = await GetQuery(currentUser, false);
            return await query.CountAsync(g => g.ProjectId == projectId 
                                               && g.ParentGoalId == null);
        }

        public async Task<int> GetProjectGoalsByStateCount(ApplicationUser currentUser, int projectId, GoalState goalState)
        {
            var query = await GetQuery(currentUser, false);
            return await query.CountAsync(g => g.ProjectId == projectId
                                               && g.ParentGoalId == null
                                               && g.State == goalState);
        }

        /// <inheritdoc />
        public async Task<VmGoal> Create(ApplicationUser currentUser, VmGoal goal)
        {
            var model = await CreateGoal(currentUser, goal);
            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task Update(ApplicationUser currentUser, VmGoal goal)
        {
            await UpdateGoals(new[] {goal});
        }
        
        /// <inheritdoc />
        public async Task UpdateRange(ApplicationUser currentUser, IEnumerable<VmGoal> goals)
        {
            await UpdateGoals(goals?.ToArray());
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
        public async Task<VmGoal> GetParentGoal(ApplicationUser currentUser, int goalId)
        {
            var query = await GetQuery(currentUser, true, false, true);
            var goal = query.FirstOrDefault(g => g.Id == goalId);

            if(goal == null)
                throw new HttpResponseException(NotFound, "The vmGoal don't have parent");

            return _vmConverter.ToViewModel(goal.ParentGoal);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoal>> GetChildsPage(ApplicationUser currentUser, 
            int parentGoalId, PageOptions pageOptions)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            if (pageOptions == null)
                throw new HttpResponseException(BadRequest,
                    $"Parameter '{nameof(pageOptions)}' cannot be null");

            var query = await GetQuery(currentUser, pageOptions.WithRemoved);
            query = query.Where(x => x.ParentGoalId == parentGoalId);
            query = Filter(pageOptions.Filter, query);
            query = FilterByFields(pageOptions.FilterFields, query);
            query = SortByFields(pageOptions.SortFields, query);
            var vmGoals = SelectViews(query);

            return await vmGoals
                .Skip(pageOptions.PageNumber * pageOptions.PageSize)
                .Take(pageOptions.PageSize)
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task AddChildGoals(ApplicationUser currentUser, 
            int? parentGoalId, IEnumerable<int> childGoalIds)
        {
            var query = await GetQuery(currentUser, true, true);
            var childGoals = await query.Where(g => childGoalIds.Any(cId => cId == g.Id))
                .ToArrayAsync();
            var parentGoal = await query.FirstOrDefaultAsync(g => g.Id == parentGoalId);

            foreach (var childGoal in childGoals)
            {
                childGoal.ParentGoalId = parentGoalId;
                childGoal.ProjectId = parentGoal.ProjectId;
                _dataContext.Entry(childGoal).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<VmGoal> Restore(ApplicationUser currentUser, int goalId)
        {
            var goals = await RemoveRestore(currentUser, new[] { goalId }, false);
            return goals.FirstOrDefault();
        }

        private async Task<IQueryable<Goal>> GetQuery(ApplicationUser currentUser, 
            bool withRemoved, 
            bool withChildren = false,
            bool withParent = false)
        {
            var query = await _repository.GetGoalsForUser(_dataContext.Goals, currentUser);
            query = query
                .Include(x => x.Owner)
                .Include(x => x.Observers)
                .Include(x => x.Performer)
                .Include(x => x.Project)
                .Include(x => x.MetadataList);

            if (withChildren)
                query = query.Include(g => g.ChildGoals);

            if (withParent)
                query = query.Include(g => g.ParentGoal);

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
                else if (field.SameAs(nameof(VmGoal.ProjectId)))
                {
                    var values = field.Values.SelectIntValues().ToArray();
                    query = query.Where(g => values.Any(v => v == g.ProjectId));
                }
                else if (field.SameAs(nameof(VmGoal.GoalNumber)))
                {
                    var values = field.Values.SelectIntValues().ToArray();
                    query = query.Where(g => values.Any(v => v == g.GoalNumber));
                }
                else if (field.SameAs(nameof(VmGoal.State)))
                {
                    var values = field.Values.SelectEnumValues<GoalState>().Cast<int>().ToArray();
                    query = query.Where(g => values.Any(v => v == (int) g.State));
                }
                else if (field.SameAs(nameof(VmGoal.Priority)))
                {
                    var values = field.Values.SelectEnumValues<GoalPriority>().Cast<int>().ToArray();
                    query = query.Where(g => values.Any(v => v == (int) g.Priority));
                }
                else if (field.SameAs(nameof(VmGoal.OwnerId)))
                {
                    var values = field.Values.OfType<string>().ToArray();
                    query = query.Where(g => values.Any(v => v == g.OwnerId));
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
                else if (field.SameAs(nameof(VmGoal.PerformerId)))
                {
                    var values = field.Values.OfType<string>().ToArray();
                    query = query.Where(g => values.Any(v => v == g.PerformerId));
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
                    var values = field.Values.SelectBoolValues().ToArray();
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

        private IQueryable<VmGoal> SelectViews(IQueryable<Goal> query)
        {
            var vmGoals = query.Select(goal => new VmGoal
            {
                Id = goal.Id,
                ParentGoalId = goal.ParentGoalId,
                OwnerId = goal.OwnerId,
                CreationDate = goal.CreationDate,
                Title = goal.Title,
                Description = goal.Description,
                GoalNumber = goal.GoalNumber,
                PerformerId = goal.PerformerId,
                PerformerFio = goal.Performer.LastName + " " +
                               goal.Performer.FirstName + " " +
                               goal.Performer.MiddleName,
                ProjectId = goal.ProjectId,
                ProjectName = goal.Project.Name,
                State = goal.State,
                Priority = goal.Priority,
                ExpectedCompletedDate = goal.ExpectedCompletedDate,
                EstimatedPerformingTime = goal.EstimatedPerformingTime,
                HasChildren = _dataContext.Goals.Any(childGoal => childGoal.ParentGoalId == goal.Id),
                IsAttachmentsExist = _dataContext.Goals
                    .Where(g => g.Id == goal.Id)
                    .SelectMany(g => g.Attachments)
                    .Any(),
                IsRemoved = goal.IsRemoved,
                MetadataList = goal.MetadataList.Select(m => new VmMetadata
                {
                    Key = m.Key,
                    Value = m.Value
                }).ToList()
            });
            return vmGoals;
        }

        private async Task<Goal> CreateGoal(ApplicationUser currentUser, VmGoal vmGoal)
        {
            if (vmGoal == null)
                throw new HttpResponseException(BadRequest,
                    $"Parameter '{nameof(vmGoal)}' cannot be null");

            if (string.IsNullOrWhiteSpace(vmGoal.Title))
                throw new HttpResponseException(BadRequest, "Goal title cannot be empty");

            var creatingGoals = new List<Goal>();
            var goal = CreateGoalHierarchy(vmGoal, currentUser.Id, creatingGoals);

            await _dataContext.Goals.AddRangeAsync(creatingGoals);
            await _dataContext.SaveChangesAsync();

            return goal;
        }

        private Goal CreateGoalHierarchy(VmGoal vmGoal, string creatorId, List<Goal> creatingGoals)
        {
            var goal = _vmConverter.ToModel(vmGoal);
            goal.Id = 0;
            goal.CreationDate = DateTime.Now.ToUniversalTime();
            goal.OwnerId = creatorId;
            goal.MetadataList = vmGoal.MetadataList?.Select(m => new Metadata
            {
                Key = m.Key,
                Value = m.Value
            }).ToList();

            goal.Observers = vmGoal.ObserverIds?
                .Select(observerId => new GoalObserver(vmGoal.Id, observerId))
                .ToList();

            creatingGoals.Add(goal);

            if (vmGoal.Children == null) 
                return goal;

            foreach (var vmChildGoal in vmGoal.Children)
            {
                var childGoal = CreateGoalHierarchy(vmChildGoal, creatorId, creatingGoals);
                childGoal.ParentGoal = goal;
            }

            return goal;
        }

        private async Task UpdateGoals(ICollection<VmGoal> vmGoals)
        {
            if (vmGoals == null)
                throw new HttpResponseException(BadRequest, $"Parameter '{nameof(vmGoals)}' cannot be null");

            vmGoals = ChildrenHierarchyToPlainList(vmGoals);

            var goalIds = vmGoals.Select(g => g.Id).ToArray();
            var existedGoals = await _dataContext.Goals
                .Where(g => goalIds.Any(gId => gId == g.Id))
                .ToArrayAsync();

            foreach (var vmGoal in vmGoals)
            {
                if (string.IsNullOrWhiteSpace(vmGoal.Title))
                    throw new HttpResponseException(BadRequest, 
                        "Cannot update vmGoal. The title cannot be empty");

                var model = existedGoals.FirstOrDefault(eg => eg.Id == vmGoal.Id);
                if(model == null)
                    throw new HttpResponseException(NotFound, $"Goal with id='{vmGoal.Id}' not found");

                if (model.State != vmGoal.State) 
                    model.StateChangedDate = DateTime.Now.ToUniversalTime();

                model.Id = vmGoal.Id;
                model.ProjectId = vmGoal.ProjectId;
                model.Title = vmGoal.Title;
                model.Description = vmGoal.Description;
                model.State = vmGoal.State;
                model.Priority = vmGoal.Priority;
                model.GoalNumber = vmGoal.GoalNumber;
                model.PerformerId = vmGoal.PerformerId;
                model.ExpectedCompletedDate = vmGoal.ExpectedCompletedDate;
                model.EstimatedPerformingTime = vmGoal.EstimatedPerformingTime;

                model.Observers = vmGoal.ObserverIds?
                    .Select(observerId => new GoalObserver(vmGoal.Id, observerId))
                    .ToList();

                model.MetadataList = vmGoal.MetadataList?
                    .Select(x => new Metadata(x.Key, x.Value))
                    .ToList();

                _dataContext.GoalObservers.RemoveRange(_dataContext.GoalObservers.Where(x => x.GoalId == vmGoal.Id));
                _dataContext.Metadata.RemoveRange(_dataContext.Metadata.Where(m => m.GoalId == vmGoal.Id));
                _dataContext.Entry(model).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
        }

        private List<VmGoal> ChildrenHierarchyToPlainList(IEnumerable<VmGoal> vmGoals)
        {
            if (vmGoals == null)
                return new List<VmGoal>();

            var goalsArray = vmGoals.ToArray();
            var result = new List<VmGoal>(goalsArray);

            foreach (var vmGoal in goalsArray)
            {
                var childrenList = ChildrenHierarchyToPlainList(vmGoal.Children);
                result.AddRange(childrenList);
            }

            return result;
        }

        private async Task<IEnumerable<VmGoal>> RemoveRestore(ApplicationUser currentUser, 
            IEnumerable<int> ids, bool isRemoved)
        {
            var query = await GetQuery(currentUser, !isRemoved, true);
            var models = await query
                .Where(t => ids.Any(tId => t.Id == tId))
                .ToArrayAsync();

            SetIsRemoved(models, isRemoved);

            await _dataContext.SaveChangesAsync();
            var vmGoals = models.Select(m =>
            {
                var vm = _vmConverter.ToViewModel(m);
                vm.HasChildren = m.ChildGoals?.Any() ?? false;
                return vm;
            });
            return vmGoals;
        }

        private void SetIsRemoved(IEnumerable<Goal> goals, bool isRemoved)
        {
            if(goals == null)
                return;

            foreach (var goal in goals)
            {
                goal.IsRemoved = isRemoved;
                _dataContext.Entry(goal).State = EntityState.Modified;

                SetIsRemoved(goal.ChildGoals, isRemoved);
            }
        }


        private readonly DataContext _dataContext;
        private readonly IGoalsRepository _repository;
        private readonly VmGoalConverter _vmConverter;
    }
}