using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services;
using Workflow.Services.Common;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class TeamProjectsServiceTests
    {
        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private ServiceProvider _serviceProvider;
        private TeamProjectsService _service;
        private ApplicationUser _currentUser;

        [SetUp]
        public void Setup()
        {
            _dbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            using var serviceProvider = ContextHelper.Initialize(_dbConnection, false);
            var dataContext = serviceProvider.GetRequiredService<DataContext>();
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            dataContext.Database.EnsureCreated();
            _testData = new TestData();
            _testData.Initialize(dataContext, userManager);

            _serviceProvider = ContextHelper.Initialize(_dbConnection, true);
            _dataContext = _serviceProvider.GetService<DataContext>();
            _service = new TeamProjectsService(_dataContext);
            _currentUser = _testData.Users.First();
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _serviceProvider.Dispose();
            _dbConnection.Close();
        }


        [TestCase(1, 0, 12, 6)]
        [TestCase(1, 0, 5, 5)]
        [TestCase(2, 0, 5, 3)]
        [TestCase(2, 2, 5, 0)]
        public async Task GetPageTest(int teamId, int pageNumber, int pageSize, int expectedCount)
        {
            //Arrange
            var teamProjects = Builder<ProjectTeam>.CreateListOfSize(10)
                .TheFirst(6).WithFactory(i => new ProjectTeam(_testData.Projects[i].Id, 1))
                .TheNext(4).WithFactory(i => new ProjectTeam(_testData.Projects[i].Id, 2))
                .Build();
            var context = ContextHelper.CreateContext(_dbConnection, false);
            context.RemoveRange(_testData.ProjectTeams);
            context.AddRange(teamProjects);
            context.SaveChanges();

            //Act
            var projects = (await _service.GetPage(_currentUser, teamId,
                pageNumber, pageSize,
                "", null, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, projects.Length);
        }

        [TestCase(1, "scope1", 6)]
        [TestCase(2, "scopedescription2", 3)]
        public async Task GetPageFilterTest(int teamId, string filter, int expectedCount)
        {
            //Arrange
            var teamProjects = Builder<ProjectTeam>.CreateListOfSize(10)
                .All().WithFactory(i => new ProjectTeam(_testData.Projects[i].Id, teamId))
                .Build();
            var context = ContextHelper.CreateContext(_dbConnection, false);
            context.RemoveRange(_testData.ProjectTeams);
            context.AddRange(teamProjects);
            context.SaveChanges();

            //Act
            var projects = (await _service.GetPage(_currentUser, teamId,
                0, 10,  filter, null, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, projects.Length);
        }

        [TestCase("Name", new[]{ "scope1", "scope2" }, 9)]
        [TestCase("Name", new[] { "scope1" }, 6)]
        public async Task GetPageFilterFieldsTest(string fieldName, 
            object[] values, int expectedCount)
        {
            //Arrange
            var teamProjects = Builder<ProjectTeam>.CreateListOfSize(10)
                .All().WithFactory(i => new ProjectTeam(_testData.Projects[i].Id, 1))
                .Build();
            var context = ContextHelper.CreateContext(_dbConnection, false);
            context.RemoveRange(_testData.ProjectTeams);
            context.AddRange(teamProjects);
            context.SaveChanges();
            var filterField = new FieldFilter(fieldName, values);

            //Act
            var projects = (await _service.GetPage(_currentUser, 1,
                0, 10, null, new[] {filterField}, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, projects.Length);
        }

        [TestCase("Name", SortType.Descending, new[] {9,8,7})]
        public async Task GetPageSortTest(string fieldName, SortType sortType, int[] ids)
        {
            //Arrange
            var teamProjects = Builder<ProjectTeam>.CreateListOfSize(10)
                .All().WithFactory(i => new ProjectTeam(_testData.Projects[i].Id, 1))
                .Build();
            var context = ContextHelper.CreateContext(_dbConnection, false);
            context.RemoveRange(_testData.ProjectTeams);
            context.AddRange(teamProjects);
            context.SaveChanges();
            var field = new FieldSort(fieldName, sortType);

            //Act
            var projects = (await _service.GetPage(_currentUser, 1,
                0, 10, null, null, new[] { field })).ToArray();

            //Assert
            for (int i = 0; i < ids.Length; i++) 
                Assert.AreEqual(ids[i], projects[i].Id);
        }

        [Test]
        public void AddExistedTest()
        {
            //Arrange 
            var teamProject = new ProjectTeam(1, 1);
            var context = ContextHelper.CreateContext(_dbConnection, false);
            context.RemoveRange(_testData.ProjectTeams);
            context.Add(teamProject);
            context.SaveChanges();

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await _service.Add(teamProject.TeamId, teamProject.ProjectId));
        }

        [Test]
        public void AddNotExistedTest()
        {
            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.Add(1, 100));
        }

        [Test]
        public async Task AddTest()
        {
            //Arrange
            var context = ContextHelper.CreateContext(_dbConnection, false);
            context.RemoveRange(_testData.ProjectTeams);
            context.SaveChanges();

            //Act
            await _service.Add(1, 1);
            var result = _dataContext.ProjectTeams.First();

            //Assert
            Assert.AreEqual(1, result.TeamId);
            Assert.AreEqual(1, result.ProjectId);
        }

        [Test]
        public void RemoveNotExistedTest()
        {
            //Arrange
            var context = ContextHelper.CreateContext(_dbConnection, false);
            context.RemoveRange(_testData.ProjectTeams);
            context.SaveChanges();

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.Remove(1, 1));
        }

        [Test]
        public async Task RemoveTest()
        {
            //Arrange
            var context = ContextHelper.CreateContext(_dbConnection, false);
            context.RemoveRange(_testData.ProjectTeams);
            context.Add(new ProjectTeam(1,1));
            context.SaveChanges();

            //Act
            await _service.Remove(1, 1);
            bool isAnyExist = _dataContext.ProjectTeams.Any();

            //Assert
            Assert.IsFalse(isAnyExist);
        }
    }
}