using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Workflow.VM.ViewModels
{
    public class VmUser
    {
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        public int? PositionId { get; set; }
        public string Position { get; set; }

        public IEnumerable<int> ScopeIds { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }

    public class VmUserResult
    {
        public IdentityResult Result { get; set; }
        public VmUser User { get; set; }
    }
}
