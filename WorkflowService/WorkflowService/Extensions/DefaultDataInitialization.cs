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

            var defaultDataInitializationService = scope.ServiceProvider
                .GetService<IDefaultDataInitializationService>();

            defaultDataInitializationService.InitializeRoles().Wait();
            defaultDataInitializationService.InitializeAdmin().Wait();

            return host;
        }
    }
}
