namespace Workflow.DAL.Models
{
    public class TeamUser
    {
        public int TeamId { get; set; }
        public Team Team { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


        public bool CanEditUsers { get; set; }
        public bool CanEditGoals { get; set; }
        public bool CanCloseGoals { get; set; }


        public TeamUser()
        { }

        public TeamUser(int teamId, string userId)
        {
            TeamId = teamId;
            UserId = userId;
        }
    }
}