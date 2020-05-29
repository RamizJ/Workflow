using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using FizzWare.NBuilder;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using WorkflowService.Common;
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

        [TestCase(0, 10)]
        [TestCase(1, 3)]
        public async Task GetAllTest(int userIndex, int expectedScopesCount)
        {
            //Arrange

            //Act
            var scopes = (await _service.GetAll(_testData.Users[userIndex])).ToArray();

            //Assert
            Assert.AreEqual(expectedScopesCount, scopes.Length);
        }

        [TestCase(0, 12, 10)]
        [TestCase(0, 5, 5)]
        [TestCase(1, 5, 5)]
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

        [TestCase(0, 12, null, 10)]
        [TestCase(0, 5, "Scope1", 5)]
        [TestCase(1, 5, "Scope1", 1)]
        [TestCase(0, 5, "Group1", 3)]
        [TestCase(0, 5, "Group2", 5)]
        [TestCase(1, 5, "Group2", 2)]
        [TestCase(0, 5, "Team1", 3)]
        [TestCase(1, 5, "Team2", 2)]
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
        [TestCase(1, 5, null, "GroupName", "Group2", 2)]
        [TestCase(1, 5, null, "OwnerFio", "Firstname0", 5)]
        [TestCase(1, 5, null, "OwnerFio", "lastname0", 5)]
        [TestCase(1, 5, null, "OwnerFio", "middlename0", 5)]
        [TestCase(0, 5, null, "OwnerFio", "Firstname3", 0)]
        [TestCase(0, 3, "Team1", "OwnerFio", "Firstname1", 0)]
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
        [TestCase(0, 5, "", "Name", SortType.Descending, new[] { 10, 9, 8 })]
        [TestCase(0, 5, "Team1", "Name", SortType.Descending, new[] { 3, 2, 1 })]
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


        private SqliteConnection _dbConnection;
        private DataContext _dataContext;
        private TestData _testData;
        private IScopesService _service;
    }
}
