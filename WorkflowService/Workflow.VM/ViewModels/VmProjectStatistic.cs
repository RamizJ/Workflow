using System;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels
{
    public class VmProjectStatistic
    {
        public int[] GoalsCountForState { get; set; }

        public VmProjectStatistic()
        {
            var statesCount = Enum.GetValues(typeof(GoalState)).Length;
            GoalsCountForState = new int[statesCount];
        }
    }
}
