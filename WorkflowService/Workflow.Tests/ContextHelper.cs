using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
            return new DataContext(options);
        }

        public static UserManager<ApplicationUser> CreateUserManager(DataContext context)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore, null, passwordHasher, null, null, null, null, null, null);
            return userManager;
        }

        public static ILoggerFactory GetLoggerFactory()
        {
            //var a = new ConsoleLoggerProvider((category, level) =>
            //    category == DbLoggerCategory.Database.Command.Name &&
            //    level == LogLevel.Information, true);

            return LoggerFactory.Create(builder => builder.AddConsole());
        }
    }
}
