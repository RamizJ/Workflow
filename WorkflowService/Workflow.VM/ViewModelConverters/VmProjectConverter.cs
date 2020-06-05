using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmProjectConverter : IViewModelConverter<Project, VmProject>
    {
        public Project ToModel(VmProject viewModel)
        {
            if (viewModel == null)
                return null;

            return new Project
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
                OwnerId = viewModel.OwnerId,
                TeamId = viewModel.TeamId,
                GroupId = viewModel.GroupId,
            };
        }

        public VmProject ToViewModel(Project model)
        {
            if (model == null)
                return null;

            return new VmProject
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                OwnerId = model.OwnerId,
                OwnerFio = model.Owner?.Fio,
                TeamId = model.TeamId,
                TeamName = model.Team?.Name,
                GroupId = model.GroupId,
                GroupName = model.Group?.Name,
                CreationDate = model.CreationDate,
                IsRemoved = model.IsRemoved
            };
        }
    }
}