using System;
using System.Collections.Generic;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels
{
    public class ProjectStatisticOptions
    {
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
    }

    public class ProjectStatistic
    {
        public int[] GoalsCountForState { get; set; }
        public ByDateStatistic[] ByDateStatistics { get; set; }

        public ProjectStatistic()
        {
            var statesCount = Enum.GetValues(typeof(GoalState)).Length;
            GoalsCountForState = new int[statesCount];
        }
    }

    public class ByDateStatistic
    {
        public DateTime Date { get; set; }
        public int[] GoalCountForState { get; set; }


        public ByDateStatistic(DateTime date, int[] goalCountForState)
        {
            Date = date;
            GoalCountForState = goalCountForState;
        }
    }
}
