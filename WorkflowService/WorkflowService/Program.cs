using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using WorkflowService.Extensions;

#pragma warning disable 1591

namespace WorkflowService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .InitializeDefaultData()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
