using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmAttachmentConverter : IViewModelConverter<Attachment, VmAttachment>
    {
        public Attachment ToModel(VmAttachment viewModel)
        {
            return new Attachment { Id = viewModel.Id };
        }

        public VmAttachment ToViewModel(Attachment model)
        {
            if (model == null)
                return null;

            return new VmAttachment
            {
                Id = model.Id,
                FileDataId = model.FileDataId,
                CreationDate = model.CreationDate,
                FileName = model.FileName,
                FileSize = model.FileSize,
                FileType = model.FileType
            };
        }

        public void SetModel(VmAttachment viewModel, Attachment model)
        { }

        public void SetViewModel(Attachment model, VmAttachment viewModel)
        {
            if (model == null || viewModel == null)
                return;

            viewModel.Id = model.Id;
            viewModel.FileDataId = model.FileDataId;
            viewModel.CreationDate = model.CreationDate;
            viewModel.FileName = model.FileName;
            viewModel.FileSize = model.FileSize;
            viewModel.FileType = model.FileType;
        }
    }
}