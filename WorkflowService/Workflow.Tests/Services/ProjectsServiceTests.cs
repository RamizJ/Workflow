using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services;
using Workflow.Services.Abstract;
using Workflow.Services.Common;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class ProjectsServiceTests
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
            _service = new ProjectsService(_dataContext, _userManager);
            _currentUser = _testData.Users.First();
            _vmConverter = new VmProjectConverter();
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _serviceProvider.Dispose();
            _dbConnection.Close();
        }

        [TestCase(0, 0, 1)]
        [TestCase(2, 0, 1)]
        [TestCase(1, 1, 2)]
        public async Task GetTest(int userIndex, int projectIndex, int? expectedProjectId)
        {
            //Arrange
            var currentUser = _testData.Users[userIndex];
            var project = _testData.Projects[projectIndex];

            //Act
            var resultProject = await _service.Get(currentUser, project.Id);

            //Assert
            Assert.AreEqual(expectedProjectId, resultProject?.Id);
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

        [TestCase(0, 5, null, "Name", new object[] { "scope1" }, 5)]
        [TestCase(0, 5, null, "GroupName", new object[] { "Group2" }, 5)]
        [TestCase(1, 5, null, "GroupName", new object[] { "Group2" }, 1)]
        [TestCase(1, 5, null, "OwnerFio", new object[] { "Firstname1" }, 4)]
        [TestCase(1, 5, null, "OwnerFio", new object[] { "lastname1" }, 4)]
        [TestCase(1, 5, null, "OwnerFio", new object[] { "middlename1" }, 4)]
        [TestCase(0, 5, null, "OwnerFio", new object[] { "Firstname3" }, 0)]
        public async Task GetPageFilterFieldsTest(int pageNumber, int pageSize,
            string filter, string fieldName, object[] values, int expectedCount)
        {
            //Arrange

            //Act
            var filterField = new FieldFilter(fieldName, values);
            var projects = (await _service.GetPage(_testData.Users.First(), pageNumber, pageSize,
                filter, new []{ filterField }, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, projects.Length);
        }

        [TestCase(0, 5, "", "Name", SortType.Descending, new[] { 9, 8, 7 })]
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
        public void CreateForNullInvalidNameTest(string name)
        {
            //Arrange
            var vmScope = new VmProject
            {
                Id = 0,
                Name = name,
                GroupId = null,
                OwnerId = _testData.Users.First().Id,
                IsRemoved = false,
            };

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.Create(_testData.Users.First(), vmScope));
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
                GroupId = groupId,
                OwnerId = _testData.Users[1].Id,
                IsRemoved = false,
            };
            var currentUser = _testData.Users.First();

            //Act
            var result = await _service.Create(currentUser, vmScope);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testData.Projects.Count + 1, result.Id);
            Assert.AreEqual(currentUser.Id, result.OwnerId);
            Assert.AreEqual(groupId, result.GroupId);
        }

        [Test]
        public async Task UpdateTest()
        {
            //Arrange
            var project = _testData.Projects.First();
            project.Id = 1;
            project.Name = "New project";
            project.Description = "New project description";
            var vmProject = _vmConverter.ToViewModel(project);

            //Act
            await _service.Update(_currentUser, vmProject);
            var result = _dataContext.Projects.First(x => x.Id == 1);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(project.Id, result.Id);
            Assert.AreEqual(project.Name, result.Name);
            Assert.AreEqual(project.Description, result.Description);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void UpdateInvalidNameTest(string name)
        {
            //Arrange
            var project = _testData.Projects.First();
            project.Id = 1;
            project.Name = name;
            project.Description = "New project description";
            var vmProject = _vmConverter.ToViewModel(project);

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await _service.Update(_currentUser, vmProject));
        }


        private SqliteConnection _dbConnection;
        private DataContext _dataContext;
        private TestData _testData;
        private IProjectsService _service;
        private VmProjectConverter _vmConverter;
        private ApplicationUser _currentUser;
        private ServiceProvider _serviceProvider;
        private UserManager<ApplicationUser> _userManager;
    }
}
