using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Org.BouncyCastle.Asn1;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services;
using Workflow.Services.Common;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class GoalsServiceTest
    {
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
            _userManager = _serviceProvider.GetService<UserManager<ApplicationUser>>();
            _service = new GoalsService(_dataContext, _userManager);
            _currentUser = _testData.Users.First();
            _vmConverter = new VmGoalConverter();
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _serviceProvider.Dispose();
            _dbConnection.Close();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task GetTest(int goalId)
        {
            //Arrange
            var expectedGoal = _testData.Goals.First(t => t.Id == goalId);

            //Act
            var team = await _service.Get(_currentUser, goalId);

            //Assert
            Assert.AreEqual(expectedGoal.Id, team.Id);
            Assert.AreEqual(expectedGoal.Title, team.Title);
            Assert.AreEqual(expectedGoal.Description, team.Description);
        }


        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1000)]
        public async Task GetNotExistedTest(int goalId)
        {
            //Arrange

            //Act
            var goal = await _service.Get(_currentUser, goalId);

            //Assert
            Assert.IsNull(goal);
        }

        [TestCase(0, 12, 9)]
        [TestCase(0, 5, 5)]
        [TestCase(1, 5, 4)]
        [TestCase(2, 5, 0)]
        public async Task GetPageTest(int pageNumber, int pageSize, int expectedCount)
        {
            //Arrange
            var projectId = _testData.Projects.First().Id;

            //Act
            var goals = (await _service.GetPage(_currentUser, projectId,
                pageNumber, pageSize,
                "", null, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, goals.Length);
        }

        [TestCase(0, 12, null, 9)]
        [TestCase(0, 5, "Goal1", 5)]
        [TestCase(1, 5, "Goal1", 1)]
        [TestCase(0, 5, "Goal2", 3)]
        public async Task GetPageFilterTest(int pageNumber, int pageSize,
            string filter, int expectedCount)
        {
            //Arrange
            var projectId = _testData.Projects.First().Id;

            //Act
            var result = (await _service.GetPage(_currentUser, projectId, 
                pageNumber, pageSize,
                filter, null, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, result.Length);
        }

        [TestCase(0, 5, null, "Title", "Goal1", false, 5)]
        [TestCase(0, 5, null, "Description", "Description1", false, 1)]
        [TestCase(0, 5, null, "Title", "Goal2", true, 4)]
        public async Task GetPageFilterFieldsTest(int pageNumber, int pageSize,
            string filter, string fieldName, object value, bool withRemoved, int expectedCount)
        {
            //Arrange
            var projectId = _testData.Projects.First().Id;

            //Act
            var filterField = new FieldFilter(fieldName, value);
            var resultScopes = (await _service.GetPage(_currentUser, projectId,
                pageNumber, pageSize,
                filter, new[] { filterField }, null, withRemoved)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }

        [TestCase(new[] { 1, 2, 3 })]
        [TestCase(new[] { 9, 10 })]
        public async Task GetRangeTest(int[] ids)
        {
            //Arrange

            //Act
            var resultScopes = (await _service.GetRange(_currentUser, ids)).ToArray();

            //Assert
            Assert.AreEqual(ids.Length, resultScopes.Length);
            for (var i = 0; i < ids.Length; i++)
            {
                Assert.AreEqual(ids[i], resultScopes[i].Id);
            }
        }

        [TestCase(0, 5, "", "Title", SortType.Ascending, new[] { 1, 2, 3 })]
        [TestCase(0, 5, "", "Title", SortType.Descending, new[] { 9, 8, 7 })]
        [TestCase(0, 5, "Goal2", "Title", SortType.Descending, new[] { 9, 8, 7 })]
        public async Task GetPageWithSortingTest(int pageNumber, int pageSize,
            string filter, string fieldName, SortType sortType, int[] expectedIds)
        {
            //Arrange
            var projectId = _testData.Projects.First().Id;

            //Act
            var sortField = new FieldSort(fieldName, sortType);
            var resultScopes = (await _service.GetPage(_currentUser,
                projectId, pageNumber, pageSize,
                filter, null, new[] { sortField })).ToArray();

            //Assert
            Assert.LessOrEqual(expectedIds.Length, resultScopes.Length);
            for (var i = 0; i < expectedIds.Length; i++)
            {
                Assert.AreEqual(expectedIds[i], resultScopes[i].Id);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public async Task CreateTest(int id)
        {
            //Arrange
            var vmTeam = new VmGoal
            {
                Id = id,
                Title = "Goal3",
                ProjectId = _testData.Projects.First().Id,
                IsRemoved = false
            };

            //Act
            var result = await _service.Create(_currentUser, vmTeam);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testData.Projects.Count + 1, result.Id);
            Assert.AreEqual(vmTeam.Title, result.Title);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void CreateInvalidTitleTest(string title)
        {
            //Arrange
            var vmTeam = new VmGoal
            {
                Id = 0,
                Title = title,
                ProjectId = _testData.Projects.First().Id,
                IsRemoved = false
            };

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.Create(_currentUser, vmTeam));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void UpdateInvalidTitleTest(string title)
        {
            //Arrange
            int goalId = 1;
            var vmTeam = new VmGoal
            {
                Id = goalId,
                Title = title,
                ProjectId = _testData.Projects.First().Id,
                IsRemoved = false
            };

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.Update(_currentUser, vmTeam));
        }

        [TestCase("goal123")]
        [TestCase("g")]
        [TestCase("1")]
        public void UpdateTitleTest(string title)
        {
            //Arrange
            int goalId = 1;
            var vmTeam = new VmGoal
            {
                Id = goalId,
                Title = title,
                ProjectId = _testData.Projects.First().Id,
                IsRemoved = false
            };

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.Update(_currentUser, vmTeam));
        }




        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private ServiceProvider _serviceProvider;
        private GoalsService _service;
        private ApplicationUser _currentUser;
        private VmGoalConverter _vmConverter;
        private UserManager<ApplicationUser> _userManager;
    }
}