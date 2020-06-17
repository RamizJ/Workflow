using System.IO;
using System.Threading.Tasks;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// Сервис работы с файлами
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Загрузка файла по идентификатору
        /// </summary>
        /// <param name="stream">Поток в который будет загружен файл</param>
        /// <param name="fileDataId">Идентификатор файла</param>
        /// <returns></returns>
        public Task Download(Stream stream, int fileDataId);
    }
}