namespace Workflow.DAL.Models
{
    public class UserGoalMessage
    {
        public int GoalId { get; set; }
        public Goal Goal { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


        public UserGoalMessage()
        { }

        public UserGoalMessage(int goalId, string userId)
        {
            GoalId = goalId;
            UserId = userId;
        }
    }
}