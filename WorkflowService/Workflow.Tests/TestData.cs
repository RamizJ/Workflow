using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Identity;
using Workflow.DAL;
using Workflow.DAL.Models;

namespace Workflow.Tests
{
    public class TestData
    {
        public List<Scope> Scopes { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public Team Team { get; set; }
        public List<TeamUser> TeamUsers { get; set; }
        public List<Goal> Goals { get; set; }


        public void Initialize(DataContext context, UserManager<ApplicationUser> userManager)
        {
            Users = Builder<ApplicationUser>.CreateListOfSize(3)
                .All()
                .With((x,i) => x.UserName = $"User {i}")
                .With((x, i) => x.NormalizedUserName = x.UserName.ToUpper())
                .With((x, i) => x.Email = $"Email {i}")
                .With((x, i) => x.NormalizedEmail = x.Email.ToUpper())
                .With((x, i) => x.FirstName = $"FirstName{i}")
                .With((x, i) => x.LastName = $"LastName{i}")
                .With((x, i) => x.PositionId = null)
                .Build().ToList();

            Team = Builder<Team>.CreateNew()
                .With(x => x.GroupId = null)
                .Build();
            Scopes = Builder<Scope>.CreateListOfSize(2).All()
                .With(x => x.OwnerId = Users.First().Id)
                .With(x => x.GroupId = null)
                .TheFirst(1).With(x => x.TeamId = null)
                .TheNext(1).With(x => x.TeamId = Team.Id)
                .Build().ToList();
            TeamUsers = Builder<TeamUser>.CreateListOfSize(1)
                .All().WithFactory(() => new TeamUser(Team.Id, Users[1].Id))
                .Build().ToList();

            Goals = Builder<Goal>.CreateListOfSize(10)
                .TheFirst(5).WithFactory(i => new Goal
                {
                    OwnerId = Users[0].Id,
                    PerformerId = Users[0].Id,
                    Title = $"Goal {i}",
                    ScopeId = Scopes[0].Id
                })
                .With(x => x.AttachmentId = null)
                .With(x => x.ParentGoalId = null)
                .TheNext(5).WithFactory(i => new Goal
                {
                    OwnerId = Users[1].Id,
                    PerformerId = Users[1].Id,
                    Title = $"Goal {i}",
                    ScopeId = Scopes[1].Id
                })
                .With(x => x.AttachmentId = null)
                .With(x => x.ParentGoalId = null)
                .Build().ToList();

            context.Users.AddRange(Users);
            context.Scopes.AddRange(Scopes);
            context.Teams.AddRange(Team);
            context.TeamUsers.AddRange(TeamUsers);
            context.Goals.AddRange(Goals);

            context.SaveChanges();
        }
    }
}