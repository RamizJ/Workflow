using System.Collections.Generic;

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

        public int? PositionId { get; set; }
        public string Position { get; set; }

        public bool IsRemoved { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }

    public class VmNewUser : VmUser
    {
        /// <summary>
        /// Пароль пользователя. Пароль должен содержать не менее 6 символов, включая цифры
        /// </summary>
        public string Password { get; set; }
    }

    public class VmTeamUser : VmUser
    {
        public bool CanEditUsers { get; set; }
        public bool CanEditGoals { get; set; }
        public bool CanCloseGoals { get; set; }
    }

    public class VmTeamUserBind
    {
        public int TeamId { get; set; }
        public string UserId { get; set; }

        public bool CanEditUsers { get; set; }
        public bool CanEditGoals { get; set; }
        public bool CanCloseGoals { get; set; }
    }
}
