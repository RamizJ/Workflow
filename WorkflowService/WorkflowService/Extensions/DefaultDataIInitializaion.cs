using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Extensions
{
    public static class DefaultDataIInitializaion
    {
        public static IHost InitializeDefaultData(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var defaultDataInitializationService = scope.ServiceProvider
                .GetService<IDefaultDataInitializationService>();

            defaultDataInitializationService.InitializeRoles().Wait();
            defaultDataInitializationService.InitializeAdmin().Wait();

            return host;
        }
    }
}
