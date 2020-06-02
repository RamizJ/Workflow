using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Workflow.DAL;
using WorkflowService.Services;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class UsersServiceTests
    {
        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private UsersService _service;

        [SetUp]
        public void Setup()
        {
            _dbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            using var dataContext = ContextHelper.CreateContext(_dbConnection, false);
            var userManager = ContextHelper.CreateUserManager(dataContext);

            _testData = new TestData();
            _testData.Initialize(dataContext, userManager);

            _dataContext = ContextHelper.CreateContext(_dbConnection, true);
            _service = new UsersService(_dataContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Dispose();
            _dbConnection.Close();
        }


        [Test]
        public async Task GetUserTest()
        {
            //Arrange
            var currentUser = _testData.Users.First();
            var expectedUser = _testData.Users[1];

            //Act
            var resultUser = await _service.Get(currentUser, expectedUser.Id);

            //Assert
            Assert.AreEqual(expectedUser.Id, resultUser.Id);
            Assert.AreEqual(expectedUser.LastName, resultUser.LastName);
        }
    }
}