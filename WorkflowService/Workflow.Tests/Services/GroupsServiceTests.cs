using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PageLoading;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.VM.ViewModelConverters;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class GroupsServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _testContext.Initialize();
            _vmConverter = new VmGroupConverter(new VmProjectConverter(), new VmMetadataConverter());
            _dataContext = _testContext.DataContext;
            _currentUser = _testContext.CurrentUser;
            _testData = _testContext.TestData;
            _service = _testContext.ServiceProvider.GetService<IGroupsService>();
        }

        [TearDown]
        public void TearDown()
        {
            _testContext.Uninitialize();
        }

        [TestCase(int.MinValue)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public async Task GetWithInvalidIdTest(int invalidId)
        {
            //Act
            var result = await _service.Get(_currentUser, invalidId);

            //
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetTest()
        {
            //Arrange
            int firstGroupId = _testData.Groups.First().Id;

            //Act
            var result = await _service.Get(_currentUser, firstGroupId);

            //Assert
            Assert.AreEqual(firstGroupId, result.Id);
        }

        [Test]
        public async Task GetPageTest()
        {
            //Act
            var groups = await _service.GetPage(_currentUser, null, new PageOptions(0, 10));

            //Assert
            Assert.AreEqual(9, groups.Count());
        }

        [TestCase("Group", 9)]
        [TestCase("5", 1)]
        public async Task GetPageFilterTest(string filter, int expectedCount)
        {
            //Act
            var groups = await _service.GetPage(_currentUser, null, new PageOptions(0, 10)
            {
                Filter = filter
            });

            //Assert
            Assert.AreEqual(expectedCount, groups.Count());
        }

        [TestCase("Name", "Group", 9)]
        [TestCase("Name", "5", 1)]
        public async Task GetPageFilterFieldTest(string fieldName, string fieldValue, int expectedCount)
        {
            //Act
            var groups = await _service.GetPage(_currentUser, null, new PageOptions(0, 10)
            {
                FilterFields = new []
                {
                    new FieldFilter(fieldName, new object[] {fieldValue})
                }
            });

            //Assert
            Assert.AreEqual(expectedCount, groups.Count());
        }

        [Test]
        public void GetPageFilterNotExistedFieldTest()
        {
            //Arrange 
            var pageOptions = new PageOptions(0, 10)
            {
                FilterFields = new[]
                {
                    new FieldFilter("NotExistedField", new object[] {null})
                }
            };

            //Assert
            Assert.ThrowsAsync<HttpResponseException>(async () =>
                await _service.GetPage(_currentUser, null, pageOptions));
        }

        [Test]
        public async Task AddProjectToGroupTest()
        {
            //Arrange 
            int groupId = _testData.Groups.First().Id;
            var project1 = _testData.Projects[0].Id;
            var project2 = _testData.Projects[1].Id;

            //Act
            await _service.AddProjects(_currentUser, groupId, new[] {project1, project2});
            var groupProjectsCount = await _dataContext.Projects.Where(p => p.GroupId == groupId).CountAsync();

            //Assert
            Assert.AreEqual(2, groupProjectsCount);
        }

        [Test]
        public async Task RemoveProjectFromGroupTest()
        {
            //Arrange 
            int groupId = _testData.Groups.First().Id;
            var project1 = _testData.Projects[0].Id;
            var project2 = _testData.Projects[1].Id;

            //Act
            await _service.RemoveProjects(_currentUser, groupId, new[] { project1, project2 });
            var groupProjectsCount = await _dataContext.Projects.Where(p => p.GroupId == groupId).CountAsync();

            //Assert
            Assert.AreEqual(1, groupProjectsCount);
        }

        private readonly TestContext _testContext = new TestContext();
        private IGroupsService _service;
        private VmGroupConverter _vmConverter;
        private TestData _testData;
        private ApplicationUser _currentUser;
        private DataContext _dataContext;
    }
}