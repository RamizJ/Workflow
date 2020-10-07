using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmProjectTeamRoleConverter : IViewModelConverter<ProjectTeam, VmProjectTeamRole>
    {
        public ProjectTeam ToModel(VmProjectTeamRole viewModel)
        {
            if (viewModel == null)
                return null;

            var model = new ProjectTeam();
            SetModel(viewModel, model);
            return model;
        }

        public VmProjectTeamRole ToViewModel(ProjectTeam model)
        {
            if (model == null)
                return null;

            var viewModel = new VmProjectTeamRole();
            SetViewModel(model, viewModel);
            return viewModel;
        }

        public void SetModel(VmProjectTeamRole viewModel, ProjectTeam model)
        {
            if (viewModel == null || model == null)
                return;

            model.ProjectId = viewModel.ProjectId;
            model.TeamId = viewModel.TeamId;

            model.CanCloseGoals = viewModel.CanCloseGoals;
            model.CanEditGoals = viewModel.CanEditGoals;
            model.CanEditUsers = viewModel.CanEditUsers;
        }

        public void SetViewModel(ProjectTeam model, VmProjectTeamRole viewModel)
        {
            if (viewModel == null || model == null)
                return;

            viewModel.ProjectId = model.ProjectId;
            viewModel.TeamId = model.TeamId;

            viewModel.CanCloseGoals = model.CanCloseGoals;
            viewModel.CanEditGoals = model.CanEditGoals;
            viewModel.CanEditUsers = model.CanEditUsers;
        }
    }
}