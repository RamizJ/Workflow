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
        public void CreateByFormForInvalidNameTest(string name)
        {
            //Arrange
            var vmProject = new VmProject
            {
                Id = 0,
                Name = name,
                GroupId = null,
                OwnerId = _testData.Users.First().Id,
                IsRemoved = false,
            };

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await _service.CreateByForm(_testData.Users.First(), new VmProjectForm(vmProject, null)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void CreateForInvalidNameTest(string name)
        {
            //Arrange
            var vmProject = new VmProject
            {
                Id = 0,
                Name = name,
                GroupId = null,
                OwnerId = _testData.Users.First().Id,
                IsRemoved = false,
            };

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.CreateByForm(_testData.Users.First(), new VmProjectForm(vmProject, null)));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public async Task CreateTest(int id)
        {
            //Arrange
            var groupId = _testData.Groups.First().Id;
            var vmProject = new VmProject
            {
                Id = id,
                Name = "new project",
                GroupId = groupId,
                OwnerId = _testData.Users[1].Id,
                IsRemoved = false,
            };
            var currentUser = _testData.Users.First();

            //Act
            var result = await _service.CreateByForm(currentUser, new VmProjectForm(vmProject, null));

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testData.Projects.Count + 1, result.Id);
            Assert.AreEqual(currentUser.Id, result.OwnerId);
            Assert.AreEqual(groupId, result.GroupId);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public async Task CreateByFormTest(int id)
        {
            //Arrange
            var groupId = _testData.Groups.First().Id;
            var vmProject = new VmProject
            {
                Id = id,
                Name = "new project",
                GroupId = groupId,
                OwnerId = _testData.Users[1].Id,
                IsRemoved = false,
            };
            var currentUser = _testData.Users.First();

            //Act
            var result = await _service.CreateByForm(currentUser, new VmProjectForm(vmProject, null));

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testData.Projects.Count + 1, result.Id);
            Assert.AreEqual(currentUser.Id, result.OwnerId);
            Assert.AreEqual(groupId, result.GroupId);
        }

        [Test]
        public async Task CreateByFormWithTeamsTest()
        {
            //Arrange
            var teamIds = new[] {1, 2, 3};
            var vmProject = new VmProject
            {
                Id = 0,
                Name = "new project",
                OwnerId = _testData.Users[1].Id,
                IsRemoved = false
            };
            var currentUser = _testData.Users.First();

            //Act
            var result = await _service.CreateByForm(currentUser, new VmProjectForm(vmProject, teamIds));
            var projectTeams = await _dataContext.ProjectTeams
                .Where(pt => pt.ProjectId == result.Id)
                .ToArrayAsync();

            //Assert
            Assert.AreEqual(teamIds.Length, projectTeams.Length);
            for (int i = 0; i < teamIds.Length; i++) 
                Assert.AreEqual(teamIds[i], projectTeams[i].TeamId);
        }

        [Test]
        public async Task UpdateByFormTest()
        {
            //Arrange
            var project = _testData.Projects.First();
            project.Id = 1;
            project.Name = "New project";
            project.Description = "New project description";
            var vmProject = _vmConverter.ToViewModel(project);

            //Act
            await _service.UpdateByForm(_currentUser, new VmProjectForm(vmProject, null));
            var result = _dataContext.Projects.First(x => x.Id == 1);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(project.Id, result.Id);
            Assert.AreEqual(project.Name, result.Name);
            Assert.AreEqual(project.Description, result.Description);
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

        [Test]
        public async Task UpdateByFormWithTeamsTest()
        {
            //Arrange
            var project = _testData.Projects.First();
            project.Id = 1;
            project.Name = "New project";
            project.Description = "New project description";
            var vmProject = _vmConverter.ToViewModel(project);
            var oldProjectTeams = new[]
            {
                new ProjectTeam(1, 1),
                new ProjectTeam(1, 2),
                new ProjectTeam(1, 3)
            };
            var newProjectTeams = new[]
            {
                new ProjectTeam(1, 5),
                new ProjectTeam(1, 6),
                new ProjectTeam(1, 7)
            };

            await using var dataContext = ContextHelper.CreateContext(_dbConnection, false);
            dataContext.ProjectTeams.RemoveRange(_dataContext.ProjectTeams);
            await dataContext.ProjectTeams.AddRangeAsync(oldProjectTeams);
            await dataContext.SaveChangesAsync();

            //Act
            await _service.UpdateByForm(_currentUser, new VmProjectForm(vmProject,
                newProjectTeams.Select(pt => pt.TeamId)));
            var resultProjectTeams = await _dataContext.ProjectTeams
                .Where(pt => pt.ProjectId == 1)
                .OrderBy(pt => pt.TeamId)
                .ToArrayAsync();

            //Assert
            Assert.AreEqual(newProjectTeams.Length, resultProjectTeams.Length);
            for (int i = 0; i < newProjectTeams.Length; i++)
                Assert.AreEqual(newProjectTeams[i].TeamId, resultProjectTeams[i].TeamId);
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

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void UpdateByFormInvalidNameTest(string name)
        {
            //Arrange
            var project = _testData.Projects.First();
            project.Id = 1;
            project.Name = name;
            project.Description = "New project description";
            var vmProject = _vmConverter.ToViewModel(project);

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.UpdateByForm(_currentUser, new VmProjectForm(vmProject, null)));
        }

        [Test]
        public async Task UpdateRangeTest()
        {
            //Arrange
            string updatedName = "UpdatedName";
            string updatedDescription = "UpdatedDescription";
            var vmProjects = _testData.Projects.Select(p =>
            {
                p.Name = updatedName;
                p.Description = updatedDescription;
                return _vmConverter.ToViewModel(p);
            }).ToArray();
            var vmProjectForms = vmProjects
                .Select(vmP => new VmProjectForm(vmP, new [] {5,6}));

            //Act
            await _service.UpdateByFormRange(_currentUser, vmProjectForms);
            var projects = await _dataContext.Projects.Include(p => p.ProjectTeams).ToArrayAsync();

            //Assert
            foreach (var project in projects)
            {
                Assert.AreEqual(updatedName, project.Name);
                Assert.AreEqual(updatedDescription, project.Description);
                Assert.AreEqual(2, project.ProjectTeams.Count);
                Assert.AreEqual(5, project.ProjectTeams[0].TeamId);
                Assert.AreEqual(6, project.ProjectTeams[1].TeamId);
            }
        }

        [Test]
        public async Task DeleteTest()
        {
            //Arrange
            int id = 1;

            //Act
            await _service.Delete(_currentUser, id);
            var resultProject = await _dataContext.Projects
                .SingleOrDefaultAsync(p => p.Id == id);

            //Assert
            Assert.IsTrue(resultProject.IsRemoved);
        }

        [Test]
        public async Task RestoreTest()
        {
            //Arrange
            var project = _testData.Projects.First(p => p.IsRemoved);

            //Act
            await _service.Restore(_currentUser, project.Id);
            var resultProject = await _dataContext.Projects
                .SingleOrDefaultAsync(p => p.Id == project.Id);

            //Assert
            Assert.IsFalse(resultProject.IsRemoved);
        }

        [Test]
        public async Task DeleteRangeTest()
        {
            //Arrange
            var ids = new[] {1,2,3};

            //Act
            await _service.DeleteRange(_currentUser, ids);
            var resultProjects = await _dataContext.Projects
                .Where(p => ids.Any(pId => p.Id == pId))
                .ToArrayAsync();

            //Assert
            Assert.AreEqual(ids.Length, resultProjects.Length);
            foreach (var resultProject in resultProjects) 
                Assert.IsTrue(resultProject.IsRemoved);
        }

        [Test]
        public async Task RestoreRangeTest()
        {
            //Arrange
            var ids = new[] { 10 };

            //Act
            await _service.RestoreRange(_currentUser, ids);
            var resultProjects = await _dataContext.Projects
                .Where(p => ids.Any(pId => p.Id == pId))
                .ToArrayAsync();

            //Assert
            Assert.AreEqual(ids.Length, resultProjects.Length);
            foreach (var resultProject in resultProjects)
                Assert.IsFalse(resultProject.IsRemoved);
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
