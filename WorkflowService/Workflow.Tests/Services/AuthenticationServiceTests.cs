using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services;
using Workflow.Services.Exceptions;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;

namespace Workflow.Tests.Services
{
    public class AuthenticationServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _dbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            using var dataContext = ContextHelper.CreateContext(_dbConnection, false);
            dataContext.Database.EnsureCreated();

            var configMock = new Mock<IConfiguration>();
            configMock.SetupGet(conf => conf[It.Is<string>(s => s == "Jwt:SigningKey")]).Returns("E7605B2B-0D85-4575-B36C-CF3E6F1BC7D1");
            configMock.SetupGet(conf => conf[It.Is<string>(s => s == "Jwt:Issuer")]).Returns("WorkflowServer");
            configMock.SetupGet(conf => conf[It.Is<string>(s => s == "Jwt:Audience")]).Returns("WorkflowClient");
            configMock.SetupGet(conf => conf[It.Is<string>(s => s == "Jwt:ExpiryInHours")]).Returns("1");

            _dataContext = ContextHelper.CreateContext(_dbConnection, true);
            _userManager = ContextHelper.CreateUserManager(_dataContext);
            _authenticationService = new AuthenticationService(_userManager, configMock.Object);

            _vmConverter = new VmUserConverter();
        }

        [TearDown]
        public void TearDown()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Dispose();
            _dbConnection.Close();
        }

        [Test]
        public void LoginWithNullInputTest()
        {
            Assert.ThrowsAsync<HttpResponseException>(async () => await _authenticationService.Login(null));
        }

        [TestCase("user@email.com", "")]
        [TestCase("", "Password!")]
        public void LoginWithEmptyCredentialsTest(string userName, string password)
        {
            //Arrange
            var userInput = new VmAuthInput { UserName = userName, Password = password };

            //Assert
            Assert.ThrowsAsync<HttpResponseException>(async () => await _authenticationService.Login(userInput));
        }

        [Test]
        public async Task LoginWithUserNotFoundTest()
        {
            //Arrange
            var userInput = new VmAuthInput { UserName = "unknown", Password = "pwd" };

            //Assert
            Assert.ThrowsAsync<HttpResponseException>(async () => await _authenticationService.Login(userInput));
        }


        [Test]
        public async Task LoginTest()
        {
            //Arrange
            var vmUser = new VmUser
            {
                UserName = "user01",
                Email = "user@email.com",
                Phone = "123",
                FirstName = "FirstName",
                MiddleName = "MiddleName",
                LastName = "LastName"
            };
            var user = _vmConverter.ToModel(vmUser);
            await _userManager.CreateAsync(user, "Password!");
            var authInput = new VmAuthInput
            {
                UserName = vmUser.Email,
                Password = "Password!"
            };

            //Act
            var authOutput = await _authenticationService.Login(authInput);

            //Assert
            Assert.IsNotNull(authOutput?.Token);
            Assert.IsFalse(string.IsNullOrWhiteSpace(authOutput.User.Id));
            Assert.AreEqual(vmUser.FirstName, authOutput.User.FirstName);
            Assert.AreEqual(vmUser.MiddleName, authOutput.User.MiddleName);
            Assert.AreEqual(vmUser.LastName, authOutput.User.LastName);
            Assert.AreEqual(vmUser.Email, authOutput.User.Email);
            Assert.AreEqual(vmUser.Phone, authOutput.User.Phone);
        }


        private SqliteConnection _dbConnection;
        private DataContext _dataContext;
        private UserManager<ApplicationUser> _userManager;
        private VmUserConverter _vmConverter;
        private AuthenticationService _authenticationService;
    }
}