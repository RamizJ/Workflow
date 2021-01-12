using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.DAL.Repositories.Abstract;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class UsersRepositoryTests
    {
        #region Setup

        [SetUp]
        public void Setup()
        {
            _dbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            using var serviceProvider = ContextHelper.Initialize(_dbConnection, false);
            var dataContext = serviceProvider.GetRequiredService<DataContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            dataContext.Database.EnsureCreated();
            _testData = new TestData();
            _testData.Initialize(dataContext, userManager);

            _serviceProvider = ContextHelper.Initialize(_dbConnection, true);
            _dataContext = _serviceProvider.GetRequiredService<DataContext>();
            _repositrory = _serviceProvider.GetRequiredService<IUsersRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _serviceProvider.Dispose();
            _dbConnection.Close();
        }

        #endregion


        #region GetUserIdsForGoalProjects

        [Test]
        public void GetUserIdsForNullGoalsProjectsTest()
        {
            Assert.Throws<ArgumentNullException>(
                () => _repositrory.GetUserIdsForGoalsProjects(null, new []{1,2,3}));
        }

        [Test]
        public void GetUserIdsForNullGoalIdsProjectsTest()
        {
            var goals = _dataContext.Goals;


            Assert.Throws<ArgumentNullException>(
                () => _repositrory.GetUserIdsForGoalsProjects(goals, null));
        }

        [Test]
        public async Task GetUserIdsForGoalProjectsTest()
        {
            var goals = _dataContext.Goals;
            var ids = new[] {1, 2, 3};

            var userIdsQeury = _repositrory.GetUserIdsForGoalsProjects(goals, ids);
            var userIds = await userIdsQeury.ToArrayAsync();

            Assert.NotNull(userIds);
        }

        #endregion

        #region GetUserIdsForProjects

        [Test]
        public void GetUserIdsForNullProjectsTest()
        {
            Assert.Throws<ArgumentNullException>(
                () => _repositrory.GetUserIdsForProjects(null, new[] { 1, 2, 3 }));
        }

        [Test]
        public void GetUserIdsForNullProjectIdsTest()
        {
            var projects = _dataContext.Projects;

            Assert.Throws<ArgumentNullException>(
                () => _repositrory.GetUserIdsForProjects(projects, null));
        }

        [Test]
        public async Task GetUserIdsForProjectsTest()
        {
            var projects = _dataContext.Projects;
            var ids = new[] { 1, 2, 3 };

            var userIdsQeury = _repositrory.GetUserIdsForProjects(projects, ids);
            var userIds = await userIdsQeury.ToArrayAsync();

            Assert.NotNull(userIds);
        }

        #endregion

        #region GetUserIdsForTeams

        [Test]
        public void GetUserIdsForNullTeamsTest()
        {
            Assert.Throws<ArgumentNullException>(
                () => _repositrory.GetUserIdsForTeams(null, new[] { 1, 2, 3 }));
        }

        [Test]
        public void GetUserIdsForNullTeamIdsTest()
        {
            var teams = _dataContext.Teams;

            Assert.Throws<ArgumentNullException>(
                () => _repositrory.GetUserIdsForTeams(teams, null));
        }

        [Test]
        public async Task GetUserIdsForTeamsTest()
        {
            var teams = _dataContext.Teams;
            var ids = new[] { 1, 2, 3 };

            var userIdsQeury = _repositrory.GetUserIdsForTeams(teams, ids);
            var userIds = await userIdsQeury.ToArrayAsync();

            Assert.NotNull(userIds);
        }

        #endregion

        #region GetUserIdsForGoalGroups

        [Test]
        public void GetUserIdsForNullGroupsTest()
        {
            Assert.Throws<ArgumentNullException>(
                () => _repositrory.GetProjectUserIdsForGroups(null, new[] { 1, 2, 3 }));
        }

        [Test]
        public void GetUserIdsForNullGroupIdsTest()
        {
            var groups = _dataContext.Groups;

            Assert.Throws<ArgumentNullException>(
                () => _repositrory.GetProjectUserIdsForGroups(groups, null));
        }

        [Test]
        public async Task GetUserIdsForGroupsTest()
        {
            var groups = _dataContext.Groups;
            var ids = new[] { 1, 2, 3 };

            var userIdsQeury = _repositrory.GetProjectUserIdsForGroups(groups, ids);
            var userIds = await userIdsQeury.ToArrayAsync();

            Assert.NotNull(userIds);
        }

        #endregion

        #region GetTeamMemberIdsForUsers

        [Test]
        public void GetTeamMemberIdsForNullUsersTest()
        {
            var ids = _testData.Users.Take(3).Select(x => x.Id);
            
            Assert.Throws<ArgumentNullException>(
                () => _repositrory.GetTeamMemberIdsForUsers(null, ids));
        }

        [Test]
        public void GetTeamMemberIdsForNullUserIdsTest()
        {
            var users = _dataContext.Users;

            Assert.Throws<ArgumentNullException>(
                () => _repositrory.GetTeamMemberIdsForUsers(users, null));
        }

        [Test]
        public async Task GetTeamMemberIdsForUsersTest()
        {
            var users = _dataContext.Users;
            var ids = _testData.Users.Take(3).Select(x => x.Id);

            var userIdsQeury = _repositrory.GetTeamMemberIdsForUsers(users, ids);
            var userIds = await userIdsQeury.ToArrayAsync();

            Assert.NotNull(userIds);
        }

        #endregion
        
        
        private SqliteConnection _dbConnection;
        private TestData _testData;
        private ServiceProvider _serviceProvider;
        private DataContext _dataContext;
        private IUsersRepository _repositrory;
    }
}