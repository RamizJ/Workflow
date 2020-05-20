using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Controllers.Abstract
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUsersController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IActionResult> Get(string userId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IActionResult> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber">Number of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="filter"></param>
        /// <param name="filterFields"></param>
        /// <param name="sort"></param>
        /// <param name="sortedFields"></param>
        /// <returns></returns>
        Task<IActionResult> GetPage( int pageNumber,  int pageSize,
             string filter,  string filterFields,
             SortType sort,  string sortedFields);


        /// <summary>
        /// Get users collection by ids
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        Task<IEnumerable<VmUser>> GetRange(string[] userIds);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<VmUser> Create(VmUser user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ActionResult> Update(VmUser user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ActionResult<VmScope>> Delete(string userId);
    }
}
