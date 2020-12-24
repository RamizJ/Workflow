using System.Collections.Generic;

namespace Workflow.VM.ViewModels.Statistic
{
    /// <summary>
    /// Статистика выполнения задач
    /// </summary>
    public class VmGoalCompletionStatistic
    {
        public List<VmUserGoalsCompletion> UserGoalsCompletions { get; set; }
            = new List<VmUserGoalsCompletion>();
    }

    /// <summary>
    /// Статистика выполнения задач пользователя
    /// </summary>
    public class VmUserGoalsCompletion
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Статистика выполнения задач
        /// </summary>
        public VmGoalsCompletion GoalsCompletion { get; set; }
    }

    /// <summary>
    /// Статистика выполнения задач
    /// </summary>
    public class VmGoalsCompletion
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