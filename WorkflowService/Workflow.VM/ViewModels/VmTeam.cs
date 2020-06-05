using System.Collections.Generic;
using Workflow.VM.Common;

namespace Workflow.VM.ViewModels
{
    public class VmTeam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsRemoved { get; set; }

        public List<string> TeamMembers { get; set; }
    }


    /// <inheritdoc />
    public class VmTeamResult : OperationResult<VmTeam>
    { }
}
