using System.Threading.Tasks;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{ 
    /// <summary>
    /// 
    /// </summary>
    public interface IEntityStateNotifierService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Notify(VmEntityStateMessage message);
    }
}