using Workflow.DAL.Models;

namespace Workflow.Services.Abstract
{
    public interface IGoalStateNameProvider
    {
        string GetName(GoalState state);
    }
}