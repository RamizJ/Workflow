using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Workflow.DAL.Models;

namespace Workflow.DAL.Repositories.Abstract
{
    public interface IGoalsRepository
    {
        Task<IQueryable<Goal>> GetGoalsForUser(ApplicationUser user);

        IQueryable<Goal> GetPerformerGoalsForPeriod(ICollection<string> userIds,
            DateTime dateBegin, DateTime dateEnd);
    }
}
