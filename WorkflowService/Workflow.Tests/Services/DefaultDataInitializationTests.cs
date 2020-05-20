using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using WorkflowService.Services;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class DefaultDataInitializationTests
    {
        [SetUp]
        public void SetUp()
        {
            _dbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            _dataContext = ContextHelper.CreateContext(_dbConnection, true);
            _userManager = ContextHelper.CreateUserManager(_dataContext);
            _roleManager = ContextHelper.CreateRoleManager(_dataContext);
            _service = new DefaultDataInitializationService(_userManager, _roleManager);

            _dataContext.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Dispose();
            _dbConnection.Close();
        }


        [Test]
        public async Task InitializeDefaultDataTest()
        {
            //Act
            await _service.InitializeRoles();
            await _service.InitializeAdmin();
            var admin = _dataContext.Users.First();

            //Assert
            Assert.AreEqual(RoleNames.GetAllRoleNames().Count(), _dataContext.Roles.Count());
            Assert.AreEqual(1, _dataContext.Users.Count());
            Assert.AreEqual("admin@admin.ru", admin.UserName);
        }


        private SqliteConnection _dbConnection;
        private DataContext _dataContext;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private DefaultDataInitializationService _service;
    }
}
