using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class TeamUsersServiceTests
    {
        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private ServiceProvider _serviceProvider;
        private TeamUsersService _service;
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
            _service = new TeamUsersService(_dataContext, new VmUserConverter());
            _currentUser = _testData.Users.First();
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _serviceProvider.Dispose();
            _dbConnection.Close();
        }


        [TestCase(0, 12, 1, 6)]
        [TestCase(0, 5, 1, 5)]
        [TestCase(0, 5, 2, 3)]
        [TestCase(2, 5, 2, 0)]
        public async Task GetPageTest(int pageNumber, int pageSize, int teamId, int expectedCount)
        {
            //Arrange
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            //Act
            var users = (await _service.GetPage(_currentUser, teamId, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, users.Length);
        }


        [TestCase(0, 12, 1, null, 6)]
        [TestCase(0, 5, 1, "Lastname1", 5)]
        [TestCase(0, 5, 1, "firstname1", 5)]
        [TestCase(0, 5, 1, "middlename1", 5)]
        public async Task GetPageFilterTest(int pageNumber, int pageSize, 
            int teamId, string filter, int expectedCount)
        {
            //Arrange
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = filter
            };

            //Act
            var users = (await _service.GetPage(_currentUser, teamId, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, users.Length);
        }


        [Test]
        public async Task AddTest()
        {
            //Arrange
            var team = _testData.Teams.First();
            var expectedCount = team.TeamUsers.Count + 1;
            var user = _testData.Users.Last();

            //Act
            await _service.Add(team.Id, user.Id);

            //Assert
            Assert.AreEqual(expectedCount, _dataContext.TeamUsers.Count(tu => tu.TeamId == team.Id));
        }

        [Test]
        public async Task RemoveTest()
        {
            //Arrange
            var team = _testData.Teams.First();
            var expectedCount = team.TeamUsers.Count - 1;
            var user = _testData.Users.First();

            //Act
            await _service.Remove(team.Id, user.Id);

            //Asserts
            Assert.AreEqual(expectedCount, _dataContext.TeamUsers.Count(tu => tu.TeamId == team.Id));
        }


        [Test]
        public void RemoveNotExistedTest()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.Remove(0, string.Empty));
        }
    }
}