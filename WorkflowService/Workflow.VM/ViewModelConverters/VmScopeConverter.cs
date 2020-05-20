using System.Linq;
using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmScopeConverter : IViewModelConverter<Scope, VmScope>
    {
        public Scope ToModel(VmScope viewModel)
        {
            if (viewModel == null)
                return null;

            return new Scope
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
                OwnerId = viewModel.OwnerId,
                TeamId = viewModel.TeamId,
                GroupId = viewModel.GroupId,
                CreationDate = viewModel.CreationDate,
                IsRemoved = viewModel.IsRemoved,
            };
        }

        public VmScope ToViewModel(Scope model)
        {
            if (model == null)
                return null;

            return new VmScope
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