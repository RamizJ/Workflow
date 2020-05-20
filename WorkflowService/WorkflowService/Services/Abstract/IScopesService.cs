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
        Task<VmScope> GetScope(ApplicationUser user, int id);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IEnumerable<VmScope>> GetScopes(ApplicationUser user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<VmScope> CreateScope(ApplicationUser user, VmScope scope);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task UpdateScope(ApplicationUser user, VmScope scope);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="scopeId"></param>
        /// <returns></returns>
        Task<VmScope> DeleteScope(ApplicationUser user, int scopeId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="filteredFields"></param>
        /// <param name="sort"></param>
        /// <param name="sortedFields"></param>
        /// <returns></returns>
        Task<IEnumerable<VmScope>> GetScopes(ApplicationUser user, int pageNumber, int pageSize, 
            string filter, string filteredFields, SortType sort, string sortedFields);
    }
}