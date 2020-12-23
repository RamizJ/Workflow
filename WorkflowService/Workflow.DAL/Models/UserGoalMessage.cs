using System;

namespace Workflow.DAL.Models
{
    public class UserGoalMessage
    {
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int GoalMessageId { get; set; }

        public GoalMessage GoalMessage { get; set; }

        public DateTime? LastReadingDate { get; set; }

        public UserGoalMessage()
        {
        }

        public UserGoalMessage(int goalMessageId, string userId)
        {
            this.GoalMessageId = goalMessageId;
            this.UserId = userId;
        }
    }
}