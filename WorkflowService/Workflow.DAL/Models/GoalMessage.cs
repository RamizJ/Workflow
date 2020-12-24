using System;
using System.Collections.Generic;

namespace Workflow.DAL.Models
{
    public class GoalMessage
  {
    public int Id { get; set; }

    public DateTime CreationDate { get; set; }

    public string Text { get; set; }

    public int GoalId { get; set; }

    public Goal Goal { get; set; }

    public string OwnerId { get; set; }

    public ApplicationUser Owner { get; set; }

    public DateTime? LastEditDate { get; set; }

    public bool IsRemoved { get; set; }

    public List<UserGoalMessage> MessageSubscribers { get; set; } = new List<UserGoalMessage>();
  }
}