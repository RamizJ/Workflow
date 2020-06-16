using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmAttachmentConverter : IViewModelConverter<Attachment, VmAttachment>
    {
        public Attachment ToModel(VmAttachment viewModel)
        {
            return new Attachment
            {
                Id = viewModel.Id
            };
        }

        public VmAttachment ToViewModel(Attachment model)
        {
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
    }
}