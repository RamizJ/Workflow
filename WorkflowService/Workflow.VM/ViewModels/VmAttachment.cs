using System;

namespace Workflow.VM.ViewModels
{
    public class VmAttachment
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public long FileSize { get; set; }

        public string FileType { get; set; }

        public DateTime CreationDate { get; set; }

        public int FileDataId { get; set; }
    }
}