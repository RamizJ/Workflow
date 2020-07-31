using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class FileService : IFileService
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataContext">Контекст БД</param>
        public FileService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <inheritdoc />
        public async Task Download(Stream stream, int fileDataId)
        {
            var fileData = await _dataContext.FileData.FirstOrDefaultAsync(a => a.Id == fileDataId);
            if(fileData == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest, 
                    $"File with id='{fileDataId}' not found");

            await stream.WriteAsync(fileData.Data);
        }
    }
}