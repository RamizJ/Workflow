using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Workflow.DAL.Models;

namespace WorkflowService.Services
{
    /// <summary>
    /// Преобразование файлов во вложения
    /// </summary>
    public interface IFormFilesService
    {
        /// <summary>Преобразование файлов во вложения</summary>
        /// <param name="files">Файлы</param>
        /// <returns>Коллекция вложений</returns>
        IEnumerable<Attachment> GetAttachments(IFormFileCollection files);
    }
}