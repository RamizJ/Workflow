using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class GoalAttachmentsServiceTest
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
            _service = new GoalAttachmentsService(_dataContext, _userManager, new FileService(_dataContext));
            _currentUser = _testData.Users.First();
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _serviceProvider.Dispose();
            _dbConnection.Close();
        }

        [Test]
        public void GetAttachmentsTest()
        {
            //Arrange
            ContextHelper.CreateContext(_dbConnection, false);


        }


        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private ServiceProvider _serviceProvider;
        private GoalAttachmentsService _service;
        private ApplicationUser _currentUser;
        private UserManager<ApplicationUser> _userManager;
    }
}