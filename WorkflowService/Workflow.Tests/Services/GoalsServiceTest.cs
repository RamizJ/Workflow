using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
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
            _dataContext = _serviceProvider.GetRequiredService<DataContext>();
            _service = _serviceProvider.GetRequiredService<IGoalsService>();
            _usersService = _serviceProvider.GetRequiredService<IUsersService>();
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

        [TestCase(1, true)]
        [TestCase(2, false)]
        [TestCase(3, false)]
        public async Task GetTest(int goalId, bool isChildsExists)
        {
            //Arrange
            var expectedGoal = _testData.Goals.First(t => t.Id == goalId);

            //Act
            var vmGoal = await _service.Get(_currentUser, goalId);

            //Assert
            Assert.AreEqual(expectedGoal.Id, vmGoal.Id);
            Assert.AreEqual(expectedGoal.Title, vmGoal.Title);
            Assert.AreEqual(expectedGoal.Description, vmGoal.Description);
            Assert.AreEqual(isChildsExists, vmGoal.HasChildren);
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

        [TestCase(0, 12, 1, 6)]
        [TestCase(0, 5, 1, 5)]
        [TestCase(0, 5, 10, 3)]
        [TestCase(0, 5, 2, 0)]
        [TestCase(1, 5, 1, 1)]
        [TestCase(2, 5, 1, 0)]
        public async Task GetPageTest(int pageNumber, int pageSize, int projectId, int expectedCount)
        {
            //Arrange
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            //Act
            var goals = (await _service.GetPage(_currentUser, projectId, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, goals.Length);
        }

        [TestCase(0, 12, 1, null, 6)]
        [TestCase(0, 12, 10, null, 3)]
        public async Task GetPageFilterTest(int pageNumber, int pageSize, 
            int? projectId, string filter, int expectedCount)
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
            var result = (await _service.GetPage(_currentUser, projectId, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, result.Length);
        }

        [TestCase(0, 5, 1, "Title", new object[] { "Goal1" }, false, 5)]
        [TestCase(0, 5, 1, "Description", new object[] { "Description1" }, false, 1)]
        [TestCase(0, 5, 10, "Title", new object[] { "Goal2" }, true, 4)]
        [TestCase(0, 5, 10, "State", new object[] { GoalState.Perform }, true, 4)]
        [TestCase(0, 5, 10, "Priority", new object[] { GoalPriority.High }, true, 4)]
        public async Task GetPageFilterFieldsTest(int pageNumber, int pageSize,
            int projectId, string fieldName, object[] values, 
            bool withRemoved, int expectedCount)
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
                IsRemoved = false,
                MetadataList = new List<VmMetadata>
                {
                    new("Key1", "Val1"),
                    new("Key2", "Val2")
                }
            };

            //Act
            var result = await _service.Create(_currentUser, vmGoal);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testData.Goals.Count + 1, result.Id);
            Assert.AreEqual(vmGoal.Title, result.Title);
            Assert.AreEqual(vmGoal.MetadataList.Count, result.MetadataList.Count);
            Assert.AreEqual(vmGoal.MetadataList[0].Key, result.MetadataList[0].Key);
            Assert.AreEqual(vmGoal.MetadataList[0].Value, result.MetadataList[0].Value);
            Assert.AreEqual(vmGoal.MetadataList[1].Key, result.MetadataList[1].Key);
            Assert.AreEqual(vmGoal.MetadataList[1].Value, result.MetadataList[1].Value);
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
            Assert.ThrowsAsync<HttpResponseException>(async () => await _service.Create(_currentUser, vmGoal));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public async Task CreateHierarchyTest(int id)
        {
            //Arrange
            var vmGoal = new VmGoal
            {
                Id = id,
                Title = "Goal3",
                ProjectId = _testData.Projects.First().Id,
                IsRemoved = false,

                Children = new List<VmGoal>
                {
                    new()
                    {
                        Id = id,
                        Title = "Goal31",
                        ProjectId = _testData.Projects.First().Id,
                        IsRemoved = false
                    },
                    _vmConverter.ToViewModel(_testData.Goals[15])
                }
            };

            //Act
            var result = await _service.Create(_currentUser, vmGoal);
            int childsCount = await _dataContext.Goals.CountAsync(g => g.ParentGoalId == result.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testData.Goals.Count + 1, result.Id);
            Assert.AreEqual(vmGoal.Title, result.Title);
            Assert.IsTrue(result.HasChildren);
            Assert.AreEqual(2, childsCount);
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void UpdateInvalidTitleTest(string title)
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
            //Assert
            Assert.ThrowsAsync<HttpResponseException>(async () => await _service.Update(_currentUser, vmGoal));
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
            Assert.AreEqual(projectId, goal.ProjectId);
            Assert.AreEqual(priority, goal.Priority);
            Assert.AreEqual(state, goal.State);
            Assert.AreEqual(0, (goal.CreationDate - DateTime.Now).TotalMinutes, 1);
            Assert.IsFalse(goal.IsRemoved);
        }

        [Test]
        public async Task UpdateRangeTest()
        {
            //Arrange
            string updatedName = "UpdatedName";
            string updatedDescription = "UpdatedDescription";
            var vmGoal = _vmConverter.ToViewModel(_testData.Goals.First());
            vmGoal.Title = updatedName;
            vmGoal.Description = updatedDescription;
            vmGoal.ObserverIds = _testData.Users.Skip(4).Take(6).Select(u => u.Id).ToList();
            vmGoal.MetadataList = new List<VmMetadata>
            {
                new("UpdKey1", "UpdVal1"),
                new("UpdKey2", "UpdVal2"),
                new("UpdKey3", "UpdVal3")
            };

            //Act
            await _service.UpdateRange(_currentUser, new[] {vmGoal});
            var metadataCount = await _dataContext.Metadata
                .AsNoTracking()
                .CountAsync(x => x.GoalId == vmGoal.Id);
            var result = await _dataContext.Goals
                .Include(x => x.MetadataList)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == vmGoal.Id);
                

            //Assert
            Assert.AreEqual(updatedName, result.Title);
            Assert.AreEqual(updatedDescription, result.Description);
            Assert.AreEqual(3, metadataCount);
            Assert.AreEqual(3, result.MetadataList.Count);
        }

        [Test]
        public async Task DeleteTest()
        {
            //Arrange
            var id = _testData.Goals.First().Id;

            //Act
            var vmGoal = await _service.Delete(_currentUser, id);
            var childGoals = await _dataContext.Goals
                .Where(g => g.ParentGoalId == id)
                .ToArrayAsync();

            //Assert
            Assert.IsNotNull(vmGoal);
            Assert.AreEqual(1, vmGoal.Id);
            Assert.IsTrue(vmGoal.IsRemoved);

            foreach (var childGoal in childGoals) 
                Assert.IsTrue(childGoal.IsRemoved);
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
            var delta = Math.Abs((DateTime.Now.ToUniversalTime() - result.CreationDate).TotalSeconds);

            //Assert
            Assert.AreEqual(0, delta, 10);
        }

        [Test]
        public async Task HasChildrenTest()
        {
            //Arrange
            var serviceProvider = ContextHelper.Initialize(_dbConnection, false);
            var context = serviceProvider.GetRequiredService<DataContext>();

            var firstGoal = _testData.Goals.First();
            var lastGoal = _testData.Goals.Last();

            lastGoal.ParentGoalId = firstGoal.Id;

            context.Update(lastGoal);
            await context.SaveChangesAsync();

            //Act
            var goal = await _service.Get(_currentUser, firstGoal.Id);

            //Assert
            Assert.IsTrue(goal.HasChildren);
        }

        [Test]
        public async Task AddChildGoalTest()
        {
            //Arrange
            var parentGoal = _testData.Goals.First();
            var childGoal = _testData.Goals.Last();

            //Act
            await _service.AddChildGoals(_currentUser, parentGoal.Id, new []{ childGoal.Id });
            var resultChildGoal = _dataContext.Goals
                .First(x => x.Id == childGoal.Id);

            //Assert
            Assert.AreEqual(parentGoal.Id, resultChildGoal.ParentGoalId);
            Assert.AreEqual(parentGoal.ProjectId, resultChildGoal.ProjectId);
        }

        [TestCase(14, 1)]
        [TestCase(18, 10)]
        public async Task GetParentGoalTest(int goalId, int expectedParentId)
        {
            //Arrange

            //Act
            var parentGoal = await _service.GetParentGoal(_currentUser, goalId);

            //Assert
            Assert.AreEqual(expectedParentId, parentGoal?.Id);
        }


        [TestCase(0, 12, 1, 6)]
        [TestCase(0, 5, 1, 5)]
        [TestCase(0, 5, 10, 4)]
        [TestCase(0, 5, 2, 0)]
        [TestCase(1, 5, 1, 1)]
        [TestCase(2, 5, 1, 0)]
        public async Task GetChildsPageTest(int pageNumber, int pageSize, int parentGoalId, int expectedCount)
        {
            //Arrange
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            //Act
            var goals = (await _service.GetChildsPage(_currentUser, parentGoalId, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, goals.Length);
        }

        [Test]
        public async Task ChangeStatesTest()
        {
            //Arrange
            var goalState = new VmGoalState
            {
                GoalId = 1,
                GoalState = GoalState.Perform,
                Comment = "State changed comment"
            };

            //Act
            await _service.ChangeStates(_currentUser, new[] {goalState});
            var goal = await _dataContext.Goals
                .Include(g => g.Messages)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == goalState.GoalId);
            var lastMessage = goal.Messages.Last();

            //Assert
            Assert.AreEqual(goalState.GoalState, goal.State);
            Assert.AreEqual(_currentUser.Id, lastMessage.OwnerId);
            Assert.IsTrue(lastMessage.Text.Contains(goalState.Comment));
        }


        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private ServiceProvider _serviceProvider;
        private IGoalsService _service;
        private IUsersService _usersService;
        private ApplicationUser _currentUser;
        private VmGoalConverter _vmConverter;
    }
}