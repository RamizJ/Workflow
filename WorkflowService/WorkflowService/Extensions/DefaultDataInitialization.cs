using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Workflow.Services.Abstract;

namespace WorkflowService.Extensions
{
    /// <summary>
    /// Инициализация БД начальными данными
    /// </summary>
    public static class DefaultDataInitialization
    {
        /// <summary>
        /// Инициализация БД начальными данными
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IHost InitializeDefaultData(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var service = scope.ServiceProvider
                .GetRequiredService<IDefaultDataInitializationService>();

            service.InitializeRoles().Wait();
            service.InitializeAdmin().Wait();

            return host;
        }
    }
}
