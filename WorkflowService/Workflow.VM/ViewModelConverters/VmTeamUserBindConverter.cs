using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmTeamUserBindConverter : IViewModelConverter<TeamUser, VmTeamUserBind>
    {
        public TeamUser ToModel(VmTeamUserBind viewModel)
        {
            if (viewModel == null)
                return null;

            var model = new TeamUser();
            SetModel(viewModel, model);

            return model;
        }

        public VmTeamUserBind ToViewModel(TeamUser model)
        {
            if (model == null)
                return null;

            var viewModel = new VmTeamUserBind();
            SetViewModel(model, viewModel);

            return viewModel;
        }

        public void SetModel(VmTeamUserBind viewModel, TeamUser model)
        {
            if(model == null || viewModel == null) 
                return;

            model.TeamId = viewModel.TeamId;
            model.UserId = viewModel.UserId;
            model.CanCloseGoals = viewModel.CanCloseGoals;
            model.CanEditGoals = viewModel.CanEditGoals;
            model.CanEditUsers = viewModel.CanEditUsers;
        }

        public void SetViewModel(TeamUser model, VmTeamUserBind viewModel)
        {
            if (model == null || viewModel == null)
                return;

            viewModel.TeamId = model.TeamId;
            viewModel.UserId = model.UserId;
            viewModel.CanCloseGoals = model.CanCloseGoals;
            viewModel.CanEditGoals = model.CanEditGoals;
            viewModel.CanEditUsers = model.CanEditUsers;
        }
    }
}