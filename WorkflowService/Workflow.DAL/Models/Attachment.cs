using System;
using System.Collections.Generic;

namespace Workflow.DAL.Models
{
    public class Attachment
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public DateTime CreationDate { get; set; }

        public int FileDataId { get; set; }
        public FileData FileData { get; set; }
    }
}