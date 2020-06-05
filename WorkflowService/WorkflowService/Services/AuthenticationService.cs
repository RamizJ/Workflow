using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WorkflowService.Services
{
    /// <inheritdoc />
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="configuration"></param>
        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _vmConverter = new VmUserConverter();
        }

        /// <inheritdoc />
        public async Task<VmAuthOutput> Login(VmAuthInput authInput)
        {
            if(authInput == null)
                throw new ArgumentNullException(nameof(authInput));

            if (string.IsNullOrWhiteSpace(authInput.UserName) || string.IsNullOrWhiteSpace(authInput.Password))
                throw new ArgumentException("Username and password cannot be empty", nameof(authInput));

            var user = await _userManager.FindByEmailAsync(authInput.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, authInput.Password))
                return null;

            var roleNames = await _userManager.GetRolesAsync(user);
            var signinKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName)
            };

            claims.AddRange(roleNames.Select(r => new Claim(ClaimTypes.Role, r)));
            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            int expiryInHours = Convert.ToInt32(_configuration["Jwt:ExpiryInHours"]);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claimsIdentity.Claims,
                expires: authInput.RememberMe ? DateTime.UtcNow.AddMonths(12) : DateTime.UtcNow.AddHours(expiryInHours),
                signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
            );

            var vmUser = _vmConverter.ToViewModel(user);
            vmUser.Roles = await _userManager.GetRolesAsync(user);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new VmAuthOutput(vmUser, tokenString);
        }

        /// <inheritdoc />
        public Task Logout()
        {
            return Task.CompletedTask;
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly VmUserConverter _vmConverter;
    }
}