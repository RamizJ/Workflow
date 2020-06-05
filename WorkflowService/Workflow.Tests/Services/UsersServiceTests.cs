using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services;
using Workflow.Services.Common;
using Workflow.VM.ViewModelConverters;

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
            var user = await _service.Get(_testData.Users.First(), emptyUserId);

            //Assert
            Assert.IsNull(user);
        }


        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetUserTest(int userIndex)
        {
            //Arrange
            var currentUser = _testData.Users.First();
            var expectedUser = _testData.Users[userIndex];

            //Act
            var resultUser = await _service.Get(currentUser, expectedUser.Id);

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

            //Act
            var resultScopes = (await _service.GetPage(_testData.Users.First(),
                pageNumber, pageSize,
                "", null, null)).ToArray();

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

            //Act
            var resultScopes = (await _service.GetPage(_testData.Users.First(), pageNumber, pageSize,
                filter, null, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }


        [TestCase(0, 5, null, "Email", "Email", 5)]
        [TestCase(1, 5, null, "Email", "Email", 4)]
        [TestCase(0, 5, null, "Email", "Email1", 1)]
        [TestCase(0, 5, null, "Phone", "Phone2", 3)]
        [TestCase(1, 5, null, "FirstName", "Firstname1", 1)]
        [TestCase(1, 5, null, "LastName", "lastname1", 1)]
        [TestCase(1, 5, null, "MiddleName", "middlename1", 1)]
        public async Task GetPageFilterFieldsTest(int pageNumber, int pageSize,
            string filter, string fieldName, object value, int expectedCount)
        {
            //Arrange

            //Act
            var filterField = new FieldFilter(fieldName, value);
            var resultScopes = (await _service.GetPage(_testData.Users.First(), pageNumber, pageSize,
                filter, new[] { filterField }, null)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }


        [TestCase(null)]
        public async Task GetRangeForNullInputTest(string[] ids)
        {
            //Act
            var resultScopes = await _service.GetRange(_testData.Users.First(), ids);

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
            var resultUsers = (await _service.GetRange(_testData.Users.First(), ids))
                .OrderBy(u => u.Id)
                .ToArray();

            //Assert
            Assert.AreEqual(ids.Length, resultUsers.Length);
            for (var i = 0; i < ids.Length; i++)
            {
                Assert.AreEqual(ids[i], resultUsers[i].Id);
            }
        }


        [Test]
        public void CreateForNullInputTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.Create(null));
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
            vmUser.Password = "Aa010110!";


            //Act
            var result = await _service.Create(vmUser);

            //Assert
            Assert.IsTrue(result.Succeeded);
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

        [TestCase("UserNameNew1", "EmailNew1@email", "PhoneNew1", "LastNameNew1", "FirstNameNew1", "MiddleNameNew1", 1, "Pos1")]
        [TestCase("UserNameNew2", "EmailNew2@email", "PhoneNew2", "LastNameNew2", "FirstNameNew2", "MiddleNameNew2", 2, "Pos2")]
        public async Task UpdateTest(string userName, string email, string phone,
            string lastName, string firstName, string middleName, 
            int positionIndex, string positionCustom)
        {
            //Arrange
            int posId = _testData.Positions[positionIndex].Id;
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
            var result = await _service.Update(vmUser);

            //Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(userName, result.Data.UserName);
            Assert.AreEqual(email, result.Data.Email);
            Assert.AreEqual(phone, result.Data.Phone);
            Assert.AreEqual(lastName, result.Data.LastName);
            Assert.AreEqual(firstName, result.Data.FirstName);
            Assert.AreEqual(middleName, result.Data.MiddleName);
            Assert.AreEqual(posId, result.Data.PositionId);
            Assert.AreEqual(positionCustom, result.Data.Position);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("123")]
        public void DeleteNotExisted(string userId)
        {
            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.Delete(userId));
        }

        [Test]
        public async Task Delete()
        {
            //Arrange
            var userId = _testData.Users[3].Id;
            //Act
            var result = await _service.Delete(userId);

            //Assert
            Assert.IsTrue(result.Succeeded);
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
            var result = await _service.ChangePassword(user, "Aa010110!", newPassword);

            //Assert
            Assert.AreEqual(isSucceed, result.Succeeded);
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
            var result = await _service.ResetPassword(user.Id, newPassword);

            //Assert
            Assert.AreEqual(isSucceed, result.Succeeded);
        }
    }
}