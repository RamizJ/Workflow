namespace Workflow.DAL.Models
{
    public class ProjectUserRole : IUserRole
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public bool CanEditUsers { get; set; }
        public bool CanEditGoals { get; set; }
        public bool CanCloseGoals { get; set; }


        public ProjectUserRole()
        { }

        public ProjectUserRole(int projectId, string userId)
        {
            ProjectId = projectId;
            UserId = userId;
        }
    }
}