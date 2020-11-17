using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services;
using Workflow.VM.Common;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class UsersServiceTests
    {
        private SqliteConnection _dbConnection;
        private TestData _testData;
        private DataContext _dataContext;
        private UsersService _service;
        private VmUserConverter _vmConverter;
        private UserManager<ApplicationUser> _userManager;
        private ServiceProvider _serviceProvider;
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
            _userManager = _serviceProvider.GetService<UserManager<ApplicationUser>>();
            _service = new UsersService(_dataContext, _userManager);
            _vmConverter = new VmUserConverter();
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
        public void GetUserForNullCurrentTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.Get(null, _testData.Users.First().Id));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task GetUserForNullIdTest(string emptyUserId)
        {
            //Arrange
            var user = await _service.Get(_currentUser, emptyUserId);

            //Assert
            Assert.IsNull(user);
        }


        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetUserTest(int userIndex)
        {
            //Arrange
            var expectedUser = _testData.Users[userIndex];

            //Act
            var resultUser = await _service.Get(_currentUser, expectedUser.Id);

            //Assert
            Assert.AreEqual(expectedUser.Id, resultUser.Id);
            Assert.AreEqual(expectedUser.LastName, resultUser.LastName);
        }

        [Test]
        public void GetCurrentUserForNullTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetCurrent(null));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetCurrentUserTest(int userIndex)
        {
            //Arrange
            var expectedUser = _testData.Users[userIndex];

            //Act
            var resultUser = await _service.GetCurrent(expectedUser);

            //Assert
            Assert.AreEqual(expectedUser.Id, resultUser.Id);
            Assert.AreEqual(expectedUser.LastName, resultUser.LastName);
        }


        [TestCase(0, 12, 9)]
        [TestCase(0, 5, 5)]
        [TestCase(1, 5, 4)]
        [TestCase(2, 5, 0)]
        public async Task GetPageTest(int pageNumber, int pageSize, int expectedCount)
        {
            //Arrange
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
            };

            //Act
            var resultScopes = (await _service.GetPage(_currentUser, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }


        [TestCase(0, 12, null, 9)]
        [TestCase(0, 5, "FirstName1", 5)]
        [TestCase(1, 5, "FirstName1", 1)]
        [TestCase(0, 5, "LastName1", 5)]
        [TestCase(1, 5, "LastName1", 1)]
        [TestCase(0, 5, "FirstName2", 3)]
        [TestCase(1, 5, "FirstName2", 0)]
        [TestCase(1, 5, "Email ", 4)]
        [TestCase(0, 5, "Phone1 ", 5)]
        [TestCase(0, 5, "Phone2 ", 3)]
        public async Task GetPageFilterTest(int pageNumber, int pageSize,
            string filter, int expectedCount)
        {
            //Arrange
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = filter,
            };

            //Act
            var resultScopes = (await _service.GetPage(_currentUser, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }


        [TestCase(0, 5, null, "Email", new []{"Email"}, 5)]
        [TestCase(1, 5, null, "Email", new[] { "Email" }, 4)]
        [TestCase(0, 5, null, "Email", new[] { "Email", "Email1" }, 5)]
        [TestCase(0, 5, null, "Email", new[] { "Email1" }, 1)]
        [TestCase(0, 5, null, "Email", new[] { "1@", "2@", "3@" }, 3)]
        [TestCase(0, 5, null, "Phone", new[] { "Phone2" }, 3)]
        [TestCase(1, 5, null, "FirstName", new[] { "Firstname1" }, 1)]
        [TestCase(1, 5, null, "LastName", new[] { "lastname1" }, 1)]
        [TestCase(1, 5, null, "MiddleName", new[] { "middlename1" }, 1)]
        public async Task GetPageFilterFieldsTest(int pageNumber, int pageSize,
            string filter, string fieldName, object[] value, int expectedCount)
        {
            //Arrange
            var filterField = new FieldFilter(fieldName, value);
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = filter,
                FilterFields = new[] { filterField }
            };

            //Act
            var result = (await _service.GetPage(_currentUser, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, result.Length);
        }

        [TestCase(1, 5, null, "IsRemoved", new object[] { true, false }, 5)]
        [TestCase(0, 5, null, "IsRemoved", new object[] { true }, 1)]
        public async Task GetPageFilterFieldRemovedTest(int pageNumber, int pageSize,
            string filter, string fieldName, object[] value, int expectedCount)
        {
            //Arrange
            var filterField = new FieldFilter(fieldName, value);
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = filter,
                FilterFields = new[] { filterField },
                WithRemoved = true
            };

            //Act
            var result = (await _service.GetPage(_currentUser,
                pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, result.Length);
        }


        [TestCase(null)]
        public async Task GetRangeForNullInputTest(string[] ids)
        {
            //Act
            var resultScopes = await _service.GetRange(_currentUser, ids);

            //Assert
            Assert.IsNull(resultScopes);
        }


        [TestCase(new[] { 1, 2, 3 })]
        [TestCase(new[] { 8, 9 })]
        public async Task GetRangeTest(int[] indexes)
        {
            //Arrange
            var ids = _testData.Users
                .Where((u, i) => indexes.Any(idx => idx == i))
                .Select((u,i) => u.Id).ToArray()
                .OrderBy(id => id)
                .ToArray();

            //Act
            var resultUsers = (await _service.GetRange(_currentUser, ids))
                .OrderBy(u => u.Id)
                .ToArray();

            //Assert
            Assert.AreEqual(ids.Length, resultUsers.Length);
            for (var i = 0; i < ids.Length; i++)
            {
                Assert.AreEqual(ids[i], resultUsers[i].Id);
            }
        }


        [TestCase("Email1@email", true)]
        [TestCase("email2@email", true)]
        [TestCase("email101@email", false)]
        public async Task IsEmailExistTest(string email, bool isExist)
        {
            //Arrange

            //Act
            var result = await _service.IsEmailExist(email);

            //Assert
            Assert.AreEqual(isExist, result);
        }

        [TestCase("User0", true)]
        [TestCase("user1", true)]
        [TestCase("user_", false)]
        public async Task IsUserNameExistTest(string userName, bool isExist)
        {
            //Arrange

            //Act
            var result = await _service.IsUserNameExist(userName);

            //Assert
            Assert.AreEqual(isExist, result);
        }


        [Test]
        public void CreateForNullInputTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.Create(null, "Aa010110!"));
        }

        [Test]
        public async Task CreateTest()
        {
            //Arrange
            string i = "New";
            var user = Builder<ApplicationUser>.CreateNew()
                .With(x => x.UserName = "UserName")
                .With(x => x.NormalizedUserName = x.UserName.ToUpper())
                .With(x => x.Email = $"Email{i}@email")
                .With(x => x.NormalizedEmail = x.Email.ToUpper())
                .With(x => x.PositionId = null)
                .With(x => x.FirstName = $"FirstName{i}")
                .With(x => x.LastName = $"LastName{i}")
                .With(x => x.MiddleName = $"MiddleName{i}")
                .With(x => x.PhoneNumber = $"Phone{i}")
                .Build();

            var vmUser = _vmConverter.ToViewModel(user);
            string password = "Aa010110!";


            //Act
            var result = await _service.Create(vmUser, password);

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void UpdateForNullInputTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.Update(null));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("123")]
        public void UpdateNotExisted(string userId)
        {
            var user = _testData.Users.First();
            user.Id = userId;
            var vmUser = _vmConverter.ToViewModel(user);

            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.Update(vmUser));
        }

        [TestCase("UserNameNew1", "EmailNew1@email", "PhoneNew1", "LastNameNew1", "FirstNameNew1", "MiddleNameNew1", null, "Pos1")]
        [TestCase("UserNameNew2", "EmailNew2@email", "PhoneNew2", "LastNameNew2", "FirstNameNew2", "MiddleNameNew2", 2, null)]
        public async Task UpdateTest(string userName, string email, string phone,
            string lastName, string firstName, string middleName, 
            int? posId, string positionCustom)
        {
            //Arrange
            var user = _testData.Users.First();
            user.UserName = userName;
            user.Email = email;
            user.PhoneNumber = phone;
            user.LastName = lastName;
            user.FirstName = firstName;
            user.MiddleName = middleName;
            user.PositionId = posId;
            user.PositionCustom = positionCustom;
            var vmUser = _vmConverter.ToViewModel(user);

            //Act
            await _service.Update(vmUser);
            var expectedUser = _dataContext.Users.First();

            //Assert
            Assert.AreEqual(userName, expectedUser.UserName);
            Assert.AreEqual(email, expectedUser.Email);
            Assert.AreEqual(phone, expectedUser.PhoneNumber);
            Assert.AreEqual(lastName, expectedUser.LastName);
            Assert.AreEqual(firstName, expectedUser.FirstName);
            Assert.AreEqual(middleName, expectedUser.MiddleName);
            Assert.AreEqual(posId, expectedUser.PositionId);
            Assert.AreEqual(positionCustom, expectedUser.PositionCustom);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("123")]
        public async Task DeleteNotExisted(string userId)
        {
            //Act
            var user = await _service.Delete(_currentUser, userId);

            //Assert
            Assert.IsNull(user);
        }

        [Test]
        public async Task Delete()
        {
            //Arrange
            var userId = _testData.Users[3].Id;
            //Act
            var result = await _service.Delete(_currentUser, userId);

            //Assert
            Assert.IsNotNull(result);
        }


        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("123", false)]
        [TestCase("abc", false)]
        [TestCase("Aa12345!", true)]
        public async Task ChangePasswordTest(string newPassword, bool isSucceed)
        {
            //Arrange
            var user = await _userManager.FindByIdAsync(_testData.Users.First().Id);

            //Act
            bool result = true;
            try
            {
                await _service.ChangePassword(user, "Aa010110!", newPassword);
            }
            catch (Exception)
            {
                result = false;
            }

            //Assert
            Assert.AreEqual(isSucceed, result);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("123", false)]
        [TestCase("abc", false)]
        [TestCase("Aa12345!", true)]
        public async Task ResetPasswordTest(string newPassword, bool isSucceed)
        {
            //Arrange
            var user = _testData.Users.First();

            //Act
            bool result = true;
            try
            {
                await _service.ResetPassword(user.Id, newPassword);
            }
            catch (Exception)
            {
                result = false;
            }

            //Assert
            Assert.AreEqual(isSucceed, result);
        }
    }
}