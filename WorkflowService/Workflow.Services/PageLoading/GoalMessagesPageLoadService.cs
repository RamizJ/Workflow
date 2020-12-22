using System.Linq;
using PageLoading;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.PageLoading
{
    public class GoalMessagesPageLoadService : PageLoadService<GoalMessage>
    {
        protected override IQueryable<GoalMessage> FilterByWord(IQueryable<GoalMessage> query, string word)
        {
            return query.Where(x => x.Text.ToLower().Contains(word)
                                    || x.Owner.FirstName.ToLower().Contains(word)
                                    || x.Owner.MiddleName.ToLower().Contains(word)
                                    || x.Owner.LastName.ToLower().Contains(word)
                                    || x.Owner.Email.ToLower().Contains(word)
                                    || x.Owner.PhoneNumber.ToLower().Contains(word));
        }

        protected override IQueryable<GoalMessage> FilterByField(IQueryable<GoalMessage> query, FieldFilter field)
        {
            if (field.SameAs(nameof(VmGoalMessage.Id)))
            {
                query = FilterByValues(query, field.IntValues,
                    (q, v) => q.Where(x => x.Id.Equals(v)));
            }
            else if (field.SameAs(nameof(VmGoalMessage.GoalId)))
            {
                query = FilterByValues(query, field.IntValues,
                    (q, v) => q.Where(x => x.GoalId.Equals(v)));
            }
            else if (field.SameAs(nameof(VmGoalMessage.OwnerId)))
            {
                query = FilterByValues(query, field.StringLowerValues,
                    (q, v) => q.Where(x => x.OwnerId.ToLower().Contains(v)));
            }
            else if (field.SameAs(nameof(VmGoalMessage.Text)))
            {
                query = FilterByValues(query, field.StringLowerValues,
                    (q, v) => q.Where(x => x.Text.ToLower().Contains(v)));
            }
            else if (field.SameAs(nameof(VmGoalMessage.GoalTitle)))
            {
                query = FilterByValues(query, field.StringLowerValues,
                    (q, v) => q.Where(x => x.Goal.Title.ToLower().Contains(v)));
            }
            else if (field.SameAs(nameof(VmGoalMessage.OwnerFullName)))
            {
                query = FilterByValues(query, field.StringLowerValues,
                    (q, v) => q.Where(x => x.Owner.LastName.ToLower().Contains(v)
                                           || x.Owner.FirstName.ToLower().Contains(v)
                                           || x.Owner.MiddleName.ToLower().Contains(v)));
            }

            return query;
        }

        protected override IQueryable<GoalMessage> SortByField(IQueryable<GoalMessage> query, 
            FieldSort field, bool isAscending)
        {
            return query;
        }
    }
}