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
    public class ProjectTeamRolesService : IProjectTeamRolesService
    {
        public ProjectTeamRolesService(DataContext dataContext,
            IViewModelConverter<ProjectTeamRole, VmProjectTeamRole> vmConverter)
        {
            _dataContext = dataContext;
            _vmConverter = vmConverter;
        }

        public async Task<VmProjectTeamRole> Get(int projectId, int teamId)
        {
            var projectUserRole = await _dataContext.ProjectTeamRoles
                .FirstOrDefaultAsync(pur => pur.ProjectId == projectId && pur.TeamId == teamId);

            if (projectUserRole == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return _vmConverter.ToViewModel(projectUserRole);
        }

        public async Task<VmProjectTeamRole> Add(VmProjectTeamRole viewModel)
        {
            var model = _vmConverter.ToModel(viewModel);

            _dataContext.Entry(model).State = EntityState.Added;
            await _dataContext.SaveChangesAsync();

            return _vmConverter.ToViewModel(model);
        }

        public async Task Update(VmProjectTeamRole viewModel)
        {
            var model = _vmConverter.ToModel(viewModel);

            try
            {
                _dataContext.Entry(model).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!await IsExist(viewModel.ProjectId, viewModel.TeamId))
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                throw;
            }
        }

        public async Task Delete(int projectId, int teamId)
        {
            try
            {
                _dataContext.Entry(new ProjectTeamRole(projectId, teamId)).State = EntityState.Deleted;
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!await IsExist(projectId, teamId))
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                throw;
            }
        }

        public async Task<bool> IsExist(int projectId, int teamId)
        {
            return await _dataContext.ProjectTeamRoles
                .AnyAsync(ptr => ptr.ProjectId == projectId
                                 && ptr.TeamId == teamId);
        }


        private readonly DataContext _dataContext;
        private readonly IViewModelConverter<ProjectTeamRole, VmProjectTeamRole> _vmConverter;
    }
}