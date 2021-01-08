using System;
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
using Workflow.VM.ViewModels;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class GroupsServiceTests
    {
        [SetUp]
        public void Setup()
        {
            var serviceProvider = _testContext.Initialize(out _testData);
            
            _vmConverter = new VmGroupConverter(new VmProjectConverter(), new VmMetadataConverter());
            _dataContext = serviceProvider.GetRequiredService<DataContext>();
            _currentUser = _testContext.CurrentUser;
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
            var project4 = _testData.Projects[3].Id;
            var project5 = _testData.Projects[4].Id;

            //Act
            await _service.AddProjects(_currentUser, groupId, new[] {project4, project5});
            var groupProjectsCount = await _dataContext.Projects.Where(p => p.GroupId == groupId).CountAsync();

            //Assert
            Assert.AreEqual(5, groupProjectsCount);
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


        [Test]
        public async Task CreateGroupTest()
        {
            //Arrange
            var vmGroup = new VmGroup
            {
                Name = "New group"
            };

            //Act
            var result = await _service.Create(_currentUser, vmGroup);

            //Assert
            Assert.NotNull(result);
            Assert.AreEqual(vmGroup.Name, result.Name);
            Assert.AreEqual(0, (DateTime.Now.ToUniversalTime() - result.CreationDate).TotalSeconds, 10);
            Assert.AreNotEqual(0, result.Id);
        }

        [Test]
        public async Task UpdateGroupTest()
        {
            //Arrange
            var vmGroup = _vmConverter.ToViewModel(_testData.Groups.First());
            var expectedCreationDate = vmGroup.CreationDate;
            vmGroup.CreationDate = DateTime.Now + TimeSpan.FromDays(2);
            vmGroup.Name = "New name";

            //Act
            await _service.Update(_currentUser, vmGroup);
            var result = _dataContext.Groups.First(x => x.Id == vmGroup.Id);


            //Assert
            Assert.NotNull(result);
            Assert.AreEqual(vmGroup.Name, result.Name);
            Assert.AreEqual(0, (result.CreationDate - expectedCreationDate).TotalSeconds, 10);
        }

        [Test]
        public async Task UpdateGroupRangeTest()
        {
            //Arrange
            var vmGroup = _vmConverter.ToViewModel(_testData.Groups.First());
            var expectedCreationDate = vmGroup.CreationDate;
            vmGroup.CreationDate = DateTime.Now + TimeSpan.FromDays(2);
            vmGroup.Name = "New name";

            //Act
            await _service.UpdateRange(_currentUser, new[] {vmGroup});
            var result = _dataContext.Groups.First(x => x.Id == vmGroup.Id);


            //Assert
            Assert.NotNull(result);
            Assert.AreEqual(vmGroup.Name, result.Name);
            Assert.AreEqual(0, (result.CreationDate - expectedCreationDate).TotalSeconds, 10);
        }


        [Test]
        public async Task DeleteGroupTest()
        {
            //Arrange
            var groupId = _testData.Groups.First().Id;

            //Act
            await _service.Delete(_currentUser, groupId);
            var result = _dataContext.Groups.First(x => x.Id == groupId);


            //Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.IsRemoved, true);
        }

        [Test]
        public async Task DeleteGroupRangeTest()
        {
            //Arrange
            var groupId = _testData.Groups.First().Id;

            //Act
            await _service.DeleteRange(_currentUser, new[] {groupId});
            var result = _dataContext.Groups.First(x => x.Id == groupId);


            //Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.IsRemoved);
        }

        [Test]
        public async Task RestoreTest()
        {
            //Arrange
            var groupId = _testData.Groups.Last().Id;

            //Act
            await _service.Restore(_currentUser, groupId);
            var result = _dataContext.Groups.First(x => x.Id == groupId);


            //Assert
            Assert.NotNull(result);
            Assert.IsFalse(result.IsRemoved);
        }

        [Test]
        public async Task RestoreRangeTest()
        {
            //Arrange
            var groupId = _testData.Groups.Last().Id;

            //Act
            await _service.RestoreRange(_currentUser, new[] { groupId });
            var result = _dataContext.Groups.First(x => x.Id == groupId);


            //Assert
            Assert.NotNull(result);
            Assert.IsFalse(result.IsRemoved);
        }


        private readonly TestContext _testContext = new();
        private IGroupsService _service;
        private VmGroupConverter _vmConverter;
        private TestData _testData;
        private ApplicationUser _currentUser;
        private DataContext _dataContext;
    }
}