using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL.Models;

namespace Workflow.DAL
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamUser> TeamUsers { get; set; }
        public DbSet<ProjectTeam> ProjectTeams { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalMessage> GoalMessages { get; set; }
        public DbSet<UserGoalMessage> UserGoalMessages { get; set; }
        public DbSet<GoalObserver> GoalObservers { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<FileData> FileData { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Metadata> Metadata { get; set; }
        public DbSet<ProjectUserRole> ProjectUserRoles { get; set; }


        public DataContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            SetupApplicationUser(builder);
            SetupGroup(builder);
            SetupTeamUser(builder);
            SetupGoalObserver(builder);
            SetupTeam(builder);
            SetupProject(builder);
            SetupProjectTeam(builder);
            SetupGoal(builder);
            SetupAttachment(builder);
            SetupPosition(builder);
            SetupGoalMessages(builder);
            SetupUserGoalMessages(builder);

            base.OnModelCreating(builder);
        }


        private void SetupGroup(ModelBuilder builder)
        { }

        private void SetupApplicationUser(ModelBuilder builder)
        {
            var entity = builder.Entity<ApplicationUser>();
            entity.Property(x => x.Id).ValueGeneratedOnAdd();
            entity.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(x => x.MiddleName).HasMaxLength(50);
            entity.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            entity.Property(x => x.PositionCustom).HasMaxLength(100);
            entity.HasIndex(x => x.UserName).IsUnique();
        }
        private void SetupTeamUser(ModelBuilder builder)
        {
            builder.Entity<TeamUser>()
                .HasKey(tu => new {tu.TeamId, tu.UserId});

            builder.Entity<TeamUser>()
                .HasOne(tu => tu.Team)
                .WithMany(t => t.TeamUsers)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TeamUser>()
                .HasOne(tu => tu.User)
                .WithMany(u => u.TeamUsers)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void SetupTeam(ModelBuilder builder)
        {
            var entity = builder.Entity<Team>();
            entity.Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired();
        }

        private void SetupProjectTeam(ModelBuilder builder)
        {
            builder.Entity<ProjectTeam>()
                .HasKey(pt => new { pt.ProjectId, pt.TeamId });

            builder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Team)
                .WithMany(t => t.TeamProjects)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Project)
                .WithMany(u => u.ProjectTeams)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void SetupProject(ModelBuilder builder)
        {
            var entity = builder.Entity<Project>();
            entity.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();

            var projectUserRole = builder.Entity<ProjectUserRole>();
            projectUserRole
                .HasKey(ptr => new { ptr.ProjectId, ptr.UserId });
            projectUserRole
                .HasOne(pur => pur.Project)
                .WithMany(p => p.UsersRoles);
            projectUserRole
                .HasOne(pur => pur.User)
                .WithMany(p => p.ProjectsRoles);
        }

        private void SetupGoal(ModelBuilder builder)
        {
            var entity = builder.Entity<Goal>();
            //entity.Property(t => t.OwnerId).IsRequired();
            entity.Property(t => t.CreationDate).IsRequired();
            entity.Property(t => t.Title).IsRequired();
            entity.Property(t => t.Title).HasMaxLength(512);
            entity.Property(t => t.Description).HasMaxLength(4096);
        }

        private void SetupGoalObserver(ModelBuilder builder)
        {
            var entity = builder.Entity<GoalObserver>();

            entity.HasKey(go => new {go.GoalId, go.ObserverId});

            entity.HasOne(go => go.Goal)
                .WithMany(g => g.Observers)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(go => go.Observer)
                .WithMany(o => o.GoalObserver)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void SetupAttachment(ModelBuilder builder)
        {
            var entity = builder.Entity<Attachment>();
            entity.Property(x => x.FileName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.CreationDate).IsRequired();
        }

        private void SetupPosition(ModelBuilder builder)
        {
            var entity = builder.Entity<Position>();
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        }

        private void SetupUserGoalMessages(ModelBuilder builder)
        {
            var entity = builder.Entity<UserGoalMessage>();

            entity.HasKey(x => new { x.GoalMessageId, x.UserId });

            entity.HasOne(x => x.GoalMessage)
                .WithMany(x => x.MessageSubscribers)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany(o => o.GoalMessages)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void SetupGoalMessages(ModelBuilder builder)
        {
            var entity = builder.Entity<GoalMessage>();

            entity.Property(x => x.Text).HasMaxLength(2048);
            entity.HasOne(x => x.Goal)
                .WithMany(x => x.Messages);
        }
    }
}
