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

        public List<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
        public List<GoalObserver> GoalObserver { get; set; } = new List<GoalObserver>();

        public int? PositionId { get; set; }
        public Position Position { get; set; }
        public string PositionCustom { get; set; }

        public string Fio => GetFio(FirstName, MiddleName, LastName);

        public static string GetFio(string firstName, string middleName, string lastName)
        {
            return string.Join(" ", lastName?.Trim() ?? string.Empty,
                firstName?.Trim() ?? string.Empty,
                middleName?.Trim() ?? string.Empty);
        }
    }
}