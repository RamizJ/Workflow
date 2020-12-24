using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class DefaultDataInitializationService : IDefaultDataInitializationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        public DefaultDataInitializationService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <inheritdoc />
        public async Task InitializeRoles()
        {
            foreach (var roleName in RoleNames.GetAllRoleNames())
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        /// <inheritdoc />
        public async Task InitializeAdmin()
        {
            ApplicationUser admin = _userManager.FindByEmailAsync("admin@admin.ru").Result;
            if (admin != null)
                return;

            var hasher = new PasswordHasher<ApplicationUser>();
            admin = new ApplicationUser
            {
                FirstName = "Administrator",
                MiddleName = string.Empty,
                LastName = string.Empty,
                Email = "admin@admin.ru",
                NormalizedEmail = "ADMIN@ADMIN.RU",
                UserName = "admin@admin.ru",
                NormalizedUserName = "ADMIN@ADMIN.RU",
                PasswordHash = hasher.HashPassword(null, "Aa010110!"),
            };
            var result = await _userManager.CreateAsync(admin);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, RoleNames.ADMINISTRATOR_ROLE);
            }
            else
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, result.Errors));
            }
        }
    }
}