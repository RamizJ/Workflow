using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Workflow.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public bool IsRemoved { get; set; }

        public List<TeamUser> TeamUsers { get; set; } = new();
        public List<GoalObserver> GoalObserver { get; set; } = new();
        public List<ProjectUserRole> ProjectsRoles { get; set; } = new();

        public int? PositionId { get; set; }
        public Position Position { get; set; }
        public string PositionCustom { get; set; }

		public List<UserGoalMessage> GoalMessages { get; set; } = new();

		public string FullName => GetFullName(FirstName, MiddleName, LastName);

        public static string GetFullName(string firstName, string middleName, string lastName)
        {
            return string.Join(" ", lastName?.Trim() ?? string.Empty,
                firstName?.Trim() ?? string.Empty,
                middleName?.Trim() ?? string.Empty);
        }
    }
}