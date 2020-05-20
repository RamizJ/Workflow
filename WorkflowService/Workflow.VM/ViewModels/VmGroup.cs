using System;
using System.Collections.Generic;

namespace Workflow.VM.ViewModels
{
    public class VmGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int? ParentGroupId { get; set; }
        public List<int> ChildGroups { get; set; }

        public DateTime CreationDate { get; set; }
        public bool IsRemoved { get; set; }
    }
}
