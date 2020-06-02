using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Services.Abstract
{
    /// <summary>
    /// 
    /// </summary>
    public interface IScopesService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VmScope> Get(ApplicationUser user, int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="withRemoved"></param>
        /// <returns></returns>
        Task<IEnumerable<VmScope>> GetAll(ApplicationUser user, bool withRemoved = false);

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
        Task<IEnumerable<VmScope>> GetPage(ApplicationUser user, int pageNumber, int pageSize,
            string filter, FieldFilter[] filterFields, FieldSort[] sortFields, bool withRemoved = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<VmScope>> GetRange(ApplicationUser user, int[] ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<VmScope> Create(ApplicationUser user, VmScope scope);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<VmScope> Update(ApplicationUser user, VmScope scope);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="scopeId"></param>
        /// <returns></returns>
        Task<VmScope> Delete(ApplicationUser user, int scopeId);
    }
}