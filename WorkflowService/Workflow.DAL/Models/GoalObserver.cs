﻿namespace Workflow.DAL.Models
{
    public class GoalObserver
    {
        public int GoalId { get; set; }
        public Goal Goal { get; set; }

        public string ObserverId { get; set; }
        public ApplicationUser Observer { get; set; }
    }
}