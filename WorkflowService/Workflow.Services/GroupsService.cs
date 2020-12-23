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
    public class GroupsService : IGroupsService
    {
        public GroupsService(DataContext dataContext, 
            IViewModelConverter<Group, VmGroup> vmConverter,
            IPageLoadService<Group> pageLoadService,
            IRolesService rolesService)
        {
            _dataContext = dataContext;
            _vmConverter = vmConverter;
            _pageLoadService = pageLoadService;
            _rolesService = rolesService;
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
            group.MetadataList = vmGroup.MetadataList?
                .Select(x => new Metadata(x.Key, x.Value))
                .ToList();

            await _dataContext.Groups.AddAsync(group);
            await _dataContext.SaveChangesAsync();
            return _vmConverter.ToViewModel(group);
        }

        public async Task Update(ApplicationUser user, VmGroup vmGroup)
        {
            await UpdateGroups(user, new[] {vmGroup});
        }

        public async Task<VmGroup> Restore(ApplicationUser currentUser, int id)
        {
            var result = await RemoveRestore(currentUser, new[] {id}, false);
            return result.FirstOrDefault();
        }

        public async Task UpdateRange(ApplicationUser currentUser, IEnumerable<VmGroup> vmGroups)
        {
            await UpdateGroups(currentUser, vmGroups);
        }

        public async Task<VmGroup> Delete(ApplicationUser currentUser, int id)
        {
            var result = await RemoveRestore(currentUser, new[] { id }, true);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<VmGroup>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, true);
        }

        public async Task<IEnumerable<VmGroup>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids)
        {
            return await RemoveRestore(currentUser, ids, false);
        }

        public async Task AddProjects(ApplicationUser currentUser, int groupId, ICollection<int> projectIds)
        {
            projectIds = projectIds.ToArray();
            var group = await _dataContext.Groups
                .Include(x => x.Projects)
                .FirstOrDefaultAsync(x => x.Id == groupId
                                          && x.OwnerId == currentUser.Id);

            if(group == null)
                throw new HttpResponseException(BadRequest, "Group for current user not found");

            if(projectIds == null || !projectIds.Any())
                throw new HttpResponseException(BadRequest, "The projects cannot be null or empty");

            var projects = _dataContext.Projects
                .Where(p => projectIds.Any(pId => pId == p.Id)
                            && p.GroupId != groupId);

            group.Projects.AddRange(projects);

            _dataContext.Entry(group).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
        }   

        public async Task RemoveProjects(ApplicationUser currentUser, int groupId, ICollection<int> projectIds)
        {
            projectIds = projectIds.ToArray();
            var group = await _dataContext.Groups
                .Include(x => x.Projects)
                .FirstOrDefaultAsync(x => x.Id == groupId
                                          && x.OwnerId == currentUser.Id);

            if (group == null)
                throw new HttpResponseException(BadRequest, "Group for current user not found");

            if (projectIds == null || !projectIds.Any())
                throw new HttpResponseException(BadRequest, "The projects cannot be null or empty");

            var remainProjects = _dataContext.Projects
                .Where(p => p.GroupId == groupId && 
                            projectIds.All(pId => pId != p.Id));

            group.Projects.Clear();
            group.Projects.AddRange(remainProjects);

            _dataContext.Entry(group).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
        }

        private async Task<IQueryable<Group>> GetQuery(ApplicationUser user, 
            bool withRemoved, bool withChildren = false, bool withParent = false)
        {
            bool isAdmin = await _rolesService.IsAdmin(user);
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

        private async Task UpdateGroups(ApplicationUser currentUser, IEnumerable<VmGroup> vmGroups)
        {
            if (vmGroups == null)
                throw new HttpResponseException(BadRequest, $"Parameter '{nameof(vmGroups)}' cannot be null");

            vmGroups = ChildrenHierarchyToPlainList(vmGroups);

            var ids = vmGroups.Select(g => g.Id).ToArray();
            var existedGroups = await _dataContext.Groups
                .Where(g => ids.Any(gId => gId == g.Id)
                            && g.OwnerId == currentUser.Id)
                .ToArrayAsync();

            foreach (var group in existedGroups)
            {
                var vmGroup = vmGroups.First(x => x.Id == group.Id);
                if (string.IsNullOrWhiteSpace(vmGroup.Name))
                    throw new HttpResponseException(BadRequest,
                        "Cannot update group. The name cannot be empty");

                _vmConverter.SetModel(vmGroup, group);

                group.MetadataList = vmGroup.MetadataList
                    .Select(x => new Metadata(x.Key, x.Value))
                    .ToList();

                _dataContext.Metadata.RemoveRange(_dataContext.Metadata.Where(m => m.GroupId == vmGroup.Id));
                _dataContext.Entry(group).State = EntityState.Modified;
            }

            await _dataContext.SaveChangesAsync();
        }

        private List<VmGroup> ChildrenHierarchyToPlainList(IEnumerable<VmGroup> vmGroups)
        {
            if (vmGroups == null)
                return new List<VmGroup>();
            
            var groupsArray = vmGroups.ToArray();
            var result = new List<VmGroup>(groupsArray);

            foreach (var vmGroup in groupsArray)
            {
                var childrenList = ChildrenHierarchyToPlainList(vmGroup.Children);
                result.AddRange(childrenList);
            }

            return result;
        }

        private async Task<IEnumerable<VmGroup>> RemoveRestore(ApplicationUser currentUser,
            IEnumerable<int> ids, bool isRemoved)
        {
            var query = await GetQuery(currentUser, !isRemoved, true);
            var models = await query
                .Where(t => ids.Any(tId => t.Id == tId))
                .ToArrayAsync();

            SetIsRemoved(models, isRemoved);

            await _dataContext.SaveChangesAsync();
            var vmGroups = models.Select(m =>
            {
                var vm = _vmConverter.ToViewModel(m);
                return vm;
            });
            return vmGroups;
        }

        private void SetIsRemoved(IEnumerable<Group> groups, bool isRemoved)
        {
            if (groups == null)
                return;

            foreach (var group in groups)
            {
                group.IsRemoved = isRemoved;
                _dataContext.Entry(group).State = EntityState.Modified;

                SetIsRemoved(group.ChildGroups, isRemoved);
            }
        }



        private readonly DataContext _dataContext;
        private readonly IViewModelConverter<Group, VmGroup> _vmConverter;
        private readonly IPageLoadService<Group> _pageLoadService;
        private readonly IRolesService _rolesService;
    }
}