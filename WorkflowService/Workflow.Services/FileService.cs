using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.Services.Abstract;

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
            var attachment = await _dataContext.FileData.FirstOrDefaultAsync(a => a.Id == fileDataId);
            if(attachment == null)
                throw new InvalidOperationException($"File with id='{fileDataId}' not found");

            await stream.WriteAsync(attachment.Data);
        }
    }
}