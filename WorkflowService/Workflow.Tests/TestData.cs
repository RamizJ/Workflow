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
        public IList<Project> Projects { get; set; }
        public List<ProjectTeam> ProjectTeams { get; set; }
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

            Teams = Builder<Team>.CreateListOfSize(10)
                .All()
                .With(x => x.GroupId = null)
                .With(x => x.CreatorId = Users.First().Id)
                .TheFirst(6).With(x => x.Name = $"Team1{x.Id}")
                .TheNext(4).With(x => x.Name = $"Team2{x.Id}")
                .TheFirst(9).With(x => x.IsRemoved = false)
                .TheLast(1).With(x => x.IsRemoved = true)
                .Build();

            Projects = Builder<Project>.CreateListOfSize(10)
                .All()
                .With(s => s.OwnerId = Users.First().Id)
                .TheFirst(6).With((s, i) => { 
                    s.Name = $"Scope1{i}"; 
                    s.Description = $"ScopeDescription1{i}";
                })
                .TheNext(4).With((s, i) => {
                    s.Name = $"Scope2{i}";
                    s.Description = $"ScopeDescription2{i}";
                })
                .TheFirst(3).With(s => s.GroupId = Groups[0].Id)
                .TheNext(7).With(s => s.GroupId = Groups[1].Id)
                .TheFirst(9).With(s => s.IsRemoved = false)
                .TheLast(1).With(s => s.IsRemoved = true)
                .Build();

            TeamUsers = Builder<TeamUser>.CreateListOfSize(10)
                .TheFirst(6).WithFactory(i => new TeamUser(Teams[0].Id, Users[i].Id))
                .TheNext(4).WithFactory(i => new TeamUser(Teams[1].Id, Users[i].Id))
                .Build().ToList();

            ProjectTeams = Builder<ProjectTeam>.CreateListOfSize(10)
                .TheFirst(6).WithFactory(i => new ProjectTeam(Projects[i].Id, Teams[0].Id))
                .TheNext(4).WithFactory(i => new ProjectTeam(Projects[i].Id, Teams[1].Id))
                .Build().ToList();

            Goals = Builder<Goal>.CreateListOfSize(20)
                .All()
                .With(x => x.ParentGoalId = null)
                .With(x => x.IsRemoved = false)
                .TheFirst(6)
                .With(x => x.ProjectId = Projects.First().Id)
                .With(x => x.OwnerId = Users[0].Id)
                .With(x => x.PerformerId = Users[0].Id)
                .With(x => x.Title = $"Goal1{x.Id}")
                .With(x => x.State = GoalState.Succeed)
                .With(x => x.Priority = GoalPriority.Low)
                .TheNext(4)
                .With(x => x.ProjectId = Projects.Last().Id)
                .With(x => x.OwnerId = Users[1].Id)
                .With(x => x.PerformerId = Users[1].Id)
                .With(x => x.Title = $"Goal2{x.Id}")
                .With(x => x.State = GoalState.Perform)
                .With(x => x.Priority = GoalPriority.High)
                //IsRemoved
                .TheFirst(9).With(x => x.IsRemoved = false)
                .TheNext(1).With(x => x.IsRemoved = true)
                //Parent goal
                .TheNext(6)
                .With(x => x.ProjectId = Projects.First().Id)
                .With(x => x.OwnerId = Users[0].Id)
                .With(x => x.PerformerId = Users[0].Id)
                .With(x => x.ParentGoalId = 1)
                .With(x => x.Title = $"ChildGoal1{x.Id}")
                .With(x => x.State = GoalState.Succeed)
                .With(x => x.Priority = GoalPriority.Low)
                .TheNext(4)
                .With(x => x.ProjectId = Projects.Last().Id)
                .With(x => x.OwnerId = Users[0].Id)
                .With(x => x.PerformerId = Users[0].Id)
                .With(x => x.ParentGoalId = 10)
                .With(x => x.Title = $"ChildGoal2{x.Id}")
                .With(x => x.State = GoalState.Succeed)
                .With(x => x.Priority = GoalPriority.Low)
                .Build().ToList();

            foreach (var user in Users)
                userManager.CreateAsync(user, "Aa010110!");

            context.Groups.AddRange(Groups);
            context.Teams.AddRange(Teams);
            context.TeamUsers.AddRange(TeamUsers);
            context.Projects.AddRange(Projects);
            context.ProjectTeams.AddRange(ProjectTeams);
            context.Goals.AddRange(Goals);
            context.Positions.AddRange(Positions);

            context.SaveChanges();
        }
    }
}