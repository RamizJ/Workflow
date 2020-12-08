namespace Workflow.VM.ViewModels.Statistic
{
    /// <summary>
    /// Статистика выполнения задач
    /// </summary>
    public class VmGoalCompletionStatistic
    {
        /// <summary>
        /// Выполнено вовремя
        /// </summary>
        public double CompletedOnTime { get; set; }

        /// <summary>
        /// Выполнено не вовремя
        /// </summary>
        public double CompletedNotOnTime { get; set; }

        /// <summary>
        /// В процессе выполнения
        /// </summary>
        public double InProcess { get; set; }

        /// <summary>
        /// Не завершено
        /// </summary>
        public double NotCompleted { get; set; }
    }
}