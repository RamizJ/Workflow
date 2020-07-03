using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.Common;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProjectsService
    {
        /// <summary>
        /// Получение проекта по идентификатору
        /// </summary>
        /// <param name="user">Текущий пользователь</param>
        /// <param name="id">Идентификатор проекта</param>
        /// <returns></returns>
        Task<VmProject> Get(ApplicationUser user, int id);

        /// <summary>
        /// Постраничная загрузка проектов с фильтрацией и сортировкой
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="pageOptions">Параметры страницы</param>
        /// <returns></returns>
        Task<IEnumerable<VmProject>> GetPage(ApplicationUser currentUser, PageOptions pageOptions);

        /// <summary>
        /// Загрузка проектов по идентфикаторам
        /// </summary>
        /// <param name="user">Текущий пользователь</param>
        /// <param name="ids">Идентификаторы проектов</param>
        /// <returns></returns>
        Task<IEnumerable<VmProject>> GetRange(ApplicationUser user, int[] ids);

        /// <summary>
        /// Создание проекта
        /// </summary>
        /// <param name="user">Текущий пользователь</param>
        /// <param name="project">Проект</param>
        /// <returns></returns>
        Task<VmProject> Create(ApplicationUser user, VmProject project);

        /// <summary>
        /// Создание проекта по форме
        /// </summary>
        /// <param name="user">Текущий пользователь</param>
        /// <param name="project">Форма проекта</param>
        /// <returns></returns>
        Task<VmProject> CreateByForm(ApplicationUser user, VmProjectForm project);

        /// <summary>
        /// Обновление проекта
        /// </summary>
        /// <param name="user"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        Task Update(ApplicationUser user, VmProject project);

        /// <summary>
        /// Обновление проекта по форме
        /// </summary>
        /// <param name="user">Текущий пользователь</param>
        /// <param name="projectForm">Форма проекта</param>
        /// <returns></returns>
        Task UpdateByForm(ApplicationUser user, VmProjectForm projectForm);

        /// <summary>
        /// Обновление проектов
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="projects">Формы проектов</param>
        /// <returns></returns>
        Task UpdateRange(ApplicationUser currentUser, IEnumerable<VmProject> projects);

        /// <summary>
        /// Обновление проектов по формам
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="projectForms">Формы проектов</param>
        /// <returns></returns>
        Task UpdateByFormRange(ApplicationUser currentUser, IEnumerable<VmProjectForm> projectForms);

        /// <summary>
        /// Удаление проектов
        /// </summary>
        /// <param name="user">Текущий пользователь</param>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <returns></returns>
        Task<VmProject> Delete(ApplicationUser user, int projectId);

        /// <summary>
        /// Удаление проектов
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="ids">Идентификаторы проектов</param>
        /// <returns></returns>
        Task<IEnumerable<VmProject>> DeleteRange(ApplicationUser currentUser, IEnumerable<int> ids);

        /// <summary>
        /// Восстановление ранее удаленного проекта
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <returns></returns>
        Task<VmProject> Restore(ApplicationUser currentUser, int projectId);

        /// <summary>
        /// Восстановление проектов
        /// </summary>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <param name="ids">Идентификаторы проектов</param>
        /// <returns></returns>
        Task<IEnumerable<VmProject>> RestoreRange(ApplicationUser currentUser, IEnumerable<int> ids);
    }
}