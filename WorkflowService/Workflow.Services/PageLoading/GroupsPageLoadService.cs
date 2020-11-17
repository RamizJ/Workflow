using System.Linq;
using PageLoading;
using Workflow.DAL.Models;
using Workflow.Services.Exceptions;
using Workflow.VM.ViewModels;
using static System.Net.HttpStatusCode;

namespace Workflow.Services.PageLoading
{
    public class GroupsPageLoadService : PageLoadService<Group>
    {
        protected override IQueryable<Group> FilterByWord(IQueryable<Group> query, string word)
        {
            return query.Where(x => x.Name.ToLower().Contains(word)
                                    || x.Description.ToLower().Contains(word)
                                    || x.Owner.FirstName.ToLower().Contains(word)
                                    || x.Owner.MiddleName.ToLower().Contains(word)
                                    || x.Owner.LastName.ToLower().Contains(word)
                                    || x.Owner.Email.ToLower().Contains(word)
                                    || x.Owner.PhoneNumber.ToLower().Contains(word));
        }

        protected override IQueryable<Group> FilterByField(IQueryable<Group> query, FieldFilter field)
        {
            if (field.SameAs(nameof(VmGroup.Id)))
            {
                query = FilterByValues(query, field.IntValues,
                    (q, v) => q.Where(x => x.Id.Equals(v)));
            }
            else if (field.SameAs(nameof(VmGroup.ParentGroupId)))
            {
                query = FilterByValues(query, field.IntValues,
                    (q, v) => q.Where(x => x.ParentGroupId.Equals(v)));
            }
            else if (field.SameAs(nameof(VmGroup.CreationDate)))
            {
                query = FilterByValues(query, field.DateValues,
                    (q, v) => q.Where(x => x.CreationDate.Equals(v)));
            }
            else if (field.SameAs(nameof(VmGroup.Name)))
            {
                query = FilterByValues(query, field.StringLowerValues,
                    (q, v) => q.Where(x => x.Name.ToLower().Contains(v)));
            }
            else if (field.SameAs(nameof(VmGroup.Description)))
            {
                query = FilterByValues(query, field.StringLowerValues,
                    (q, v) => q.Where(x => x.Description.ToLower().Contains(v)));
            }
            else if (field.SameAs(nameof(VmGroup.IsRemoved)))
            {
                query = FilterByValues(query, field.BoolValues,
                    (q, v) => q.Where(x => x.IsRemoved.Equals(v)));
            }
            else
            {
                throw new HttpResponseException(BadRequest, 
                    $"The search for field '{field.FieldName}' not supported");
            }

            return query;
        }

        protected override IQueryable<Group> SortByField(IQueryable<Group> query, FieldSort field, bool isAscending)
        {
            if (field.Is(nameof(VmGroup.Name)))
                query = query.SortBy(x => x.Name, isAscending);

            else if (field.Is(nameof(VmGroup.Description)))
                query = query.SortBy(x => x.Description, isAscending);

            else if (field.Is(nameof(VmGroup.IsRemoved)))
                query = query.SortBy(x => x.IsRemoved, isAscending);

            else if (field.Is(nameof(VmGroup.OwnerFio)))
                query = query
                    .SortBy(p => p.Owner.LastName, isAscending)
                    .SortBy(p => p.Owner.FirstName, isAscending)
                    .SortBy(p => p.Owner.MiddleName, isAscending);

            return query;
        }
    }
}