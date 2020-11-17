using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services;
using Workflow.Services.Abstract;
using Workflow.Services.PageLoading;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;
using WorkflowService.Filters;
using WorkflowService.Services;
using WorkflowService.Services.Abstract;
using static Workflow.Services.ConfigKeys;

#pragma warning disable 1591    //Disable xml documentation for this file

namespace WorkflowService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString(DB_CONNECTION_STRING);
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connectionString, o => o.MigrationsAssembly("Workflow.DAL")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration[JWT_ISSUER],
                    ValidAudience = Configuration[JWT_AUDIENCE],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[JWT_SIGNING_KEY]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/entity-state-observer"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();

            if (Configuration.GetValue<bool>(IS_API_TEST_MODE))
            {
                services.AddCors(options => options
                    .AddDefaultPolicy(builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()));
            }

            services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()))
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });

            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new OpenApiInfo {Title = "Workflow API", Version = "31.08.2020"});
                setup.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                setup.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "WorkflowService.xml");
                setup.IncludeXmlComments(filePath);
            });
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });


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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) 
                app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            if (!env.IsDevelopment()) 
                app.UseSpaStaticFiles();

            app.UseRouting();

            if(Configuration.GetValue<bool>(IS_API_TEST_MODE))
                app.UseCors();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("./v1/swagger.json", "Workflow API V1");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa => { });
        }
    }
}
