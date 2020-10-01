using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.Services
{
    public class ProjectUserRolesService : IProjectUserRolesService
    {
        public ProjectUserRolesService(DataContext dataContext,
            IViewModelConverter<ProjectUserRole, VmProjectUserRole> vmConverter)
        {
            _dataContext = dataContext;
            _vmConverter = vmConverter;
        }

        public async Task<ProjectUserRole> Get(int projectId, string userId)
        {
            var projectUserRole = await _dataContext.ProjectUserRoles
                .FirstOrDefaultAsync(pur => pur.ProjectId == projectId
                                            && pur.UserId == userId);

            if (projectUserRole == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return projectUserRole;
        }

        public async Task<VmProjectUserRole> Add(VmProjectUserRole viewModel)
        {
            var model = _vmConverter.ToModel(viewModel);

            _dataContext.Entry(model).State = EntityState.Added;
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        public async Task AddRange(IEnumerable<VmProjectUserRole> viewModels)
        {
            var models = viewModels.Select(_vmConverter.ToModel);

            await _dataContext.ProjectUserRoles.AddRangeAsync(models);
            await _dataContext.SaveChangesAsync();
        }

        public async Task AddForTeam(int projectId, int teamId)
        {
            var teamUsers = await _dataContext.TeamUsers
                .Where(tu => tu.TeamId == teamId)
                .ToArrayAsync();

            var projectUserRoles = teamUsers
                .Select(tu => new ProjectUserRole(projectId, tu.UserId));

            await _dataContext.ProjectUserRoles.AddRangeAsync(projectUserRoles);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Update(VmProjectUserRole viewModel)
        {
            var model = _vmConverter.ToModel(viewModel);

            try
            {
                _dataContext.Entry(model).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!await IsExist(viewModel.ProjectId, viewModel.UserId))
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                throw;
            }
        }

        public async Task UpdateRange(IEnumerable<VmProjectUserRole> viewModels)
        {
            var models = viewModels.Select(_vmConverter.ToModel);

            _dataContext.ProjectUserRoles.UpdateRange(models);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Delete(int projectId, string userId)
        {
            try
            {
                _dataContext.Entry(new ProjectUserRole(projectId, userId)).State = EntityState.Deleted;
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!await IsExist(projectId, userId))
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                throw;
            }
        }

        public async Task DeleteRange(int projectId, IEnumerable<string> userIds)
        {
            var models = userIds.Select(uId => new ProjectUserRole(projectId, uId));
            _dataContext.ProjectUserRoles.RemoveRange(models);
            
            await _dataContext.SaveChangesAsync();
        }

        public async Task<bool> IsExist(int projectId, string userId)
        {
            return await _dataContext.ProjectUserRoles
                .AnyAsync(pur => pur.ProjectId == projectId
                                 && pur.UserId == userId);
        }


        private readonly DataContext _dataContext;
        private readonly IViewModelConverter<ProjectUserRole, VmProjectUserRole> _vmConverter;
    }
}