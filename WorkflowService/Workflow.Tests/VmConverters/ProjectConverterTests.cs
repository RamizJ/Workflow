using FizzWare.NBuilder;
using NUnit.Framework;
using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters;

namespace Workflow.Tests.VmConverters
{
    [TestFixture]
    public class ProjectConverterTests
    {
        private VmProjectConverter _vmConverter;

        [SetUp]
        public void SetUp()
        {
            _vmConverter = new VmProjectConverter();
        }


        [Test]
        public void NullToViewModelTest()
        {
            //Arrange

            //Act
            var vm = _vmConverter.ToViewModel(null);

            //Assert
            Assert.IsNull(vm);
        }

        [Test]
        public void ToViewModelTest()
        {
            //Arrange
            var scope = Builder<Project>.CreateNew()
                .With(x => x.Team = new Team{Id = x.TeamId ?? 1, Name = "Team"})
                .With(x => x.Group = new Group { Id = x.GroupId ?? 1, Name = "Group" })
                .With(x => x.OwnerId = "userId")
                .With(x => x.Owner = new ApplicationUser { Id = x.OwnerId, FirstName = "A", MiddleName = "B", LastName = "C"})
                .Build();

            //Act
            var vm = _vmConverter.ToViewModel(scope);

            //Assert
            Assert.IsNotNull(vm);
            Assert.AreEqual(scope.Id, vm.Id);
            Assert.AreEqual(scope.Name, vm.Name);
            Assert.AreEqual(scope.Description, vm.Description);
            Assert.AreEqual(scope.OwnerId, vm.OwnerId);
            Assert.AreEqual(scope.Owner.Fio, vm.OwnerFio);
            Assert.AreEqual(scope.TeamId, vm.TeamId);
            Assert.AreEqual(scope.Team.Name, vm.TeamName);
            Assert.AreEqual(scope.GroupId, vm.GroupId);
            Assert.AreEqual(scope.Group.Name, vm.GroupName);
            Assert.AreEqual(scope.CreationDate, vm.CreationDate);
            Assert.AreEqual(scope.IsRemoved, vm.IsRemoved);
        }
    }
}
