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
            var scopes = Builder<Scope>.CreateListOfSize(10)
                .All()
                .With(s => s.OwnerId = _testData.Users.First().Id)
                .With(s => s.TeamId = null)
                .With(s => s.GroupId = null)
                .Build();
            _dataContext.Scopes.BatchDelete();
            _dataContext.Scopes.AddRange(scopes);
            _dataContext.SaveChanges();

            //Act
            var resultScopes = (await _service.GetPage(_testData.Users.First(), pageNumber, pageSize, 
                "", null, SortType.Unspecified, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }

        [TestCase(0, 12, null, null, 10)]
        [TestCase(0, 5, "Scope1", null, 5)]
        [TestCase(1, 5, "Scope1", null, 1)]
        [TestCase(1, 5, "Group1", null, 2)]
        public async Task GetPageFilterTest(int pageNumber, int pageSize, 
            string filter, string[] filterFields, int expectedCount)
        {
            //Arrange
            var context = ContextHelper.CreateContext(_dbConnection, false);
            var teams = Builder<Group>.CreateListOfSize(2)
                .All()
                .With(x => x.ParentGroupId = null)
                .Build();
            var groups = Builder<Group>.CreateListOfSize(2)
                .All().With((g,i) => g.Name = $"Group{i}").Build();
            var scopes = Builder<Scope>.CreateListOfSize(10)
                .All()
                .With(s => s.OwnerId = _testData.Users.First().Id)
                .With(s => s.TeamId = null)
                .TheFirst(6).With((s, i) => s.Name = $"Scope1{i}")
                .TheNext(4).With((s, i) => s.Name = $"Scope2{i}")
                .TheFirst(3).With(s => s.GroupId = groups[0].Id)
                .TheNext(7).With(s => s.GroupId = groups[1].Id)
                .Build();
            context.Scopes.BatchDelete();
            context.Groups.AddRange(groups);
            context.Scopes.AddRange(scopes);
            context.SaveChanges();

            //Act
            var resultScopes = (await _service.GetPage(_testData.Users.First(), pageNumber, pageSize,
                filter, filterFields, SortType.Unspecified, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }



        private SqliteConnection _dbConnection;
        private DataContext _dataContext;
        private TestData _testData;
        private IScopesService _service;
    }
}
