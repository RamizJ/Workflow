using System;

namespace Workflow.VM.ViewModels
{
    public class VmUserGoalMessage
    {
        public string UserId { get; set; }
        public string UserFullName { get; set; }

        public DateTime? LastReadingDate { get; set; }
    }
}