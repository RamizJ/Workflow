using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmProjectUserRoleConverter : IViewModelConverter<ProjectUserRole, VmProjectUserRole>
    {
        public ProjectUserRole ToModel(VmProjectUserRole viewModel)
        {
            if (viewModel == null)
                return null;

            var model = new ProjectUserRole();
            SetModel(viewModel, model);
            return model;
        }

        public VmProjectUserRole ToViewModel(ProjectUserRole model)
        {
            if (model == null)
                return null;

            var vm = new VmProjectUserRole();
            SetViewModel(model, vm);
            return vm;
        }

        public void SetModel(VmProjectUserRole viewModel, ProjectUserRole model)
        {
            if (viewModel == null || model == null)
                return;

            model.CanCloseGoals = viewModel.CanCloseGoals;
            model.CanEditGoals = viewModel.CanEditGoals;
            model.CanEditUsers = viewModel.CanEditUsers;
        }

        public void SetViewModel(ProjectUserRole model, VmProjectUserRole viewModel)
        {
            if (viewModel == null || model == null)
                return;

            viewModel.CanCloseGoals = model.CanCloseGoals;
            viewModel.CanEditGoals = model.CanEditGoals;
            viewModel.CanEditUsers = model.CanEditUsers;
        }
    }
}