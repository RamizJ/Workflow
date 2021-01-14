using System.Collections.Generic;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;

namespace Workflow.Services
{
    public class GoalStateNameProvider : IGoalStateNameProvider
    {
        public GoalStateNameProvider()
        {
            //TODO localization
            _stateName = new Dictionary<GoalState, string>
            {
                { GoalState.New, "Новое"},
                { GoalState.Perform, "Выполняется"},
                { GoalState.Testing, "Проверяется"},
                { GoalState.Succeed, "Выполнено"},
                { GoalState.Rejected, "Отклонено"},
                { GoalState.Delay, "Отложено"}
            };
        }

        
        public string GetName(GoalState state)
        {
            return _stateName[state];
        }


        private readonly Dictionary<GoalState, string> _stateName;
    }
}