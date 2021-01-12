using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Workflow.DAL.Models;

namespace Workflow.DAL.Repositories.Abstract
{
    public interface IGoalsRepository
    {
        IQueryable<Goal> GetGoalsForUser(
            IQueryable<Goal> goalsQuery, 
            ApplicationUser user);

        IQueryable<Goal> GetPerformerGoals(
            IQueryable<Goal> goalsQuery,
            ICollection<string> userIds);

        IQueryable<Goal> GetGoalsForPeriod(
            IQueryable<Goal> goalsQuery,
            DateTime dateBegin, DateTime dateEnd);

        IQueryable<Goal> GetGoalsForProjects(
            IQueryable<Goal> goalsQuery,
            ICollection<int> projectIds);
    }
}
