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
using Workflow.Services;
using Workflow.VM.Common;
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
            _usersService = new UsersService(_dataContext, _userManager);
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

        [TestCase(0, 12, 6)]
        [TestCase(0, 5, 5)]
        [TestCase(1, 5, 1)]
        [TestCase(2, 5, 0)]
        public async Task GetPageTest(int pageNumber, int pageSize, int expectedCount)
        {
            //Arrange
            var projectId = _testData.Projects.First().Id;
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
            };

            //Act
            var goals = (await _service.GetPage(_currentUser, projectId, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, goals.Length);
        }

        [TestCase(0, 12, null, 9)]
        [TestCase(0, 5, "Goal1", 5)]
        [TestCase(1, 5, "Goal1", 1)]
        [TestCase(0, 5, "Goal2", 3)]
        [TestCase(1, 5, "Scope1", 1)]
        public async Task GetPageFilterTest(int pageNumber, int pageSize,
            string filter, int expectedCount)
        {
            //Arrange
            //var projectId = _testData.Projects.First().Id;

            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = filter
            };

            //Act
            var result = (await _service.GetPage(_currentUser, null, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, result.Length);
        }

        [TestCase(0, 5, 1, "Title", new object[] { "Goal1" }, false, 5)]
        [TestCase(0, 5, 1, "Description", new object[] { "Description1" }, false, 1)]
        [TestCase(0, 5, 10, "Title", new object[] { "Goal2" }, true, 4)]
        [TestCase(0, 5, 10, "State", new object[] { GoalState.Perform }, true, 4)]
        [TestCase(0, 5, 10, "Priority", new object[] { GoalPriority.High }, true, 4)]
        public async Task GetPageFilterFieldsTest(int pageNumber, int pageSize,
            int projectId, string fieldName, object[] values, bool withRemoved, int expectedCount)
        {
            //Arrange
            var filterField = new FieldFilter(fieldName, values);
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                FilterFields = new[] { filterField },
                WithRemoved = withRemoved
            };

            //Act
            var resultScopes = (await _service.GetPage(_currentUser, projectId, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }

        [Test]
        public async Task GetPageForNewUserTest()
        {
            //Arrange
            var vmUser = new VmUser
            {
                LastName = "New",
                FirstName = "New",
                Email = "New@New",
                UserName = "New"
            };
            var vmNewUser = await _usersService.Create(vmUser, "new12345!");
            var newUser = new VmUserConverter().ToModel(vmNewUser);

            var pageOptions = new PageOptions
            {
                PageNumber = 0,
                PageSize = 10,
            };
            

            //Act
            var goals = (await _service.GetPage(newUser, null, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(0, goals.Length);
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

        [TestCase(0, 5, 1, "", "Title", SortType.Ascending, new[] { 1, 2, 3 })]
        [TestCase(0, 5, 10, "", "Title", SortType.Descending, new[] { 9, 8, 7 })]
        [TestCase(0, 5, 10, "Goal2", "Title", SortType.Descending, new[] { 9, 8, 7 })]
        public async Task GetPageWithSortingTest(int pageNumber, int pageSize, int projectId,
            string filter, string fieldName, SortType sortType, int[] expectedIds)
        {
            //Arrange
            var sortField = new FieldSort(fieldName, sortType);
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = filter,
                SortFields = new[] { sortField }
            };

            //Act
            var resultScopes = (await _service.GetPage(_currentUser, projectId, pageOptions)).ToArray();

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
            var vmGoal = new VmGoal
            {
                Id = id,
                Title = "Goal3",
                ProjectId = _testData.Projects.First().Id,
                IsRemoved = false
            };

            //Act
            var result = await _service.Create(_currentUser, vmGoal);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testData.Projects.Count + 1, result.Id);
            Assert.AreEqual(vmGoal.Title, result.Title);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void CreateInvalidTitleTest(string title)
        {
            //Arrange
            var vmGoal = new VmGoal
            {
                Id = 0,
                Title = title,
                ProjectId = _testData.Projects.First().Id,
                IsRemoved = false
            };

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.Create(_currentUser, vmGoal));
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public async Task UpdateInvalidTitleTest(string title)
        {
            //Arrange
            int goalId = 1;
            var vmGoal = new VmGoal
            {
                Id = goalId,
                Title = title,
                ProjectId = _testData.Projects.First().Id,
                IsRemoved = false
            };

            //Act
            await _service.Update(_currentUser, vmGoal);

            //Assert
            Assert.Pass();
        }

        [TestCase("goal123", "d1", 10, 1, GoalState.Perform, GoalPriority.Low)]
        [TestCase("g", "d2", 11, 2, GoalState.Testing, GoalPriority.Normal)]
        [TestCase("1", "d3", 12, 3, GoalState.New, GoalPriority.High)]
        public async Task UpdateTitleTest(string title, string description, 
            int goalNumber, int projectId, GoalState state, GoalPriority priority)
        {
            //Arrange
            int goalId = 1;
            var vmGoal = new VmGoal
            {
                Id = goalId,
                Title = title,
                Description = description,
                ProjectId = projectId,
                State = state,
                Priority = priority,
                GoalNumber = goalNumber,
                IsRemoved = true
            };

            //Act
            await _service.Update(_currentUser, vmGoal);
            var goal = _dataContext.Goals.First(g => g.Id == goalId);

            //Assert
            Assert.AreEqual(title, goal.Title);
            Assert.AreEqual(description, goal.Description);
            Assert.AreEqual(1, goal.ProjectId);
            Assert.AreEqual(priority, goal.Priority);
            Assert.AreEqual(state, goal.State);
            Assert.IsFalse(goal.IsRemoved);
        }

        [Test]
        public async Task UpdateRangeTest()
        {
            //Arrange
            string updatedName = "UpdatedName";
            string updatedDescription = "UpdatedDescription";
            var vmGoals = _testData.Goals.Take(1).Select(t =>
            {
                t.Title = updatedName;
                t.Description = updatedDescription;
                return _vmConverter.ToViewModel(t);
            }).ToArray();
            var observerIds = _testData.Users.Skip(4).Take(6).Select(u => u.Id).ToList();
            var childGoalIds = _testData.Goals.Skip(6).Take(4).Select(g => g.Id).ToList();

            var vmGoalForms = vmGoals
                .Select(vm => new VmGoalForm(vm, observerIds, childGoalIds));

            //Act
            await _service.UpdateByFormRange(_currentUser, vmGoalForms);
            var goals = await _dataContext.Goals
                .Include(g => g.ChildGoals)
                .Include(g => g.Observers)
                .Where(g => vmGoals.Select(vm => vm.Id).Any(vmId => vmId == g.Id))
                .ToArrayAsync();

            //Assert
            foreach (var goal in goals)
            {
                Assert.AreEqual(updatedName, goal.Title);
                Assert.AreEqual(updatedDescription, goal.Description);
                Assert.AreEqual(observerIds.Count, goal.Observers.Count);
                Assert.AreEqual(childGoalIds.Count, goal.ChildGoals.Count);
            }
        }

        [Test]
        public async Task DeleteTest()
        {
            //Arrange
            var id = _testData.Goals.First().Id;

            //Act
            var goal = await _service.Delete(_currentUser, id);

            //Assert
            Assert.IsNotNull(goal);
            Assert.AreEqual(1, goal.Id);
            Assert.IsTrue(goal.IsRemoved);
        }

        [Test]
        public async Task DeleteRangeTest()
        {
            //Arrange
            var ids = new[] { 0, 1, 2 };

            //Act
            var goals = (await _service.DeleteRange(_currentUser, ids)).ToArray();

            //Assert
            Assert.IsNotNull(goals);
            Assert.AreEqual(2, goals.Length);
            Assert.AreEqual(1, goals[0].Id);
            Assert.AreEqual(2, goals[1].Id);
            Assert.IsTrue(goals[0].IsRemoved);
            Assert.IsTrue(goals[1].IsRemoved);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public async Task DeleteNotExistedTest(int id)
        {
            //Act
            var goal = await _service.Delete(_currentUser, id);

            //Assert
            Assert.IsNull(goal);
        }

        [TestCase(1)]
        [TestCase(10)]
        public async Task RestoreGoalTest(int goalId)
        {
            //Act
            var result = await _service.Restore(_currentUser, goalId);

            //Assert
            Assert.IsFalse(result.IsRemoved);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public async Task RestoreNotExistedGoalTest(int goalId)
        {
            //Arrange

            //Act
            var result = await _service.Restore(_currentUser, goalId);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task RestoreRangeTest()
        {
            //Arrange
            var ids = _testData.Goals.TakeLast(1).Select(g => g.Id).ToArray();

            //Act
            var goals = (await _service.RestoreRange(_currentUser, ids)).ToArray();

            //Assert
            Assert.IsNotNull(goals);
            Assert.AreEqual(1, goals.Length);
            Assert.AreEqual(ids[0], goals[0].Id);
            Assert.IsFalse(goals[0].IsRemoved);
        }

        [Test]
        public async Task CreationTimeTest()
        {
            //Arrange
            var vmGoal = new VmGoal
            {
                Title = "NewGoal",
                ProjectId = _testData.Projects.First().Id,
                CreationDate = DateTime.MinValue,
                IsRemoved = false
            };

            //Act
            var result = await _service.Create(_currentUser, vmGoal);
            var delta = Math.Abs((DateTime.Now - result.CreationDate).TotalSeconds);

            //Assert
            Assert.AreEqual(0, delta, 10);
        }


        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private ServiceProvider _serviceProvider;
        private GoalsService _service;
        private UsersService _usersService;
        private ApplicationUser _currentUser;
        private UserManager<ApplicationUser> _userManager;
        private VmGoalConverter _vmConverter;
    }
}