using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Common;
using Workflow.VM.ViewModels;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class ProjectTeamsService : IProjectTeamsService
    {
        /// <inheritdoc />
        public async Task<IEnumerable<VmProject>> GetTeamProjectsPage(ApplicationUser currentUser, int teamId, int pageNumber, int pageSize, string filter,
            FieldFilter[] filterFields, FieldSort[] sortFields, bool withRemoved = false)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmTeam>> GetProjectTeamsPage(ApplicationUser currentUser, int projectId, int pageNumber, int pageSize, string filter,
            FieldFilter[] filterFields, FieldSort[] sortFields, bool withRemoved = false)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task AddProject(int teamId, int projectId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task RemoveProject(int teamId, int projectId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task AddTeam(int projectId, int teamId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task RemoveTeam(int projectId, int teamId)
        {
            throw new System.NotImplementedException();
        }
    }
}