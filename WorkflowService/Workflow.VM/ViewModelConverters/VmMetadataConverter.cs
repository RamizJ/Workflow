using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmMetadataConverter : ViewModelConverter<Metadata, VmMetadata>
    {
        public override void SetModel(VmMetadata viewModel, Metadata model)
        {
            if (viewModel == null || model == null)
                return;

            model.Key = viewModel.Key;
            model.Value = viewModel.Value;
        }

        public override void SetViewModel(Metadata model, VmMetadata viewModel)
        {
            viewModel.Key = model.Key;
            viewModel.Value = model.Value;
        }
    }
}