using System.Collections.Generic;

namespace Workflow.DAL.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TeamUser> TeamUsers { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        private List<Metadata> Metadata { get; set; }
    }
}