using System.Threading.Tasks;

namespace Workflow.Services.Abstract
{
    public interface IGoalJournalService
    {
        Task AddMessage();
    }
}