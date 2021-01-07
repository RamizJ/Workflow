using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.DAL.Repositories.Abstract;
using Workflow.Services.Abstract;
using Workflow.Services.Hubs;
using Workflow.VM.ViewModels;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class EntityStateNotifierService : IEntityStateNotifierService
    {
        public EntityStateNotifierService(DataContext dataContext,
            IHubContext<EntityStateHub> hubContext,
            IUsersRepository usersRepository)
        {
            _dataContext = dataContext;
            _hubContext = hubContext;
            _usersRepository = usersRepository;
        }

        /// <inheritdoc />
        public async Task Notify(VmEntityStateMessage message)
        {
            var recieverIds = await GetReceiverIds(message);
            if (recieverIds == null)
            {
                await _hubContext.Clients.All
                    .SendAsync(EntityStateHubMethods.EntityStateChanged, message);
            }
            else
            {
                await _hubContext.Clients
                    .Users(recieverIds.Except(new[] { message.SenderId }).ToList())
                    .SendAsync(EntityStateHubMethods.EntityStateChanged, message);
            }
        }

        
        private async Task<IEnumerable<string>> GetReceiverIds(
            VmEntityStateMessage message)
        {
            IEnumerable<string> receiverIds;
            switch (message.EntityType)
            {
                case nameof(Goal):
                    receiverIds = await _usersRepository
                        .GetUserIdsForGoalsProjects(_dataContext.Goals, message.EntityIds.Cast<int>())
                        .ToArrayAsync();
                    break;

                case nameof(Project):
                    receiverIds = await _usersRepository
                        .GetUserIdsForProjects(_dataContext.Projects, message.EntityIds.Cast<int>())
                        .ToArrayAsync();
                    break;

                case nameof(ApplicationUser):
                    receiverIds = await _usersRepository
                        .GetTeamMemberIdsForUsers(_dataContext.Users, message.EntityIds.Cast<string>())
                        .ToArrayAsync();
                    break;

                case nameof(Group):
                    receiverIds = await _usersRepository
                        .GetProjectUserIdsForGroups(_dataContext.Groups, message.EntityIds.Cast<int>())
                        .ToArrayAsync();
                    break;

                case nameof(Team):
                    receiverIds = await _usersRepository
                        .GetUserIdsForTeams(_dataContext.Teams, message.EntityIds.Cast<int>())
                        .ToArrayAsync();
                    break;

                default:
                    return null;
            }

            return receiverIds;
        }

        
        private readonly DataContext _dataContext;
        private readonly IHubContext<EntityStateHub> _hubContext;
        private readonly IUsersRepository _usersRepository;
    }


    public static class EntityStateHubMethods
    {
        public const string EntityStateChanged = "EntityStateChanged";
    }
}