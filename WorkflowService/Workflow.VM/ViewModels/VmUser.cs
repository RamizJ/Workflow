using System.Collections.Generic;
using Workflow.VM.Common;

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

        public bool IsRemoved { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }


    /// <inheritdoc />
    public class VmUserResult : OperationResult<VmUser>
    {
        public VmUserResult(IEnumerable<string> errorMessages, bool succeeded) 
            : base(errorMessages, succeeded)
        { }

        public VmUserResult(string errorMessage, bool succeeded = false)
            : base(new []{ errorMessage }, succeeded)
        { }

        public VmUserResult()
        { }
    }
}
