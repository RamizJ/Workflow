using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.DAL.Repositories.Abstract;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class ProjectWorkloadStatisticTests
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
            _serviceProvider.GetService<IGoalsRepository>();
            _service = _serviceProvider.GetRequiredService<IWorkloadForProjectStatisticService>();
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
        public async Task GetStatisticForProject()
        {
            //Arrange
            var options = new StatisticOptions
            {
                DateBegin = DateTime.Now - TimeSpan.FromDays(1),
                DateEnd = DateTime.Now,
                UserIds = new List<string> { _testData.Users.First().Id },
                ProjectIds = new List<int> { _testData.Projects.First().Id }
            };

            //Act
            var result = await _service.GetWorkloadByProject(_currentUser, options);

            //Assert
            Assert.NotNull(result);
        }


        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private ServiceProvider _serviceProvider;
        private IWorkloadForProjectStatisticService _service;
        private ApplicationUser _currentUser;
    }
}