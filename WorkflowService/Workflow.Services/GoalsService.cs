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
    public class GoalsService : IGoalsService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataContext"></param>
        public GoalsService(DataContext dataContext)
        {
            _dataContext = dataContext;
            _vmConverter = new VmGoalConverter();
        }


        /// <inheritdoc />
        public async Task<VmGoal> Get(ApplicationUser currentUser, int id)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var scope = await GetQuery(true)
                .FirstOrDefaultAsync(s => s.Id == id);

            return _vmConverter.ToViewModel(scope);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoal>> GetPage(ApplicationUser currentUser, int scopeId,
            int pageNumber, int pageSize, string filter, FieldFilter[] filterFields,
            FieldSort[] sortFields, bool withRemoved = false)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = GetQuery(withRemoved).Where(x => x.ProjectId == scopeId);
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

            return await GetQuery(true)
                .Where(x => ids.Any(id => x.Id == id))
                .Select(x => _vmConverter.ToViewModel(x))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<VmGoal> Create(ApplicationUser currentUser, VmGoal goal)
        {
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));

            if (string.IsNullOrWhiteSpace(goal.Title))
                throw new InvalidOperationException("Goal title cannot be empty");

            var model = _vmConverter.ToModel(goal);
            model.Id = 0;
            model.OwnerId = currentUser.Id;

            await _dataContext.Goals.AddAsync(model);
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task<VmGoal> Update(ApplicationUser currentUser, VmGoal goal)
        {
            if (goal == null)
                return null;

            if (string.IsNullOrWhiteSpace(goal.Title))
                throw new InvalidOperationException("Goal title cannot be empty");

            var model = _vmConverter.ToModel(goal);

            _dataContext.Entry(model).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task<VmGoal> Delete(ApplicationUser currentUser, int goalId)
        {
            var model = await GetQuery(true)
                .FirstOrDefaultAsync(s => s.Id == goalId);

            if (model != null)
            {
                model.IsRemoved = true;
                _dataContext.Goals.Update(model);
                await _dataContext.SaveChangesAsync();

                return _vmConverter.ToViewModel(model);
            }

            return null;
        }


        private IQueryable<Goal> GetQuery(bool withRemoved)
        {
            var query = _dataContext.Goals.AsNoTracking()
                .Include(x => x.Owner)
                .Include(x => x.Observers)
                .Include(x => x.Performer)
                .AsQueryable();

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
                query = query
                    .Where(goal => goal.Title.ToLower().Contains(word)
                                   || goal.Description.ToLower().Contains(word)
                                   || goal.Project.Name.ToLower().Contains(word)
                                   || goal.GoalNumber.ToString() == word
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

            foreach (var field in filterFields)
            {
                if (field == null)
                    continue;

                var strValue = field.Value?.ToString()?.ToLower();

                if (field.Is(nameof(VmGoal.Title)))
                {
                    query = query.Where(goal => goal.Title.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmGoal.Description)))
                {
                    query = query.Where(goal => goal.Description.ToLower().Contains(strValue));
                }
                else if (field.Is(nameof(VmGoal.GoalNumber)))
                {
                    int.TryParse(field.Value?.ToString(), out var intValue);
                    query = query.Where(goal => goal.GoalNumber == intValue);
                }
                else if (field.Is(nameof(VmGoal.GoalState)))
                {
                    Enum.TryParse<GoalState>(field.Value?.ToString(), out var state);
                    query = query.Where(goal => goal.GoalState == state);
                }
                else if (field.Is(nameof(VmGoal.OwnerFio)))
                {
                    var names = strValue?.Split();
                    if (names == null || names.Length == 0)
                        continue;

                    foreach (var name in names)
                    {
                        query = query.Where(goal => goal.Owner.FirstName.ToLower().Contains(name)
                                                 || goal.Owner.MiddleName.ToLower().Contains(name)
                                                 || goal.Owner.LastName.ToLower().Contains(name));
                    }
                }
                else if (field.Is(nameof(VmGoal.PerformerFio)))
                {
                    var names = strValue?.Split();
                    if (names == null || names.Length == 0)
                        continue;

                    foreach (var name in names)
                    {
                        query = query.Where(goal => goal.Performer.FirstName.ToLower().Contains(name)
                                                    || goal.Performer.MiddleName.ToLower().Contains(name)
                                                    || goal.Performer.LastName.ToLower().Contains(name));
                    }
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
                else if (field.Is(nameof(VmGoal.GoalState)))
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? query.OrderBy(goal => goal.GoalState)
                            : query.OrderByDescending(goal => goal.GoalState);
                    }
                    else
                    {
                        orderedQuery = field.SortType == SortType.Ascending
                            ? orderedQuery.ThenBy(goal => goal.GoalState)
                            : orderedQuery.ThenByDescending(goal => goal.GoalState);
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


        private readonly DataContext _dataContext;
        private readonly VmGoalConverter _vmConverter;
    }
}