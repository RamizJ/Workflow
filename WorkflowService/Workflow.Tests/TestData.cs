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
        public IList<ApplicationUser> Users { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Team> Teams { get; set; }
        public IList<TeamUser> TeamUsers { get; set; }
        public IList<Scope> Scopes { get; set; }
        public IList<Goal> Goals { get; set; }
        public IList<Position> Positions { get; set; }


        public void Initialize(DataContext context, UserManager<ApplicationUser> userManager)
        {
            Positions = Builder<Position>.CreateListOfSize(10)
                .All()
                .Build();

            Users = Builder<ApplicationUser>.CreateListOfSize(10)
                .All()
                .With((x,i) => x.UserName = $"User{i}")
                .With((x, i) => x.NormalizedUserName = x.UserName.ToUpper())
                .With((x, i) => x.Email = $"Email{i}@email")
                .With((x, i) => x.NormalizedEmail = x.Email.ToUpper())
                .With((x, i) => x.PositionId = null)
                .TheFirst(6)
                .With((x, i) => x.FirstName = $"FirstName1{i}")
                .With((x, i) => x.LastName = $"LastName1{i}")
                .With((x, i) => x.MiddleName = $"MiddleName1{i}")
                .With((x, i) => x.PhoneNumber = $"Phone1{i}")
                .TheNext(4)
                .With((x, i) => x.FirstName = $"FirstName2{i}")
                .With((x, i) => x.LastName = $"LastName2{i}")
                .With((x, i) => x.MiddleName = $"MiddleName2{i}")
                .With((x, i) => x.PhoneNumber = $"Phone2{i}")
                .TheFirst(9).With(x => x.IsRemoved = false)
                .TheNext(1).With(x => x.IsRemoved = true)
                .Build().ToList();

            Groups = Builder<Group>.CreateListOfSize(2)
                .All().With(g => g.Name = $"Group{g.Id}").Build();
                
            Teams = Builder<Team>.CreateListOfSize(2)
                .All()
                .With(x => x.GroupId = null)
                .With(x => x.Name = $"Team{x.Id}")
                .Build();

            Scopes = Builder<Scope>.CreateListOfSize(10)
                .All()
                .With(s => s.OwnerId = Users.First().Id)
                .With(s => s.TeamId = null)
                .TheFirst(6).With((s, i) => s.Name = $"Scope1{i}")
                .TheNext(4).With((s, i) => s.Name = $"Scope2{i}")
                .TheFirst(3).With(s => s.GroupId = Groups[0].Id).With(s => s.TeamId = Teams[0].Id)
                .TheNext(7).With(s => s.GroupId = Groups[1].Id).With(s => s.TeamId = Teams[1].Id)
                .TheFirst(9).With(s => s.IsRemoved = false)
                .TheLast(1).With(s => s.IsRemoved = true)
                .Build();

            TeamUsers = Builder<TeamUser>.CreateListOfSize(1)
                .All().WithFactory(() => new TeamUser(Teams[0].Id, Users[1].Id))
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

            foreach (var user in Users)
                userManager.CreateAsync(user, "Aa010110!");

            //context.Users.AddRange(Users);
            context.Groups.AddRange(Groups);
            context.Teams.AddRange(Teams);
            context.TeamUsers.AddRange(TeamUsers);
            context.Scopes.AddRange(Scopes);
            context.Goals.AddRange(Goals);
            context.Positions.AddRange(Positions);

            context.SaveChanges();
        }
    }
}