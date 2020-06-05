using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;
using WorkflowService.Common;
using WorkflowService.Services;
using WorkflowService.Services.Abstract;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class ProjectsServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _dbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            using var dataContext = ContextHelper.CreateContext(_dbConnection, false);
            var userManager = ContextHelper.CreateUserManager(dataContext);

            _testData = new TestData();
            _testData.Initialize(dataContext, userManager);

            _dataContext = ContextHelper.CreateContext(_dbConnection, true);
            _service = new ProjectsService(_dataContext);
            _vmConverter = new VmProjectConverter();
            _currentUser = _testData.Users.First();
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Dispose();
            _dbConnection.Close();
        }

        [TestCase(0, 0, 1)]
        [TestCase(2, 0, null)]
        [TestCase(1, 1, 2)]
        public async Task GetTest(int userIndex, int scopeIndex, int? expectedScopeId)
        {
            //Arrange

            //Act
            var vmScope = await _service.Get(_testData.Users[userIndex], _testData.Projects[scopeIndex].Id);

            //Assert
            Assert.AreEqual(expectedScopeId, vmScope?.Id);
            if (expectedScopeId != null)
            {
                Assert.AreEqual(_testData.Projects[scopeIndex].Team?.Name, vmScope?.TeamName);
                Assert.AreEqual(_testData.Projects[scopeIndex].Group?.Name, vmScope?.GroupName);
                Assert.AreEqual(_testData.Projects[scopeIndex].Owner?.Fio, vmScope?.OwnerFio);
            }
        }

        
        [TestCase(0, 12, 9)]
        [TestCase(0, 5, 5)]
        [TestCase(1, 5, 4)]
        [TestCase(2, 5, 0)]
        public async Task GetPageTest(int pageNumber, int pageSize, int expectedCount)
        {
            //Arrange

            //Act
            var resultScopes = (await _service.GetPage(_testData.Users.First(), 
                pageNumber, pageSize, 
                "", null, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }

        [TestCase(0, 12, null, 9)]
        [TestCase(0, 5, "Scope1", 5)]
        [TestCase(1, 5, "Scope1", 1)]
        [TestCase(0, 5, "Group1", 3)]
        [TestCase(0, 5, "Group2", 5)]
        [TestCase(1, 5, "Group2", 1)]
        [TestCase(1, 5, "Team12", 1)]
        [TestCase(0, 5, "Team11", 3)]
        public async Task GetPageFilterTest(int pageNumber, int pageSize, 
            string filter, int expectedCount)
        {
            //Arrange

            //Act
            var resultScopes = (await _service.GetPage(_testData.Users.First(), pageNumber, pageSize,
                filter, null, null)).ToArray(); 

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }

        [TestCase(0, 5, null, "Name", "scope1", 5)]
        [TestCase(0, 5, null, "GroupName", "Group2", 5)]
        [TestCase(1, 5, null, "GroupName", "Group2", 1)]
        [TestCase(1, 5, null, "OwnerFio", "Firstname1", 4)]
        [TestCase(1, 5, null, "OwnerFio", "lastname1", 4)]
        [TestCase(1, 5, null, "OwnerFio", "middlename1", 4)]
        [TestCase(0, 5, null, "OwnerFio", "Firstname3", 0)]
        [TestCase(0, 3, "Team1", "OwnerFio", "Firstname1", 3)]
        public async Task GetPageFilterFieldsTest(int pageNumber, int pageSize,
            string filter, string fieldName, object value, int expectedCount)
        {
            //Arrange

            //Act
            var filterField = new FieldFilter(fieldName, value);
            var resultScopes = (await _service.GetPage(_testData.Users.First(), pageNumber, pageSize,
                filter, new []{ filterField }, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }


        [TestCase(0, 5, "", "TeamName", SortType.Ascending, new[] { 1, 2, 3 })]
        [TestCase(0, 5, "", "Name", SortType.Descending, new[] { 9, 8, 7 })]
        [TestCase(0, 5, "Team11", "Name", SortType.Descending, new[] { 3, 2, 1 })]
        public async Task GetPageWithSortingTest(int pageNumber, int pageSize,
            string filter, string fieldName, SortType sortType, int[] expectedIds)
        {
            //Arrange

            //Act
            var sortField = new FieldSort(fieldName, sortType);
            var resultScopes = (await _service.GetPage(_testData.Users.First(), pageNumber, pageSize,
                filter, null, new[] { sortField })).ToArray() ;

            //Assert
            Assert.LessOrEqual(expectedIds.Length, resultScopes.Length);
            for (var i = 0; i < expectedIds.Length; i++)
            {
                Assert.AreEqual(expectedIds[i], resultScopes[i].Id);
            }
        }


        [TestCase(null)]
        [TestCase(new int[0])]
        public async Task GetRangeForNullInputTest(int[] ids)
        {
            //Act
            var resultScopes = await _service.GetRange(_testData.Users.First(), ids);

            //Assert
            Assert.IsNull(resultScopes);
        }

        
        [TestCase(new[]{1,2,3})]
        [TestCase(new[] { 9, 10 })]
        public async Task GetRangeTest(int[] ids)
        {
            //Arrange

            //Act
            var resultScopes = (await _service.GetRange(_testData.Users.First(), ids)).ToArray();

            //Assert
            Assert.AreEqual(ids.Length, resultScopes.Length);
            for (var i = 0; i < ids.Length; i++)
            {
                Assert.AreEqual(ids[i], resultScopes[i].Id);
            }
        }

        [Test]
        public void CreateForNullInputTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.Create(_testData.Users.First(), null));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public async Task CreateForNullInvalidNameTest(string name)
        {
            //Arrange
            var vmScope = new VmProject
            {
                Id = 0,
                Name = name,
                TeamId = null,
                GroupId = null,
                OwnerId = _testData.Users.First().Id,
                IsRemoved = false,
            };

            var result = await _service.Create(_testData.Users.First(), vmScope);

            //Assert
            Assert.IsFalse(result.Succeeded);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public async Task CreateTest(int id)
        {
            //Arrange
            var groupId = _testData.Groups.First().Id;
            var vmScope = new VmProject
            {
                Id = id,
                Name = "new project",
                TeamId = null,
                GroupId = groupId,
                OwnerId = _testData.Users[1].Id,
                IsRemoved = false,
            };
            var currentUser = _testData.Users.First();

            //Act
            var result = await _service.Create(currentUser, vmScope);

            //Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(_testData.Projects.Count + 1, result.Data.Id);
            Assert.AreEqual(currentUser.Id, result.Data.OwnerId);
            Assert.AreEqual(groupId, result.Data.GroupId);
        }

        [TestCase(1, null, "", false)]
        [TestCase(1, "", "", false)]
        [TestCase(1, "  ", "", false)]
        [TestCase(1, "New project", "New project description", true)]
        public async Task UpdateTest(int projectId, string name, string description, bool isSucceeded)
        {
            //Arrange
            var project = _testData.Projects.First();
            project.Id = projectId;
            project.Name = name;
            project.Description = description;
            var vmProject = _vmConverter.ToViewModel(project);

            //Act
            var result = await _service.Update(_currentUser, vmProject);

            //Assert
            Assert.AreEqual(isSucceeded, result.Succeeded);
            if (isSucceeded)
            {
                Assert.AreEqual(projectId, result.Data.Id);
                Assert.AreEqual(name, result.Data.Name);
                Assert.AreEqual(description, result.Data.Description);
            }
        }


        private SqliteConnection _dbConnection;
        private DataContext _dataContext;
        private TestData _testData;
        private IProjectsService _service;
        private VmProjectConverter _vmConverter;
        private ApplicationUser _currentUser;
    }
}
