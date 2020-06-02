using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Workflow.DAL;
using WorkflowService.Services;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class GoalsServiceTest
    {
        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private GoalsService _service;

        [SetUp]
        public void Setup()
        {
            _dbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            using var dataContext = ContextHelper.CreateContext(_dbConnection, false);
            var userManager = ContextHelper.CreateUserManager(dataContext);

            _testData = new TestData();
            _testData.Initialize(dataContext, userManager);

            _dataContext = ContextHelper.CreateContext(_dbConnection, true);
            _service = new GoalsService();
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
        public async Task GetScopeTest(int userIndex, int scopeIndex, int? expectedScopeId)
        {
            //Arrange

            //Act
            var vmScope = await _service.GetScope(_testData.Users[userIndex], _testData.Scopes[scopeIndex].Id);

            //Assert
            Assert.AreEqual(expectedScopeId, vmScope?.Id);
            if (expectedScopeId != null)
            {
                Assert.AreEqual(_testData.Scopes[scopeIndex].Team?.Name, vmScope?.TeamName);
                Assert.AreEqual(_testData.Scopes[scopeIndex].Group?.Name, vmScope?.GroupName);
                Assert.AreEqual(_testData.Scopes[scopeIndex].Owner?.Fio, vmScope?.OwnerFio);
            }
        }
    }
}