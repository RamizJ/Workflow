using System;
using System.Collections.Generic;

namespace Workflow.DAL.Models
{
    public class Scope
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public DateTime CreationDate { get; set; }

        public int? TeamId { get; set; }
        public Team Team { get; set; }

        public List<Goal> Goals { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public bool IsRemoved { get; set; }

        private List<Metadata> Metadata { get; set; }
    }
}
