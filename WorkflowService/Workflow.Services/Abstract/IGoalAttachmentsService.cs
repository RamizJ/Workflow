using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// Сервис работы с вложениями задач
    /// </summary>
    public interface IGoalAttachmentsService
    {
        /// <summary>
        /// Получение всех вложений задачи
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="goalId">Идентификатор задачи</param>
        /// <returns>Коллекция вложений</returns>
        Task<IEnumerable<VmAttachment>> GetAll(ApplicationUser currentUser, int goalId);

        /// <summary>
        /// Добавление вложений задачи
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="goalId">Идентификатор задачи</param>
        /// <param name="attachments">Вложения</param>
        /// <returns></returns>
        Task Add(ApplicationUser currentUser, int goalId, ICollection<Attachment> attachments);


        /// <summary>
        /// Удаление вложений
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="attachmentIds">Идентификаторы вложений</param>
        /// <returns></returns>
        Task Remove(ApplicationUser currentUser, IEnumerable<int> attachmentIds);

        /// <summary>
        /// Загрузка файла вложения
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="stream">Поток файла</param>
        /// <param name="attachmentId">Идентификатор вложения</param>
        /// <returns></returns>
        Task<Attachment> DownloadAttachmentFile(ApplicationUser currentUser, Stream stream, int attachmentId);
    }
}