﻿using System;
using System.Collections.Generic;

namespace Workflow.DAL.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? ExpectedCompletedDate { get; set; }

        public List<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();
        public List<ProjectUserRole> UsersRoles { get; set; } = new List<ProjectUserRole>();

        public List<Goal> Goals { get; set; } = new List<Goal>();

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public List<Metadata> MetadataList { get; set; } = new List<Metadata>();

        public bool IsRemoved { get; set; }
    }
}
