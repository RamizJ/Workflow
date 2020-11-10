using System.Linq;
using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmGroupConverter : ViewModelConverter<Group, VmGroup>
    {
        public VmGroupConverter(IViewModelConverter<Project, VmProject> vmProjectConverter,
            IViewModelConverter<Metadata, VmMetadata> vmMetadataConverter)
        {
            _vmProjectConverter = vmProjectConverter;
            _vmMetadataConverter = vmMetadataConverter;
        }

        public override void SetModel(VmGroup viewModel, Group model)
        {
            if (model == null || viewModel == null)
                return;

            model.Id = viewModel.Id;
            model.Name = viewModel.Name;
            model.Description = viewModel.Description;
            model.ParentGroupId = viewModel.ParentGroupId;
        }

        public override void SetViewModel(Group model, VmGroup viewModel)
        {
            if (model == null || viewModel == null)
                return;

            viewModel.Id = model.Id;
            viewModel.Name = model.Name;
            viewModel.Description = model.Description;
            viewModel.ParentGroupId = model.ParentGroupId;
            viewModel.OwnerId = model.OwnerId;
            viewModel.OwnerFio = model.Owner?.Fio;
            viewModel.Projects = model.Projects?
                .Select(_vmProjectConverter.ToViewModel)
                .ToList();
            viewModel.ChildGroups = model.ChildGroups?
                .Select(ToViewModel)
                .ToList();
            viewModel.MetadataList = model.MetadataList?
                .Select(_vmMetadataConverter.ToViewModel)
                .ToList();
        }


        private readonly IViewModelConverter<Project, VmProject> _vmProjectConverter;
        private readonly IViewModelConverter<Metadata, VmMetadata> _vmMetadataConverter;
    }
}