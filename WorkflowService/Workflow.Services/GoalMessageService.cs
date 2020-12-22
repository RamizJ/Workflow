using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;
using static System.Net.HttpStatusCode;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class GoalMessageService : IGoalMessageService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="pageLoadService"></param>
        /// <param name="vmConverter"></param>
        public GoalMessageService(DataContext dataContext, 
            IPageLoadService<GoalMessage> pageLoadService,
            IViewModelConverter<GoalMessage, VmGoalMessage> vmConverter)
        {
            _dataContext = dataContext;
            _pageLoadService = pageLoadService;
            _vmConverter = vmConverter;
        }


        /// <inheritdoc />
        public async Task<VmGoalMessage> Get(ApplicationUser currentUser, int id)
        {
            var msg = await GetQuery(currentUser, null)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _vmConverter.ToViewModel(msg);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoalMessage>> GetPage(ApplicationUser currentUser,
            int? goalId, PageOptions pageOptions)
        {
            var query = GetQuery(currentUser, goalId);

            //Default sorting by creation date 
            if(pageOptions.SortFields == null || pageOptions.SortFields.Length == 0)
                query = query.OrderBy(x => x.CreationDate);

            query = _pageLoadService.GetPage(query, pageOptions);

            return await query
                .Select(x => _vmConverter.ToViewModel(x))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoalMessage>> GetUnreadPage(ApplicationUser currentUser,
            int? goalId, PageOptions pageOptions)
        {
            var query = GetQuery(currentUser, goalId);

            query = query
                .Where(gm => gm.MessageSubscribers
                    .Any(um => um.UserId == currentUser.Id
                               && um.LastReadingDate == null));

            //Default sorting by creation date 
            if (pageOptions.SortFields == null || pageOptions.SortFields.Length == 0)
                query = query.OrderBy(x => x.CreationDate);

            return await query
                .Select(x => _vmConverter.ToViewModel(x))
                .ToArrayAsync();
        }

        public async Task<IEnumerable<VmGoalMessage>> GetRange(ApplicationUser currentUser, 
            IEnumerable<int> ids)
        {
            var messages = await GetQuery(currentUser, null)
                .Where(x => ids.Any(id => x.Id == id))
                .Select(x => _vmConverter.ToViewModel(x))
                .ToArrayAsync();

            return messages;
        }

        /// <inheritdoc />
        public async Task<VmGoalMessage> Create(ApplicationUser currentUser, VmGoalMessage message)
        {
            var goal = await _dataContext.Goals
                .AsNoTracking()
                .Include(x => x.Performer)
                .Include(x => x.Observers)
                .SingleOrDefaultAsync(x => x.Id == message.GoalId);

            if (goal == null)
            {
                throw new HttpResponseException(BadRequest, $"The goal with id '{message.GoalId}' not found");
            }

            var model = _vmConverter.ToModel(message);
            model.Id = 0;
            model.CreationDate = DateTime.Now;

            var observerIds = goal.Observers.Select(x => x.ObserverId);
            model.MessageSubscribers = new List<UserGoalMessage>(observerIds
                .Select(id => new UserGoalMessage { UserId = id }))
            {
                new UserGoalMessage {UserId = goal.PerformerId}
            };

            await _dataContext.AddAsync(model);
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        /// <inheritdoc />
        public async Task Update(ApplicationUser currentUser, VmGoalMessage message)
        {
            var model = await _dataContext.GoalMessages
                .Where(x => x.OwnerId == currentUser.Id)
                .SingleOrDefaultAsync(x => x.Id == message.Id);

            if (model == null)
            {
                throw new HttpResponseException(BadRequest, $"Goal message with id = '{message.Id}' not found");
            }

            model.Text = message.Text;
            model.LastEditDate = DateTime.Now;

            _dataContext.Entry(model).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
        }

        public async Task MarkAsRead(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            var unreadUserMessages = await _dataContext.UserGoalMessages
                .Where(um => um.UserId == currentUser.Id)
                .ToArrayAsync();

            foreach (var msg in unreadUserMessages)
            {
                msg.LastReadingDate = DateTime.Now;
                _dataContext.Entry(msg).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<VmGoalMessage> Delete(ApplicationUser currentUser, int id)
        {
            var messages = await RemoveRestore(currentUser, new[] { id }, true);
            return messages.SingleOrDefault();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGoalMessage>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, true);
        }

        public async Task<VmGoalMessage> Restore(ApplicationUser currentUser, int id)
        {
            var messages = await RemoveRestore(currentUser, new[] { id }, false);
            return messages.SingleOrDefault();
        }

        public async Task<IEnumerable<VmGoalMessage>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, false);
        }


        private IQueryable<GoalMessage> GetQuery(ApplicationUser currentUser, int? goalId, bool withRemoved = false)
        {
            var query = _dataContext.GoalMessages
                .Include(x => x.MessageSubscribers)
                .Where(x => x.OwnerId == currentUser.Id 
                            || x.MessageSubscribers.Any(um => um.UserId == currentUser.Id));

            if (goalId != null)
                query = query.Where(x => x.GoalId == goalId);

            if (!withRemoved)
                query = query.Where(x => !x.IsRemoved);

            return query;
        }

        private async Task<IEnumerable<VmGoalMessage>> RemoveRestore(ApplicationUser currentUser,
            IEnumerable<int> ids, bool isRemoved)
        {
            var models = await _dataContext.GoalMessages
                .Where(x => x.OwnerId == currentUser.Id &&
                            ids.Any(id => id == x.Id))
                .ToArrayAsync();

            foreach (var model in models)
            {
                model.IsRemoved = isRemoved;
                _dataContext.Entry(model).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();

            return models.Select(x => _vmConverter.ToViewModel(x));
        }


        private readonly DataContext _dataContext;
        private readonly IPageLoadService<GoalMessage> _pageLoadService;
        private readonly IViewModelConverter<GoalMessage, VmGoalMessage> _vmConverter;
    }
}