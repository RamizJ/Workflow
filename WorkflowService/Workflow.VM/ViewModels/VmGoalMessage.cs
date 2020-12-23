using System;
using System.Collections.Generic;

namespace Workflow.VM.ViewModels
{
    public class VmGoalMessage
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }

        public int GoalId { get; set; }

        public string GoalTitle { get; set; }

        public string OwnerId { get; set; }

        public string OwnerFullName { get; set; }

        public DateTime? LastEditDate { get; set; }

        public bool IsRemoved { get; set; }

        public List<VmUserGoalMessage> MessageSubscribers { get; set; }
    }
}