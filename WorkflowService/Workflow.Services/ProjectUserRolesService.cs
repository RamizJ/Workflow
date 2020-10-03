using System;
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

        public async Task<VmProjectUserRole> Get(int projectId, string userId)
        {
            var projectUserRole = await _dataContext.ProjectUserRoles
                .FirstOrDefaultAsync(pur => pur.ProjectId == projectId
                                            && pur.UserId == userId);

            if (projectUserRole != null) 
                return _vmConverter.ToViewModel(projectUserRole);

            var projectTeam = await _dataContext.ProjectTeams
                .Include(pt => pt.Project.ProjectTeams)
                .Include(pt => pt.Team)
                .ThenInclude(t => t.TeamUsers)
                .Where(x => x.ProjectId == projectId
                            && x.Project.ProjectTeams
                                .Any(pt => pt.Team.TeamUsers
                                    .Any(tu => tu.UserId == userId)))
                .SelectMany(x => x.Project.ProjectTeams)
                .FirstOrDefaultAsync();

            if(projectTeam == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return new VmProjectUserRole
            {
                ProjectId = projectId,
                UserId = userId,
                CanCloseGoals = projectTeam.CanCloseGoals,
                CanEditGoals = projectTeam.CanEditGoals,
                CanEditUsers = projectTeam.CanEditUsers
            };
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
                var isExist = await IsExist(viewModel.ProjectId, viewModel.UserId);

                _dataContext.Entry(model).State = isExist 
                    ? EntityState.Modified
                    : EntityState.Added;

                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        public async Task UpdateRange(IEnumerable<VmProjectUserRole> viewModels)
        {
            var models = viewModels.Select(_vmConverter.ToModel).ToArray();
            var modelIds = models.Select(x => new {x.ProjectId, x.UserId});

            var existedModels = _dataContext.ProjectUserRoles
                .AsNoTracking()
                .Where(x => modelIds.Any(y => x.ProjectId == y.ProjectId 
                                              && x.UserId == y.UserId));

            var updatedModels = models.Intersect(existedModels);
            var addedModels = models.Except(existedModels);

            foreach (var updatedModel in updatedModels) 
                _dataContext.Entry(updatedModel).State = EntityState.Modified;

            foreach (var addedModel in addedModels)
                _dataContext.Entry(addedModel).State = EntityState.Added;

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

        public async Task DeleteForTeam(int teamId, string userId)
        {
            var role = await _dataContext.ProjectUserRoles
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.Project.ProjectTeams)
                .Where(x => x.Project.ProjectTeams.Any(pt => pt.TeamId == teamId))
                .FirstOrDefaultAsync(x => x.UserId == userId);

            _dataContext.Entry(role).State = EntityState.Deleted;

            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteRange(int projectId, IEnumerable<string> userIds)
        {
            var models = userIds.Select(uId => new ProjectUserRole(projectId, uId));
            _dataContext.ProjectUserRoles.RemoveRange(models);
            
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteRangeForTeam(int teamId, IEnumerable<string> userIds)
        {
            var projectUserRoles = await _dataContext.ProjectUserRoles
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.Project.ProjectTeams)
                .Where(x => x.Project.ProjectTeams.Any(pt => pt.TeamId == teamId))
                .Where(x => userIds.Any(uId => uId == x.UserId))
                .ToArrayAsync();

            foreach (var role in projectUserRoles) 
                _dataContext.Entry(role).State = EntityState.Deleted;

            await _dataContext.SaveChangesAsync();
        }

        public async Task<bool> IsExist(int projectId, string userId)
        {
            return await _dataContext.ProjectUserRoles
                .AsNoTracking()
                .AnyAsync(pur => pur.ProjectId == projectId
                                 && pur.UserId == userId);
        }


        private readonly DataContext _dataContext;
        private readonly IViewModelConverter<ProjectUserRole, VmProjectUserRole> _vmConverter;
    }
}