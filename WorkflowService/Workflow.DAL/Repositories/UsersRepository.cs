using System;
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
            if (goals == null)
                throw new ArgumentNullException(nameof(goals));

            if (goalIds == null)
                throw new ArgumentNullException(nameof(goalIds));

            var userIds = goals
                .Where(g => goalIds.Any(id => g.Id == id))
                .SelectMany(g => g.Project.ProjectTeams)
                .SelectMany(pt => pt.Team.TeamUsers)
                .Select(tu => tu.UserId);

            return userIds;
        }

        /// <inheritdoc />
        public IQueryable<string> GetUserIdsForProjects(
            IQueryable<Project> projects, 
            IEnumerable<int> projectIds)
        {
            if (projects == null)
                throw new ArgumentNullException(nameof(projects));

            if (projectIds == null)
                throw new ArgumentNullException(nameof(projectIds));

            var userIds = projects
                .Where(p => projectIds.Any(id => p.Id == id))
                .SelectMany(p => p.ProjectTeams)
                .SelectMany(pt => pt.Team.TeamUsers)
                .Select(tu => tu.UserId);

            return userIds.Distinct();
        }

        public IQueryable<ApplicationUser> GetUsersForProjects(
            IQueryable<Project> projects, 
            IEnumerable<int> projectIds)
        {
            if (projects == null)
                throw new ArgumentNullException(nameof(projects));

            if (projectIds == null)
                throw new ArgumentNullException(nameof(projectIds));

            var users = projects
                .Where(p => projectIds.Any(id => p.Id == id))
                .SelectMany(p => p.ProjectTeams)
                .SelectMany(pt => pt.Team.TeamUsers)
                .Select(tu => tu.User);
            
            return users.Distinct();
        }

        /// <inheritdoc />
        public IQueryable<string> GetTeamMemberIdsForUsers(
            IQueryable<ApplicationUser> users, 
            IEnumerable<string> userIds)
        {
            if (users == null)
                throw new ArgumentNullException(nameof(users));

            if (userIds == null)
                throw new ArgumentNullException(nameof(userIds));

            var teamMembersIds = users
                .Where(u => userIds.Any(id => u.Id == id))
                .SelectMany(p => p.TeamUsers)
                .Select(tu => tu.UserId)
                .Distinct()
                .Union(GetAdministratorsIds());

            return teamMembersIds;
        }

        /// <inheritdoc />
        public IQueryable<string> GetProjectUserIdsForGroups(
            IQueryable<Group> groups, 
            IEnumerable<int> groupIds)
        {
            if (groups == null)
                throw new ArgumentNullException(nameof(groups));

            if (groupIds == null)
                throw new ArgumentNullException(nameof(groupIds));

            var userIds = groups
                .Where(gr => groupIds.Any(id => gr.Id == id))
                .SelectMany(g => g.Projects)
                .SelectMany(g => g.ProjectTeams)
                .SelectMany(pt => pt.Team.TeamUsers)
                .Select(tu => tu.UserId)
                .Distinct()
                .Union(GetAdministratorsIds());

            return userIds;
        }

        /// <inheritdoc />
        public IQueryable<string> GetUserIdsForTeams(IQueryable<Team> teams, IEnumerable<int> teamIds)
        {
            if (teams == null)
                throw new ArgumentNullException(nameof(teams));

            if (teamIds == null)
                throw new ArgumentNullException(nameof(teamIds));

            var teamMembersIds = teams
                .Where(t => teamIds.Any(id => t.Id == id))
                .SelectMany(t => t.TeamUsers)
                .Select(tu => tu.UserId)
                .Distinct()
                .Union(GetAdministratorsIds());

            return teamMembersIds;
        }

        public IQueryable<string> GetAdministratorsIds()
        {
            return _dataContext.UserRoles
                .Where(ur => ur.RoleId == _dataContext.Roles
                    .First(r => r.Name == RoleNames.ADMINISTRATOR_ROLE).Id)
                .Select(r => r.UserId);
        }

        public IQueryable<ApplicationUser> GetAdministrators()
        {
            return _dataContext.Users
                .Where(u => GetAdministratorsIds()
                    .Any(aId => aId == u.Id));
        }

        public bool IsAdmin(string userId)
        {
            return GetAdministratorsIds().Any(id => id == userId);
        }


        private readonly DataContext _dataContext;
    }
}