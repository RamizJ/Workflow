using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.DAL.Repositories.Abstract;
using Workflow.Services;
using Workflow.Services.Abstract;
using Workflow.Services.PageLoading;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;
using WorkflowService.Services;
using WorkflowService.Services.Abstract;

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
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddDbContext<DataContext>(options =>
            {
                if (isLogEnabled)
                {
                    options.UseLoggerFactory(GetLoggerFactory())
                        .EnableSensitiveDataLogging();
                }
                options.UseSqlite(connection);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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

            //View model converters
            services.AddTransient<IViewModelConverter<ApplicationUser, VmUser>, VmUserConverter>();
            services.AddTransient<IViewModelConverter<Team, VmTeam>, VmTeamConverter>();
            services.AddTransient<IViewModelConverter<Project, VmProject>, VmProjectConverter>();
            services.AddTransient<IViewModelConverter<ProjectUserRole, VmProjectUserRole>, VmProjectUserRoleConverter>();
            services.AddTransient<IViewModelConverter<ProjectTeam, VmProjectTeamRole>, VmProjectTeamRoleConverter>();
            services.AddTransient<IViewModelConverter<Group, VmGroup>, VmGroupConverter>();
            services.AddTransient<IViewModelConverter<Metadata, VmMetadata>, VmMetadataConverter>();

            //Services
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IDefaultDataInitializationService, DefaultDataInitializationService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IGroupsService, GroupsService>();
            services.AddTransient<IProjectsService, ProjectsService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<ITeamsService, TeamsService>();
            services.AddTransient<ITeamUsersService, TeamUsersService>();
            services.AddTransient<IProjectTeamsService, ProjectTeamsService>();
            services.AddTransient<ITeamProjectsService, TeamProjectsService>();
            services.AddTransient<IGoalsService, GoalsService>();
            services.AddTransient<IGoalAttachmentsService, GoalAttachmentsService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IFormFilesService, FormFilesService>();
            services.AddTransient<IProjectUserRolesService, ProjectUserRolesService>();
            services.AddTransient<IStatisticService, StatisticService>();
            services.AddTransient<IRolesService, RolesService>();
            services.AddTransient<IPageLoadService<Group>, GroupsPageLoadService>();
            services.AddTransient<IGoalsRepository, GoalsRepository>();

            return services.BuildServiceProvider();
        }
    }
}
