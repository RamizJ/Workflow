using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Workflow.DAL;
using Workflow.DAL.Models;

namespace Workflow.Tests
{
    public class TestContext
    {
        public SqliteConnection DbConnection { get; private set; }
        public TestData TestData { get; private set; }
        public ServiceProvider ServiceProvider { get; private set; }
        public ApplicationUser CurrentUser { get; private set; }

        public IServiceProvider Initialize(out TestData testData)
        {
            DbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            using var serviceProvider = ContextHelper.Initialize(DbConnection, false);
            var dataContext = serviceProvider.GetRequiredService<DataContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            dataContext.Database.EnsureCreated();
            testData = TestData = new TestData();
            TestData.Initialize(dataContext, userManager);

            ServiceProvider = ContextHelper.Initialize(DbConnection, true);
            CurrentUser = TestData.Users.First();

            return ServiceProvider;
        }

        public void Uninitialize()
        {
            var dataContext = ServiceProvider.GetRequiredService<DataContext>();
            dataContext.Database.EnsureDeleted();
            ServiceProvider.Dispose();
            DbConnection.Close();
        }
    }
}