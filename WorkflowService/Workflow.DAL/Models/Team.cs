using System.Collections.Generic;

namespace Workflow.DAL.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string CreatorId { get; set; }
        public ApplicationUser Creator { get; set; }

        public List<TeamUser> TeamUsers { get; set; }
        public List<ProjectTeam> TeamProjects { get; set; } = new List<ProjectTeam>();


        public List<Project> Projects { get; set; } = new List<Project>();

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public bool IsRemoved { get; set; }
    }
}