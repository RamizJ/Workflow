﻿using System;
using System.Collections.Generic;

namespace Workflow.DAL.Models
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int? ParentGroupId { get; set; }
        public Group ParentGroup { get; set; }
        public List<Group> ChildGroups { get; set; }

        public List<Project> Projects { get; set; }

        public DateTime CreationDate { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public List<Metadata> MetadataList { get; set; } = new List<Metadata>();

        public bool IsRemoved { get; set; }
    }
}