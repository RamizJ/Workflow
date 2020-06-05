using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services;
using Workflow.Services.Common;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class TeamsServiceTests
    {
        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private ServiceProvider _serviceProvider;
        private TeamsService _service;
        private ApplicationUser _currentUser;
        private VmTeamConverter _vmConverter;

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
            _service = new TeamsService(_dataContext);
            _currentUser = _testData.Users.First();
            _vmConverter = new VmTeamConverter();
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
        public async Task GetTest(int teamId)
        {
            //Arrange
            var expectedTeam = _testData.Teams.First(t => t.Id == teamId);

            //Act
            var team = await _service.Get(_currentUser, teamId);

            //Assert
            Assert.AreEqual(expectedTeam.Id, team.Id);
            Assert.AreEqual(expectedTeam.Name, team.Name);
            Assert.AreEqual(expectedTeam.Description, team.Description);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1000)]
        public async Task GetNotExistedTest(int teamId)
        {
            //Arrange

            //Act
            var team = await _service.Get(_currentUser, teamId);

            //Assert
            Assert.IsNull(team);
        }

        [TestCase(0, 12, 9)]
        [TestCase(0, 5, 5)]
        [TestCase(1, 5, 4)]
        [TestCase(2, 5, 0)]
        public async Task GetPageTest(int pageNumber, int pageSize, int expectedCount)
        {
            //Arrange

            //Act
            var teams = (await _service.GetPage(_testData.Users.First(),
                pageNumber, pageSize,
                "", null, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, teams.Length);
        }

        [TestCase(0, 12, null, 9)]
        [TestCase(0, 5, "Team1", 5)]
        [TestCase(1, 5, "Team1", 1)]
        [TestCase(0, 5, "Team2", 3)]
        [TestCase(1, 5, "Team2", 0)]
        [TestCase(0, 5, "DEscription", 5)]
        [TestCase(1, 5, "DEscription", 4)]
        [TestCase(0, 5, "DEscription1", 1)]
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

        [TestCase(0, 5, null, "Name", "Team1", false, 5)]
        [TestCase(1, 5, null, "Name", "Team1", false, 1)]
        [TestCase(0, 5, null, "Name", "Team2", false, 3)]
        [TestCase(0, 5, null, "Description", "descriptioN1", false, 1)]
        [TestCase(0, 5, null, "isRemoved", true, true, 1)]
        public async Task GetPageFilterFieldsTest(int pageNumber, int pageSize,
            string filter, string fieldName, object value, bool withRemoved, int expectedCount)
        {
            //Arrange

            //Act
            var filterField = new FieldFilter(fieldName, value);
            var resultScopes = (await _service.GetPage(_testData.Users.First(),
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
            var vmTeam = new VmTeam
            {
                Id = 0,
                Name = name,
                GroupId = null,
                IsRemoved = false
            };

            //Act
            var result = await _service.Create(_testData.Users.First(), vmTeam);

            //Assert
            Assert.IsFalse(result.Succeeded);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public async Task CreateTest(int id)
        {
            //Arrange
            var vmTeam = new VmTeam
            {
                Id = id,
                Name = "Team",
                GroupId = null,
                IsRemoved = false
            };
            var currentUser = _testData.Users.First();

            //Act
            var result = await _service.Create(currentUser, vmTeam);

            //Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(_testData.Projects.Count + 1, result.Data.Id);
            Assert.AreEqual("Team", result.Data.Name);
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public async Task UpdateForNullInvalidNameTest(string name)
        {
            //Arrange
            var team = _testData.Teams.First();
            team.Name = name;
            var vmTeam = _vmConverter.ToViewModel(team);

            var result = await _service.Create(_testData.Users.First(), vmTeam);

            Assert.IsFalse(result.Succeeded);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public async Task UpdateForNotExistedTest(int id)
        {
            //Arrange
            var team = _testData.Teams.First();
            team.Id = id;
            var vmTeam = _vmConverter.ToViewModel(team);

            var result = await _service.Update(_testData.Users.First(), vmTeam);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestCase("TeamNew1", "DescriptionNew1")]
        public async Task UpdateTest(string name, string description)
        {
            //Arrange
            var team = _testData.Teams.First();
            team.Name = name;
            team.Description = description;
            var vmTeam = _vmConverter.ToViewModel(team);

            //Act
            var result = await _service.Update(_currentUser, vmTeam);

            //Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(name, result.Data.Name);
            Assert.AreEqual(description, result.Data.Description);
        }


        [TestCase(-1, false)]
        [TestCase(0, false)]
        [TestCase(1, true)]
        public async Task DeleteTest(int teamId, bool isSucceed)
        {
            //Arrange

            //Act
            var result = await _service.Delete(_currentUser, teamId);

            //Assert
            Assert.AreEqual(result.Succeeded, isSucceed);
            if(isSucceed)
                Assert.AreEqual(teamId, result.Data.Id);
        }
    }
}