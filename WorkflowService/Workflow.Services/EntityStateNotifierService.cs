using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Workflow.Services.Abstract;
using Workflow.Services.Hubs;
using Workflow.VM.ViewModels;
using static Workflow.Services.EntityStateHubMethods;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class EntityStateNotifierService : IEntityStateNotifierService
    {
        public EntityStateNotifierService(IHubContext<EntityStateHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <inheritdoc />
        public async Task Notify(VmEntityStateMessage message)
        {
            await _hubContext.Clients.All
                .SendAsync(EntityStateChanged, message);
        }


        private readonly IHubContext<EntityStateHub> _hubContext;
    }


    public static class EntityStateHubMethods
    {
        public const string EntityStateChanged = "EntityStateChanged";
    }
}