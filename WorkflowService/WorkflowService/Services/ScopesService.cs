using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;
using WorkflowService.Common;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Services
{
    /// <inheritdoc />
    public class ScopesService : IScopesService
    {
        private readonly DataContext _dataContext;
        private readonly VmScopeConverter _vmConverter;


        /// <summary>
        /// Database context
        /// </summary>
        /// <param name="dataContext"></param>
        public ScopesService(DataContext dataContext)
        {
            _dataContext = dataContext;
            _vmConverter = new VmScopeConverter();
        }


        /// <inheritdoc />
        public async Task<VmScope> GetScope(ApplicationUser user, int id)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var scope = await GetScopesQuery(user)
                .Select(s => _vmConverter.ToViewModel(s))
                .FirstOrDefaultAsync();

            return scope;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmScope>> GetAll(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var scopes = await GetScopesQuery(user)
                .Select(s => _vmConverter.ToViewModel(s))
                .ToArrayAsync();

            return scopes;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VmScope>> GetPage(ApplicationUser user, int pageNumber, int pageSize, 
            string filter, string[] filteredFields, 
            SortType sort, string[] sortedFields)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var query = GetScopesQuery(user);
            
            if (!string.IsNullOrEmpty(filter))
            {
                var words = filter.Split(" ");
                foreach (var word in words)
                {
                    query = query
                        .Where(s => s.Name.Contains(word)
                                    || s.Group.Name.Contains(word)
                                    || s.Team.Name.Contains(word)
                                    || s.Owner.FirstName.Contains(word)
                                    || s.Owner.MiddleName.Contains(word)
                                    || s.Owner.LastName.Contains(word));
                }
            }

            return await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .Select(s => _vmConverter.ToViewModel(s))
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public Task<IEnumerable<VmScope>> GetRange(ApplicationUser user, int[] ids)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<VmScope> CreateScope(ApplicationUser user, VmScope scope)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<VmScope> UpdateScope(ApplicationUser user, VmScope scope)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<VmScope> DeleteScope(ApplicationUser user, int scopeId)
        {
            throw new System.NotImplementedException();
        }


        private IQueryable<Scope> GetScopesQuery(ApplicationUser user)
        {
            var scopes = _dataContext.Scopes
                .Include(s => s.Owner)
                .Include(s => s.Team)
                .Include(s => s.Group)
                .Where(s => s.OwnerId == user.Id ||
                            s.Team.TeamUsers.Any(tu => tu.UserId == user.Id));

            return scopes;
        }
    }
}