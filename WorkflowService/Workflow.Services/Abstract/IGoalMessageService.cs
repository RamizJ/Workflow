using System.Collections.Generic;
using System.Threading.Tasks;
using PageLoading;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGoalMessageService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VmGoalMessage> Get(ApplicationUser currentUser, int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goalId"></param>
        /// <param name="pageOptions"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoalMessage>> GetPage(ApplicationUser currentUser, 
            int? goalId, PageOptions pageOptions);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="goalId"></param>
        /// <param name="pageOptions"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoalMessage>> GetUnreadPage(ApplicationUser currentUser,
            int? goalId, PageOptions pageOptions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoalMessage>> GetRange(ApplicationUser currentUser,
            IEnumerable<int> ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<VmGoalMessage> Create(ApplicationUser currentUser, VmGoalMessage message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Update(ApplicationUser currentUser, VmGoalMessage message);

        Task MarkAsRead(ApplicationUser currentUser, IEnumerable<int> ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VmGoalMessage> Delete(ApplicationUser currentUser, int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoalMessage>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VmGoalMessage> Restore(ApplicationUser currentUser, int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<VmGoalMessage>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids);
    }
}