using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
    public class GroupsService : IGroupsService
    {
        public GroupsService(DataContext dataContext, 
            IViewModelConverter<Group, VmGroup> vmConverter,
            UserManager<ApplicationUser> userManager,
            IPageLoadService<Group> pageLoadService)
        {
            _dataContext = dataContext;
            _vmConverter = vmConverter;
            _userManager = userManager;
            _pageLoadService = pageLoadService;
        }

        /// <inheritdoc />
        public async Task<VmGroup> Get(ApplicationUser user, int id)
        {
            if (user == null)
                throw new HttpResponseException(Unauthorized);

            var query = await GetQuery(user, true);
            query = query.Where(g => g.Id == id);
            var group = await query.FirstOrDefaultAsync();
            return _vmConverter.ToViewModel(group);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGroup>> GetPage(ApplicationUser user, 
            int? parentGroupId, PageOptions pageOptions)
        {
            if (user == null)
                throw new HttpResponseException(Unauthorized);

            if (pageOptions == null)
                throw new HttpResponseException(BadRequest,
                    $"Parameter '{nameof(pageOptions)}' cannot be null");

            var query = await GetQuery(user, pageOptions.WithRemoved);
            query = _pageLoadService.GetPage(query, pageOptions);

            return SelectViews(query);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmGroup>> GetAll(ApplicationUser user)
        {
            var query = await GetQuery(user, true, true);
            return SelectViews(query);
        }

        public async Task<VmGroup> Create(ApplicationUser currentUser, VmGroup vmGroup)
        {
            if(vmGroup == null)
                throw new HttpResponseException(BadRequest);

            var group = _vmConverter.ToModel(vmGroup);
            group.CreationDate = DateTime.Now;
            group.OwnerId = currentUser.Id;

            await _dataContext.Groups.AddAsync(group);
            await _dataContext.SaveChangesAsync();
            return _vmConverter.ToViewModel(group);
        }

        public async Task Update(ApplicationUser user, VmGroup vmGroup)
        {
            var 

            //var brigade = await GetBrigade(currentUser, id);
            //if (brigade == null)
            //    throw new InvalidOperationException("Cannot update brigade. Brigade not found for current user");

            //brigade.Name = vmBrigade.Name;

            //await _dataContext.SaveChangesAsync();
            //await _entityStateObserverService.NotifyEntityChangedAsync(brigade.Id,
            //    EntityType.Brigade, EntityOperation.Update);
        }

        public async Task<VmGroup> Restore(ApplicationUser currentUser, int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateRange(ApplicationUser currentUser, IEnumerable<VmGroup> groups)
        {
            throw new System.NotImplementedException();
        }

        public async Task<VmGroup> Delete(ApplicationUser currentUser, int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<VmGroup>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<VmGroup>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            throw new System.NotImplementedException();
        }

        private async Task<IQueryable<Group>> GetQuery(ApplicationUser user, 
            bool withRemoved, bool withChildren = false, bool withParent = false)
        {
            bool isAdmin = await _userManager.IsInRoleAsync(user, RoleNames.ADMINISTRATOR_ROLE);
            var query = _dataContext.Groups.AsNoTracking()
                .Include(x => x.Owner)
                .Include(x => x.Projects)
                .Include(x => x.MetadataList)
                .AsQueryable();

            query = query.Where(x => isAdmin
                                     || x.OwnerId == user.Id
                                     || x.Projects.Select(p => p.OwnerId)
                                         .Any(userId => userId == user.Id));

            if (withChildren)
                query = query.Include(x => x.ChildGroups);

            if (withParent)
                query = query.Include(x => x.ParentGroup);

            if (!withRemoved)
                query = query.Where(x => x.IsRemoved == false);

            return query;
        }

        private IQueryable<VmGroup> SelectViews(IQueryable<Group> query)
        {
            return query.Select(x => _vmConverter.ToViewModel(x));
        }

        private async Task UpdateTeams(ApplicationUser currentUser,
            ICollection<VmGroup> vmGroups, Action<Group> updateAction = null)
        {
            if (vmGroups == null)
                throw new HttpResponseException(BadRequest);

            var ids = vmGroups
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => x.Id);

            var query = await GetQuery(currentUser, true);
            var groups = await query
                .Where(x => ids.Any(id => x.Id == id))
                .ToArrayAsync();

            foreach (var group in groups)
            {
                if (string.IsNullOrWhiteSpace(group.Name))
                    throw new HttpResponseException(BadRequest, "Cannot update group. The name cannot be empty");

                var vmGroup = vmGroups.First(x => x.Id == group.Id);
                group.Name = vmGroup.Name;
                group.Description = vmGroup.Description;
                updateAction?.Invoke(group);
                _dataContext.Entry(group).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
        }


        private readonly DataContext _dataContext;
        private readonly IViewModelConverter<Group, VmGroup> _vmConverter;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPageLoadService<Group> _pageLoadService;
    }
}