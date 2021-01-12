using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Workflow.DAL.Models;
using Workflow.DAL.Repositories.Abstract;

namespace Workflow.DAL.Repositories
{
    public class GoalsRepository : IGoalsRepository
    {
        public GoalsRepository(UserManager<ApplicationUser> userManager, 
            IUsersRepository usersRepository,
            DataContext dataContext)
        {
            _userManager = userManager;
            _usersRepository = usersRepository;
            _dataContext = dataContext;
        }

        public IQueryable<Goal> GetGoalsForUser(
            IQueryable<Goal> goalsQuery, 
            ApplicationUser user)
        {
            var query = goalsQuery
                .Where(x => _usersRepository.IsAdmin(user.Id)
                            || x.Project.OwnerId == user.Id
                            || x.Project.ProjectTeams
                                .SelectMany(pt => pt.Team.TeamUsers)
                                .Any(tu => tu.UserId == user.Id));

            return query;
        }

        public IQueryable<Goal> GetPerformerGoals(
            IQueryable<Goal> goalsQuery,
            ICollection<string> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                throw new ArgumentException();
            }

            var userGoals = goalsQuery
                .Where(g => userIds.Any(uId => g.Performer.Id == uId)
                            && g.State != GoalState.Succeed);

            return userGoals;
        }

        public IQueryable<Goal> GetGoalsForPeriod(
            IQueryable<Goal> goalsQuery,
            DateTime dateBegin, DateTime dateEnd)
        {
            if (dateBegin >= dateEnd)
            {
                throw new ArgumentException();
            }

            var userGoals = goalsQuery
                .Where(g => g.CreationDate >= dateBegin
                            && g.CreationDate <= dateEnd);

            return userGoals;
        }

        public IQueryable<Goal> GetGoalsForProjects(
            IQueryable<Goal> goalsQuery,
            ICollection<int> projectIds)
        {
            if (projectIds == null)
                throw new ArgumentNullException(nameof(projectIds));
            
            if (!projectIds.Any())
                return goalsQuery;

            var userGoals = goalsQuery
                .Where(g => projectIds
                    .Any(pId => g.ProjectId == pId));

            return userGoals;
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsersRepository _usersRepository;
        private readonly DataContext _dataContext;
    }
}