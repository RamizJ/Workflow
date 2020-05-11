namespace Workflow.DAL.Models
{
    public class ScopeTeam
    {
        public int ScopeId { get; set; }
        public Scope Scope { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}