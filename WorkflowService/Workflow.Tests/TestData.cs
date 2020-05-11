using System;
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
        public List<Team> Teams { get; set; }
        public List<TeamUser> TeamUsers { get; set; }
        public List<Goal> Goals { get; set; }


        public void Initialize(DataContext context, UserManager<ApplicationUser> userManager)
        {
            Users = new List<ApplicationUser>
            {
                CreateUser(1, userManager),
                CreateUser(2, userManager)
            };

            Teams = Builder<Team>.CreateListOfSize(2)
                .All().With(x => x.GroupId = null)
                .Build().ToList();
            Scopes = Builder<Scope>.CreateListOfSize(2).All()
                .With(x => x.OwnerId = Users[0].Id)
                .With((x,i) => x.TeamId = Teams[i].Id)
                .With(x => x.GroupId = null)
                .Build().ToList();
            TeamUsers = Builder<TeamUser>.CreateListOfSize(3)
                .TheFirst(1).WithFactory(() => new TeamUser(Teams[0].Id, Users[0].Id))
                .TheNext(1).WithFactory(() => new TeamUser(Teams[1].Id, Users[0].Id))
                .TheNext(1).WithFactory(() => new TeamUser(Teams[1].Id, Users[1].Id))
                .Build().ToList();

            Goals = Builder<Goal>.CreateListOfSize(10)
                .TheFirst(5).WithFactory(i => new Goal
                {
                    CreatorId = Users[0].Id,
                    PerformerId = Users[0].Id,
                    Title = $"Goal {i}",
                    ScopeId = Scopes[0].Id
                })
                .With(x => x.AttachmentId = null)
                .With(x => x.ParentGoalId = null)
                .TheNext(5).WithFactory(i => new Goal
                {
                    CreatorId = Users[1].Id,
                    PerformerId = Users[1].Id,
                    Title = $"Goal {i}",
                    ScopeId = Scopes[1].Id
                })
                .With(x => x.AttachmentId = null)
                .With(x => x.ParentGoalId = null)
                .Build().ToList();

            context.Users.AddRange(Users);
            context.Scopes.AddRange(Scopes);
            context.Teams.AddRange(Teams);
            context.TeamUsers.AddRange(TeamUsers);
            context.Goals.AddRange(Goals);

            context.SaveChanges();
        }

        private static ApplicationUser CreateUser(int number, UserManager<ApplicationUser> userManager)
        {
            var user =  new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = $"User {number}",
                Email = $"user{number}@user.ru",
                PhoneNumber = $"1-000-000-00-0{number}",
                FirstName = $"FirstName {number}",
                LastName = $"LastName {number}"
            };

            userManager.CreateAsync(user, user.Id).RunSynchronously();
            return user;
        }
    }
}