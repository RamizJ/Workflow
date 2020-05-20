using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Workflow.DAL;
using WorkflowService.Services;
using WorkflowService.Services.Abstract;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class ScopesServiceTests
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
            _service = new ScopesService(_dataContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Dispose();
            _dbConnection.Close();
        }

        [TestCase(0, 0, 1)]
        [TestCase(1, 0, null)]
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

        [TestCase(0, 2)]
        [TestCase(1, 1)]
        public async Task GetScopesTest(int userIndex, int expectedScopesCount)
        {
            //Arrange

            //Act
            var scopes = (await _service.GetScopes(_testData.Users[userIndex])).ToArray();

            //Assert
            Assert.AreEqual(expectedScopesCount, scopes.Length);
        }


        private SqliteConnection _dbConnection;
        private DataContext _dataContext;
        private TestData _testData;
        private IScopesService _service;
    }
}
