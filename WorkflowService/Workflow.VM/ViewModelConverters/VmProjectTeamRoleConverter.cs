using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmProjectTeamRoleConverter : IViewModelConverter<ProjectTeamRole, VmProjectTeamRole>
    {
        public ProjectTeamRole ToModel(VmProjectTeamRole viewModel)
        {
            if (viewModel == null)
                return null;

            var model = new ProjectTeamRole();
            SetModel(viewModel, model);
            return model;
        }

        public VmProjectTeamRole ToViewModel(ProjectTeamRole model)
        {
            if (model == null)
                return null;

            var viewModel = new VmProjectTeamRole();
            SetViewModel(model, viewModel);
            return viewModel;
        }

        public void SetModel(VmProjectTeamRole viewModel, ProjectTeamRole model)
        {
            if (viewModel == null || model == null)
                return;

            model.CanCloseGoals = viewModel.CanCloseGoals;
            model.CanEditGoals = viewModel.CanEditGoals;
            model.CanEditUsers = viewModel.CanEditUsers;
        }

        public void SetViewModel(ProjectTeamRole model, VmProjectTeamRole viewModel)
        {
            if (viewModel == null || model == null)
                return;

            viewModel.CanCloseGoals = model.CanCloseGoals;
            viewModel.CanEditGoals = model.CanEditGoals;
            viewModel.CanEditUsers = model.CanEditUsers;
        }
    }
}