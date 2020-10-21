using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmTeamConverter : IViewModelConverter<Team, VmTeam>
    {
        public Team ToModel(VmTeam viewModel)
        {
            if (viewModel == null)
                return null;

            var model = new Team();
            SetModel(viewModel, model);

            return model;
        }

        public VmTeam ToViewModel(Team model)
        {
            if (model == null)
                return null;

            var viewModel = new VmTeam();
            SetViewModel(model, viewModel);

            return viewModel;
        }

        public void SetModel(VmTeam viewModel, Team model)
        {
            if(viewModel == null || model == null)
                return;

            model.Id = viewModel.Id;
            model.GroupId = viewModel.GroupId == 0 ? null : viewModel.GroupId;
            model.Name = viewModel.Name;
            model.Description = viewModel.Description;
        }

        public void SetViewModel(Team model, VmTeam viewModel)
        {
            if(model == null || viewModel == null)
                return;

            viewModel.Id = model.Id;
            viewModel.Name = model.Name;
            viewModel.Description = model.Description;
            viewModel.GroupId = model.GroupId;
            viewModel.GroupName = model.Group?.Name;
            viewModel.IsRemoved = model.IsRemoved;
        }
    }
}