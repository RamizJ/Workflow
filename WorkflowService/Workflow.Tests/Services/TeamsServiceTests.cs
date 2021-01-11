using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class TeamsServiceTests
    {
        [SetUp]
        public void Setup()
        {
            var serviceProvider = _testContext.Initialize(out _testData);
            
            _dataContext = serviceProvider.GetRequiredService<DataContext>();
            _service = serviceProvider.GetRequiredService<ITeamsService>();
            _currentUser = _testData.Users.First();
            _vmConverter = new VmTeamConverter();
        }

        [TearDown]
        public void TearDown()
        {
            _testContext.Uninitialize();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task GetTest(int teamId)
        {
            //Arrange
            var expectedTeam = _testData.Teams.First(t => t.Id == teamId);

            //Act
            var team = await _service.Get(_currentUser, teamId);

            //Assert
            Assert.AreEqual(expectedTeam.Id, team.Id);
            Assert.AreEqual(expectedTeam.Name, team.Name);
            Assert.AreEqual(expectedTeam.Description, team.Description);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1000)]
        public async Task GetNotExistedTest(int teamId)
        {
            //Arrange

            //Act
            var team = await _service.Get(_currentUser, teamId);

            //Assert
            Assert.IsNull(team);
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
            var teams = (await _service.GetPage(_testData.Users.First(), pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, teams.Length);
        }

        [TestCase(0, 12, null, 9)]
        [TestCase(0, 5, "Team1", 5)]
        [TestCase(1, 5, "Team1", 1)]
        [TestCase(0, 5, "Team2", 3)]
        [TestCase(1, 5, "Team2", 0)]
        [TestCase(0, 5, "DEscription", 5)]
        [TestCase(1, 5, "DEscription", 4)]
        [TestCase(0, 5, "DEscription1", 1)]
        public async Task GetPageFilterTest(int pageNumber, int pageSize,
            string filter, int expectedCount)
        {
            //Arrange
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = filter
            };

            //Act
            var resultScopes = (await _service.GetPage(_testData.Users.First(), pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }

        [TestCase(0, 5, null, "Name", new object[] {"Team1"}, false, 5)]
        [TestCase(1, 5, null, "Name", new object[] {"Team1"}, false, 1)]
        [TestCase(0, 5, null, "Name", new object[] {"Team2"}, false, 3)]
        [TestCase(0, 5, null, "Description", new object[] {"descriptioN1"}, false, 1)]
        [TestCase(0, 5, null, "isRemoved", new object[] {true}, true, 1)]
        public async Task GetPageFilterFieldsTest(int pageNumber, int pageSize,
            string filter, string fieldName, object[] values, bool withRemoved, int expectedCount)
        {
            //Arrange
            var filterField = new FieldFilter(fieldName, values);
            var pageOptions = new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = filter,
                FilterFields = new []{filterField},
                WithRemoved = withRemoved
            };

            //Act
            var resultScopes = (await _service.GetPage(_currentUser, pageOptions)).ToArray();

            //Assert
            Assert.AreEqual(expectedCount, resultScopes.Length);
        }


        [TestCase(new[] { 1, 2, 3 })]
        [TestCase(new[] { 9, 10 })]
        public async Task GetRangeTest(int[] ids)
        {
            //Arrange

            //Act
            var resultScopes = (await _service.GetRange(_testData.Users.First(), ids)).ToArray();

            //Assert
            Assert.AreEqual(ids.Length, resultScopes.Length);
            for (var i = 0; i < ids.Length; i++)
            {
                Assert.AreEqual(ids[i], resultScopes[i].Id);
            }
        }


        [Test]
        public void CreateForNullInputTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await _service.Create(_currentUser, null));
        }

        [Test]
        public void CreateByFormForNullInputTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.CreateByForm(_currentUser, null));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void CreateForNullInvalidNameTest(string name)
        {
            //Arrange
            var vmTeam = new VmTeam
            {
                Id = 0,
                Name = name,
                IsRemoved = false
            };

            //Act

            //Assert
            Assert.ThrowsAsync<HttpResponseException>(async () => 
                await _service.Create(_currentUser, vmTeam));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void CreateByFormForNullInvalidNameTest(string name)
        {
            //Arrange
            var vmTeam = new VmTeam
            {
                Id = 0,
                Name = name,
                IsRemoved = false
            };

            //Act

            //Assert
            Assert.ThrowsAsync<HttpResponseException>(async () =>
                await _service.CreateByForm(_currentUser, new VmTeamForm
                {
                    Team = vmTeam
                }));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public async Task CreateTest(int id)
        {
            //Arrange
            var vmTeam = new VmTeam
            {
                Id = id,
                Name = "Team",
                IsRemoved = false
            };

            //Act
            var result = await _service.Create(_currentUser, vmTeam);

            //Assert
            Assert.IsNotNull(result);
            Assert.Greater(result.Id, 0);
            Assert.AreEqual(vmTeam.Name, result.Name);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public async Task CreateByFormTest(int id)
        {
            //Arrange
            var vmTeam = new VmTeam
            {
                Id = id,
                Name = "Team",
                IsRemoved = false
            };
            var userIds = _testData.Users.Take(2).Select(u => u.Id).ToList();
            var projectIds = _testData.Projects.Take(3).Select(p => p.Id).ToList();

            //Act
            var vmForm = new VmTeamForm(vmTeam, userIds, projectIds);
            var resultForm = await _service.CreateByForm(_currentUser, vmForm);


            //Assert
            Assert.IsNotNull(resultForm);
            Assert.Greater(resultForm.Team.Id, 0);
            Assert.AreEqual(vmTeam.Name, resultForm.Team.Name);
            Assert.AreEqual(userIds.Count, resultForm.UserIds.Count);
            Assert.AreEqual(projectIds.Count, resultForm.ProjectIds.Count);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public async Task UpdateForNullInvalidNameTest(string name)
        {
            //Arrange
            var team = _testData.Teams.First();
            team.Name = name;
            var vmTeam = _vmConverter.ToViewModel(team);

            //Act
            await _service.Update(_currentUser, vmTeam);

            Assert.Pass();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public async Task UpdateByFormForNullInvalidNameTest(string name)
        {
            //Arrange
            var team = _testData.Teams.First();
            team.Name = name;
            var vmTeam = _vmConverter.ToViewModel(team);

            //Act
            await _service.UpdateByForm(_currentUser, new VmTeamForm(vmTeam));

            Assert.Pass();
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public async Task UpdateForNotExistedTest(int id)
        {
            //Arrange
            var team = _testData.Teams.First();
            team.Id = id;
            var vmTeam = _vmConverter.ToViewModel(team);

            //Act
            await _service.Update(_currentUser, vmTeam);

            //Assert
            Assert.Pass();
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public async Task UpdateByFormForNotExistedTest(int id)
        {
            //Arrange
            var team = _testData.Teams.First();
            team.Id = id;
            var vmTeam = _vmConverter.ToViewModel(team);

            //Act
            await _service.UpdateByForm(_testData.Users.First(), new VmTeamForm(vmTeam));

            //Assert
            Assert.Pass();
        }

        [TestCase("TeamNew1", "DescriptionNew1")]
        public async Task UpdateTest(string name, string description)
        {
            //Arrange
            var team = _testData.Teams.First();
            team.Name = name;
            team.Description = description;
            var vmTeam = _vmConverter.ToViewModel(team);

            //Act
            await _service.Update(_currentUser, vmTeam);
            var expectedTeam = _dataContext.Teams.First();

            //Assert
            Assert.AreEqual(name, expectedTeam.Name);
            Assert.AreEqual(description, expectedTeam.Description);
        }

        [Test]
        public async Task UpdateRangeTest()
        {
            //Arrange
            string updatedName = "UpdatedName";
            string updatedDescription = "UpdatedDescription";
            var vmTeams = _testData.Teams.Select(t =>
            {
                t.Name = updatedName;
                t.Description = updatedDescription;
                return _vmConverter.ToViewModel(t);
            }).ToArray();
            var userIds = _testData.Users.Skip(4).Take(6).Select(u => u.Id).ToList();
            var projectIds = _testData.Projects.Skip(6).Take(4).Select(p => p.Id).ToList();

            var vmTeamForms = vmTeams
                .Select(vm => new VmTeamForm(vm, userIds, projectIds));

            //Act
            await _service.UpdateByFormRange(_currentUser, vmTeamForms);
            var teams = await _dataContext.Teams
                .Include(t => t.TeamUsers)
                .Include(t => t.TeamProjects)
                .ToArrayAsync();

            //Assert
            foreach (var team in teams)
            {
                Assert.AreEqual(updatedName, team.Name);
                Assert.AreEqual(updatedDescription, team.Description);
                Assert.AreEqual(userIds.Count, team.TeamUsers.Count);
                Assert.AreEqual(projectIds.Count, team.TeamProjects.Count);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        public async Task DeleteTest(int teamId)
        {
            //Arrange

            //Act
            var result = await _service.Delete(_currentUser, teamId);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(teamId, result.Id);
        }

        [Test]
        public async Task DeleteRangeTest()
        {
            //Arrange
            var teamIds = new[] {0, 1, 2};

            //Act
            var teams = (await _service.DeleteRange(_currentUser, teamIds)).ToArray();

            //Assert
            Assert.IsNotNull(teams);
            Assert.AreEqual(2, teams.Length);
            Assert.AreEqual(1, teams[0].Id);
            Assert.AreEqual(2, teams[1].Id);
            Assert.IsTrue(teams[0].IsRemoved);
            Assert.IsTrue(teams[1].IsRemoved);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public async Task DeleteNotExistedTest(int teamId)
        {
            //Act
            var team = await _service.Delete(_currentUser, teamId);

            //Assert
            Assert.IsNull(team);
        }


        [TestCase(1)]
        [TestCase(10)]
        public async Task RestoreTest(int teamId)
        {
            //Act
            var result = await _service.Restore(_currentUser, teamId);

            //Assert
            Assert.IsFalse(result.IsRemoved);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public async Task RestoreNotExistedTest(int teamId)
        {
            //Act
            var result = await _service.Restore(_currentUser, teamId);

            //Assert
            Assert.IsNull(result);
        }


        private readonly TestContext _testContext = new();
        private TestData _testData;
        private DataContext _dataContext;
        private ITeamsService _service;
        private ApplicationUser _currentUser;
        private VmTeamConverter _vmConverter;
    }
}