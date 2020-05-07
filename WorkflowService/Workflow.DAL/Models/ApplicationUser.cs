using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Workflow.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string Phone { get; set; }

        public bool IsRemoved { get; set; }

        public List<TeamUser> TeamUsers { get; set; }

        private List<Metadata> Metadata { get; set; }

        public string Fio => GetFio(FirstName, MiddleName, LastName);

        public static string GetFio(string firstName, string middleName, string lastName)
        {
            return string.Join(" ", lastName?.Trim() ?? string.Empty,
                firstName?.Trim() ?? string.Empty,
                middleName?.Trim() ?? string.Empty);
        }
    }
}