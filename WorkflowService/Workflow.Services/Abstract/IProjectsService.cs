using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.Services.Common;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProjectsService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VmProject> Get(ApplicationUser user, int id);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="filterFields"></param>
        /// <param name="sortFields"></param>
        /// <param name="withRemoved"></param>
        /// <returns></returns>
        Task<IEnumerable<VmProject>> GetPage(ApplicationUser user, int pageNumber, int pageSize,
            string filter, FieldFilter[] filterFields, FieldSort[] sortFields, bool withRemoved = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<VmProject>> GetRange(ApplicationUser user, int[] ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<VmProject> Create(ApplicationUser user, VmProject project);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        Task Update(ApplicationUser user, VmProject project);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<VmProject> Delete(ApplicationUser user, int projectId);


        /// <summary>
        /// Восстановление ранее удаленного проекта
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <returns></returns>
        Task<VmProject> Restore(ApplicationUser currentUser, int projectId);

    }
}