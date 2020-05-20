using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;

namespace WorkflowService.Controllers.Abstract
{
    /// <summary>
    /// API for working with groups
    /// </summary>
    public interface IGroupsController
    {
        /// <summary>
        /// Get group
        /// </summary>
        /// <param name="id">group id</param>
        /// <returns>group data. Returned only if available for authenticated user</returns>
        Task<ActionResult<VmGroup>> Get(int id);


        /// <summary>
        /// Get all groups
        /// </summary>
        /// <returns>Collection of groups</returns>
        Task<IEnumerable<VmGroup>> GetAll();


        /// <summary>
        /// Get groups collection by ids
        /// </summary>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGroup>> GetRange(int[] groupIds);

        /// <summary>
        /// Create group
        /// </summary>
        /// <param name="group">New group</param>
        /// <returns>group data with updated group id</returns>
        Task<ActionResult<VmGroup>> Create(VmGroup group);

        /// <summary>
        /// Update group
        /// </summary>
        /// <param name="group">Updated group</param>
        /// <returns>Nothin</returns>
        Task<IActionResult> Update(VmGroup group);

        /// <summary>
        /// Delete group
        /// </summary>
        /// <param name="id">group id</param>
        /// <returns></returns>
        Task<ActionResult<VmGroup>> Delete(int id);
    }
}