using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Workflow.Services.Hubs
{
    public class EntityStateHub : Hub
    {
        public async Task SomeMethod()
        { 
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
