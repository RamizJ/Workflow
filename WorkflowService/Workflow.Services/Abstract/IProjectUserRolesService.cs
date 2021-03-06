﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    public interface IProjectUserRolesService
    {
        Task<VmProjectUserRole> Get(int projectId, string userId);
        Task<IEnumerable<VmProjectUserRole>> GetForTeam(int projectId, int teamId);

        Task<VmProjectUserRole> Add(VmProjectUserRole viewModel);
        Task AddRange(IEnumerable<VmProjectUserRole> viewModels);
        Task AddForTeam(int projectId, int teamId);

        Task Update(VmProjectUserRole viewModel);
        Task UpdateRange(IEnumerable<VmProjectUserRole> viewModels);

        Task Delete(int projectId, string userId);
        Task DeleteForTeam(int teamId, string userId);
        Task DeleteRange(int projectId, IEnumerable<string> userIds);
        Task DeleteRangeForTeam(int teamId, IEnumerable<string> userIds);

        Task<bool> IsExist(int projectId, string userId);
    }
}