using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Workflow.DAL;
using Workflow.DAL.Models;

namespace Workflow.Tests
{
    public static class ContextHelper
    {
        public static SqliteConnection OpenSqliteInMemoryConnection()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            return connection;
        }

        public static DbContextOptions<DataContext> GetSqliteOptions(SqliteConnection connection, bool logEnabled)
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            if (logEnabled)
            {
                builder.UseLoggerFactory(GetLoggerFactory())
                    .EnableSensitiveDataLogging();
            }
            var options = builder
                .UseSqlite(connection)
                .Options;


            return options;
        }

        public static DataContext CreateContext(SqliteConnection connection, bool isLogEnabled)
        {
            var options = GetSqliteOptions(connection, isLogEnabled);
            var context = new DataContext(options);
            context.Database.EnsureCreated();
            return context;
        }


        public static UserManager<ApplicationUser> CreateUserManager(DataContext context)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore, null, passwordHasher, null, null, null, null, null, null);
            return userManager;
        }

        public static RoleManager<IdentityRole> CreateRoleManager(DataContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);
            return roleManager;
        }

        public static ILoggerFactory GetLoggerFactory()
        {
            return LoggerFactory.Create(builder => builder.AddConsole());
        }


        public static ServiceProvider Initialize(SqliteConnection connection, bool isLogEnabled)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            serviceCollection.AddDbContext<DataContext>(options =>
            {
                if (isLogEnabled)
                {
                    options.UseLoggerFactory(GetLoggerFactory())
                        .EnableSensitiveDataLogging();
                }
                options.UseSqlite(connection);
            });

            serviceCollection.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                    options.User.RequireUniqueEmail = true;
                })
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddEntityFrameworkStores<DataContext>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
