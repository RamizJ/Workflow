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

            var model = new Project();
            SetModel(viewModel, model);

            return model;
        }

        public VmProject ToViewModel(Project model)
        {
            if (model == null)
                return null;

            var viewModel = new VmProject();
            SetViewModel(model, viewModel);

            return viewModel;
        }

        public void SetModel(VmProject viewModel, Project model)
        {
            if(viewModel == null || model == null)
                return;

            model.Id = viewModel.Id;
            model.Name = viewModel.Name;
            model.Description = viewModel.Description;
            model.ExpectedCompletedDate = viewModel.ExpectedCompletedDate;
            model.OwnerId = viewModel.OwnerId;
            model.GroupId = viewModel.GroupId == 0 ? null : viewModel.GroupId;
        }

        public void SetViewModel(Project model, VmProject viewModel)
        {
            if (viewModel == null || model == null)
                return;

            viewModel.Id = model.Id;
            viewModel.Name = model.Name;
            viewModel.Description = model.Description;
            viewModel.OwnerId = model.OwnerId;
            viewModel.OwnerFio = model.Owner?.Fio;
            viewModel.GroupId = model.GroupId;
            viewModel.GroupName = model.Group?.Name;
            viewModel.CreationDate = model.CreationDate;
            viewModel.ExpectedCompletedDate = model.ExpectedCompletedDate;
            viewModel.IsRemoved = model.IsRemoved;
        }
    }
}