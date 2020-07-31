using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;

namespace WorkflowService.Services.Abstract
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