using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var query = GetQuery(withRemoved).Where(x => x.ScopeId == scopeId);
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
            throw new NotImplementedException();
        }

        private IQueryable<Goal> FilterByFields(FieldFilter[] filterFields, IQueryable<Goal> query)
        {
            throw new NotImplementedException();
        }

        private IQueryable<Goal> SortByFields(FieldSort[] sortFields, IQueryable<Goal> query)
        {
            throw new NotImplementedException();
        }


        private readonly DataContext _dataContext;
        private readonly VmGoalConverter _vmConverter;
    }
}