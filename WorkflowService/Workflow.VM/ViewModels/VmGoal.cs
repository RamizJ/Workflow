using System;
using System.Collections.Generic;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels
{
    public class VmGoal
    {
        public int Id { get; set; }

        public int GoalNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        public int? ParentGoalId { get; set; }
        //public List<int> ChildGoals { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? ExpectedCompletedDate { get; set; }
        public TimeSpan? EstimatedPerformingTime { get; set; }

        public GoalState State { get; set; }
        public GoalPriority Priority { get; set; }

        public string OwnerId { get; set; }
        public string OwnerFio { get; set; }
        public string PerformerId { get; set; }
        public string PerformerFio { get; set; }

        public bool IsRemoved { get; set; }
    }

    public class VmGoalForm
    {
        public VmGoal Goal { get; set; }
        public List<string> ObserverIds { get; set; }
        public List<int> ChildGoalIds { get; set; }

        public VmGoalForm()
        { }

        public VmGoalForm(VmGoal goal, List<string> observerIds, List<int> childGoalIds)
        {
            Goal = goal;
            ObserverIds = observerIds;
            ChildGoalIds = childGoalIds;
        }
    }
}
