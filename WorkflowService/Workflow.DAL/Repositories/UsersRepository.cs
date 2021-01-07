using System.Collections.Generic;
using System.Linq;
using Workflow.DAL.Models;
using Workflow.DAL.Repositories.Abstract;

namespace Workflow.DAL.Repositories
{
    /// <inheritdoc />
    public class UsersRepository : IUsersRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        public UsersRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <inheritdoc />
        public IQueryable<string> GetUserIdsForGoalsProjects(
            IQueryable<Goal> goals,
            IEnumerable<int> goalIds)
        {
            var userIds = goals
                .Where(g => goalIds.Any(id => g.Id == id))
                .SelectMany(g => g.Project.ProjectTeams)
                .SelectMany(pt => pt.Team.TeamUsers)
                .Select(tu => tu.UserId)
                .Distinct()
                .Union(_dataContext.UserRoles
                    .Where(ur => ur.RoleId == _dataContext.Roles
                        .First(r => r.Name == RoleNames.ADMINISTRATOR_ROLE).Name)
                    .Select(r => r.UserId));

            return userIds;
        }

        /// <inheritdoc />
        public IQueryable<string> GetUserIdsForProjects(
            IQueryable<Project> projects, 
            IEnumerable<int> projectIds)
        {
            var userIds = projects
                .Where(p => projectIds.Any(id => p.Id == id))
                .SelectMany(p => p.ProjectTeams)
                .SelectMany(pt => pt.Team.TeamUsers)
                .Select(tu => tu.UserId)
                .Distinct()
                .Union(_dataContext.UserRoles
                    .Where(ur => ur.RoleId == _dataContext.Roles
                        .First(r => r.Name == RoleNames.ADMINISTRATOR_ROLE).Name)
                    .Select(r => r.UserId));

            return userIds;
        }

        /// <inheritdoc />
        public IQueryable<string> GetTeamMemberIdsForUsers(
            IQueryable<ApplicationUser> users, 
            IEnumerable<string> userIds)
        {
            var teamMembersIds = users
                .Where(u => userIds.Any(id => u.Id == id))
                .SelectMany(p => p.TeamUsers)
                .Select(tu => tu.UserId)
                .Distinct()
                .Union(_dataContext.UserRoles
                    .Where(ur => ur.RoleId == _dataContext.Roles
                        .First(r => r.Name == RoleNames.ADMINISTRATOR_ROLE).Name)
                    .Select(r => r.UserId));

            return teamMembersIds;
        }

        /// <inheritdoc />
        public IQueryable<string> GetProjectUserIdsForGroups(
            IQueryable<Group> groups, 
            IEnumerable<int> groupIds)
        {
            var userIds = groups
                .Where(gr => groupIds.Any(id => gr.Id == id))
                .SelectMany(g => g.Projects)
                .SelectMany(g => g.ProjectTeams)
                .SelectMany(pt => pt.Team.TeamUsers)
                .Select(tu => tu.UserId)
                .Distinct()
                .Union(_dataContext.UserRoles
                    .Where(ur => ur.RoleId == _dataContext.Roles
                        .First(r => r.Name == RoleNames.ADMINISTRATOR_ROLE).Name)
                    .Select(r => r.UserId));

            return userIds;
        }

        /// <inheritdoc />
        public IQueryable<string> GetUserIdsForTeams(IQueryable<Team> teams, IEnumerable<int> teamIds)
        {
            var teamMembersIds = teams
                .Where(t => teamIds.Any(id => t.Id == id))
                .SelectMany(t => t.TeamUsers)
                .Select(tu => tu.UserId)
                .Distinct()
                .Union(_dataContext.UserRoles
                    .Where(ur => ur.RoleId == _dataContext.Roles
                        .First(r => r.Name == RoleNames.ADMINISTRATOR_ROLE).Name)
                    .Select(r => r.UserId));

            return teamMembersIds;
        }


        private readonly DataContext _dataContext;
    }
}