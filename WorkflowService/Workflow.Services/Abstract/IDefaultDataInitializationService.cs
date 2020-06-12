using System.Threading.Tasks;

namespace Workflow.Services.Abstract
{
    /// <summary>
    /// Сервис инициализации БД начальными данными
    /// </summary>
    public interface IDefaultDataInitializationService
    {
        /// <summary>
        /// Инициализация ролей
        /// </summary>
        /// <returns></returns>
        Task InitializeRoles();

        /// <summary>
        /// Инициализация администратора
        /// </summary>
        /// <returns></returns>
        Task InitializeAdmin();
    }
}