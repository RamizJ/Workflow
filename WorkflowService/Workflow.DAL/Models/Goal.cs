using System;
using System.Collections.Generic;

namespace Workflow.DAL.Models
{
    public class Goal
    {
        public int Id { get; set; }

        public int GoalNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int ScopeId { get; set; }
        public Scope Scope { get; set; }

        public int? ParentGoalId { get; set; }
        public Goal ParentGoal { get; set; }
        public List<Goal> ChildGoals { get; set; }

        public DateTime CreationDate { get; set; }
        private DateTime ExpectedCompletedDate { get; set; }
        private TimeSpan EstimatedPerformingTime { get; set; }

        public GoalState GoalState { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public string PerformerId { get; set; }
        public ApplicationUser Performer { get; set; }

        public List<GoalObserver> Observers { get; set; }

        public int? AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public bool IsRemoved { get; set; }
    }

    public enum GoalState
    {
        New,
        Perform,
        Delay,
        Testing,
        Succeed,
        Rejected
    }
}