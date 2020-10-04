namespace Workflow.DAL.Models
{
    public interface IUserRole
    {
        bool CanEditUsers { get; set; }
        bool CanEditGoals { get; set; }
        bool CanCloseGoals { get; set; }
    }
}