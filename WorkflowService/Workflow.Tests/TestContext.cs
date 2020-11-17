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
        public DataContext DataContext { get; private set; }
        public UserManager<ApplicationUser> UserManager { get; private set; }
        public ApplicationUser CurrentUser { get; private set; }

        public void Initialize()
        {
            DbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            using var serviceProvider = ContextHelper.Initialize(DbConnection, false);
            var dataContext = serviceProvider.GetRequiredService<DataContext>();
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            dataContext.Database.EnsureCreated();
            TestData = new TestData();
            TestData.Initialize(dataContext, userManager);

            ServiceProvider = ContextHelper.Initialize(DbConnection, true);
            DataContext = ServiceProvider.GetService<DataContext>();
            UserManager = ServiceProvider.GetService<UserManager<ApplicationUser>>();
            CurrentUser = TestData.Users.First();
        }

        public void Uninitialize()
        {
            DataContext.Database.EnsureDeleted();
            ServiceProvider.Dispose();
            DbConnection.Close();
        }
    }
}