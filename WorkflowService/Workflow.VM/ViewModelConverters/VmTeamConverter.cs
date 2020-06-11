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

            return new Team
            {
                Id = viewModel.Id,
                GroupId = viewModel.GroupId,
                Name = viewModel.Name,
                Description = viewModel.Description,
            };
        }

        public VmTeam ToViewModel(Team model)
        {
            if (model == null)
                return null;

            return new VmTeam
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                GroupId = model.GroupId,
                GroupName = model.Group?.Name,
                IsRemoved = model.IsRemoved
            };
        }
    }
}