using Microsoft.EntityFrameworkCore;
using PageLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.Services
{
    public class GoalMessageService : IGoalMessageService
    {
        public GoalMessageService(
            DataContext dataContext,
            IPageLoadService<GoalMessage> pageLoadService,
            IViewModelConverter<GoalMessage, VmGoalMessage> vmConverter)
        {
            _dataContext = dataContext;
            _pageLoadService = pageLoadService;
            _vmConverter = vmConverter;
        }

        public async Task<VmGoalMessage> Get(ApplicationUser currentUser, int id)
        {
            var msg = await GetQuery(currentUser, new int?())
                .FirstOrDefaultAsync(x => x.Id == id);
            
            return _vmConverter.ToViewModel(msg);
        }

        public async Task<IEnumerable<VmGoalMessage>> GetPage(
            ApplicationUser currentUser,
            int? goalId,
            PageOptions pageOptions)
        {
            var query = GetQuery(currentUser, goalId);
            
            if (pageOptions.SortFields == null || pageOptions.SortFields.Length == 0)
                query = query.OrderBy(x => x.CreationDate);
            
            var messages = await _pageLoadService
                .GetPage(query, pageOptions)
                .Select(x => _vmConverter.ToViewModel(x))
                .ToArrayAsync();
            
            return messages;
        }

        public async Task<int> GetUnreadCount(ApplicationUser currentUser, int? goalId)
        {
            var count = await GetQuery(currentUser, goalId)
                .Where(gm => gm.MessageSubscribers
                    .Any(um => um.UserId == currentUser.Id
                               && um.LastReadingDate == new DateTime?()))
                .CountAsync();

            return count;
        }
        
        public async Task<IEnumerable<VmGoalMessage>> GetUnreadPage(
            ApplicationUser currentUser,
            int? goalId,
            PageOptions pageOptions)
        {
            var query = GetQuery(currentUser, goalId)
                .Where(gm =>  gm.MessageSubscribers
                    .Any(um => um.UserId == currentUser.Id 
                               && um.LastReadingDate == new DateTime?()));
            
            if (pageOptions.SortFields == null || pageOptions.SortFields.Length == 0)
                query = query.OrderBy(x => x.CreationDate);
            
            var messages = await query
                .Select(x => _vmConverter.ToViewModel(x))
                .ToArrayAsync();
            
            return messages;
        }

        public async Task<IEnumerable<VmGoalMessage>> GetRange(
            ApplicationUser currentUser,
            IEnumerable<int> ids)
        {
            var messages = await GetQuery(currentUser, new int?())
                .Where(x => ids.Any(id => x.Id == id))
                .Select(x => _vmConverter.ToViewModel(x))
                .ToArrayAsync();
            
            return messages;
        }

        public async Task<VmGoalMessage> Create(
            ApplicationUser currentUser,
            VmGoalMessage message)
        {
            Goal goal = await _dataContext.Goals
                .Include(x => x.Performer)
                .Include(x => x.Observers)
                .SingleOrDefaultAsync(x => x.Id == message.GoalId);
            
            if (goal == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest,
                    $"The goal with id '{message.GoalId}' not found");
            }
            
            var model = _vmConverter.ToModel(message);
            model.Id = 0;
            model.CreationDate = DateTime.Now;

            var observers = goal.Observers
                .Select(x => new UserGoalMessage
                {
                    UserId = x.ObserverId,
                    User = x.Observer,
                    GoalMessage = model
                });
            
            model.MessageSubscribers = new List<UserGoalMessage>(observers)
            {
                new()
                {
                    UserId = goal.PerformerId,
                    User = goal.Performer,
                    GoalMessage = model
                }
            };
            
            await _dataContext.AddAsync(model);
            await _dataContext.SaveChangesAsync();
            
            return _vmConverter.ToViewModel(model);
        }

        public async Task Update(ApplicationUser currentUser, VmGoalMessage message)
        {
            var msg = await _dataContext.GoalMessages
                .Where(x => x.OwnerId == currentUser.Id)
                .SingleOrDefaultAsync(x => x.Id == message.Id);
            
            if (msg == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest,
                    $"Goal message with id = '{message.Id}' not found");
            }
            
            msg.Text = message.Text;
            msg.LastEditDate = DateTime.Now;
            
            _dataContext.Entry(msg).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
        }

        public async Task MarkAsRead(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            var messages = await _dataContext.UserGoalMessages
                .Where(um => um.UserId == currentUser.Id)
                .ToArrayAsync();
            
            foreach (var msg in messages)
            {
                msg.LastReadingDate = DateTime.Now;
                _dataContext.Entry(msg).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
        }

        public async Task<VmGoalMessage> Delete(ApplicationUser currentUser, int id)
        {
            var messages = await RemoveRestore(currentUser, new[] {id}, true);
            return messages.SingleOrDefault();
        }

        public async Task<IEnumerable<VmGoalMessage>> DeleteRange(
            ApplicationUser currentUser,
            IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, true);
        }

        public async Task<VmGoalMessage> Restore(ApplicationUser currentUser, int id)
        {
            return (await RemoveRestore(currentUser, new[] {id}, false)).SingleOrDefault();
        }

        public async Task<IEnumerable<VmGoalMessage>> RestoreRange(
            ApplicationUser currentUser,
            IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, false);
        }
        

        private IQueryable<GoalMessage> GetQuery(
            ApplicationUser currentUser,
            int? goalId,
            bool withRemoved = false)
        {
            var query = _dataContext.GoalMessages
                .Include(x => x.MessageSubscribers)
                .Where(x => x.OwnerId == currentUser.Id
                            || x.MessageSubscribers.Any(um => um.UserId == currentUser.Id));
            
            if (goalId.HasValue) 
                query = query.Where(x => x.GoalId == goalId);

            if (!withRemoved)
                query = query.Where(x => !x.IsRemoved);
            
            return query;
        }

        private async Task<IEnumerable<VmGoalMessage>> RemoveRestore(
            ApplicationUser currentUser, IEnumerable<int> ids, bool isRemoved)
        {
            var query = GetQuery(currentUser, null, true);
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


        private readonly DataContext _dataContext;
        private readonly IPageLoadService<GoalMessage> _pageLoadService;
        private readonly IViewModelConverter<GoalMessage, VmGoalMessage> _vmConverter;
    }
}
