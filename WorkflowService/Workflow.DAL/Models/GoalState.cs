namespace Workflow.DAL.Models
{
    /// <summary>
    /// Состояние выполнения задачи
    /// </summary>
    public enum GoalState
    {
        /// <summary>
        /// Новая
        /// </summary>
        New,

        /// <summary>
        /// Выполняется
        /// </summary>
        Perform,

        /// <summary>
        /// Приостановлена
        /// </summary>
        Delay,

        /// <summary>
        /// Тестируется
        /// </summary>
        Testing,
        
        /// <summary>
        /// Прошла тестирование и успешно завершена
        /// </summary>
        Succeed,

        /// <summary>
        /// Отклонена по результатам тестирования
        /// </summary>
        Rejected
    }
}