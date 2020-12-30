using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Workflow.DAL.Models;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Services
{
    /// <inheritdoc />
    public class FormFilesService : IFormFilesService
    {
        /// <inheritdoc />
        public IEnumerable<Attachment> GetAttachments(IFormFileCollection files)
        {
            foreach (IFormFile file in files.Where(f => f.Length < MB_5))
            {
                var fileData = new FileData();

                using var binaryReader = new BinaryReader(file.OpenReadStream());
                fileData.Data = binaryReader.ReadBytes((int)file.Length);

                yield return new Attachment
                {
                    FileName = file.FileName,
                    FileSize = file.Length,
                    FileType = file.ContentType,
                    FileData = fileData,
                    CreationDate = DateTime.Now.ToUniversalTime()
                };
            }
        }

        private const int MB_5 = 5 * 1024 * 1024;
    }
}