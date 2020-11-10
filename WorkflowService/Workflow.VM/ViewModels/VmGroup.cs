using System;
using System.Collections.Generic;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels
{
    public class VmGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreationDate { get; set; }
        public bool IsRemoved { get; set; }

        public string OwnerId { get; set; }
        public string OwnerFio { get; set; }

        public int? ParentGroupId { get; set; }
        public List<VmGroup> ChildGroups { get; set; }

        public List<VmProject> Projects { get; set; }
        public List<VmMetadata> MetadataList { get; set; }
    }
}
