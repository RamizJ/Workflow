using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Workflow.DAL.Models;
using Workflow.DAL.Repositories.Abstract;

namespace Workflow.DAL.Repositories
{
    public class GoalsRepository : IGoalsRepository
    {
        public GoalsRepository(UserManager<ApplicationUser> userManager, DataContext dataContext)
        {
            _userManager = userManager;
            _dataContext = dataContext;
        }

        public async Task<IQueryable<Goal>> GetGoalsForUser(ApplicationUser user)
        {
            bool isAdmin = await _userManager.IsInRoleAsync(user, RoleNames.ADMINISTRATOR_ROLE);

            var query = _dataContext.Goals
                .Where(x => isAdmin
                            || x.Project.OwnerId == user.Id
                            || x.Project.ProjectTeams
                                .SelectMany(pt => pt.Team.TeamUsers)
                                .Any(tu => tu.UserId == user.Id));

            return query;
        }

        public IQueryable<Goal> GetPerformerGoalsForPeriod(ICollection<string> userIds, 
            DateTime dateBegin, DateTime dateEnd)
        {
            if (userIds == null || !userIds.Any() || dateBegin <= dateEnd)
            {
                throw new ArgumentException();
            }

            var userGoals = _dataContext.Goals
                .Where(g => userIds.Any(uId => g.Performer.Id == uId)
                            && (g.CreationDate >= dateBegin
                                && g.CreationDate <= dateEnd
                                || g.State != GoalState.Succeed));

            return userGoals;
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataContext _dataContext;
    }
}