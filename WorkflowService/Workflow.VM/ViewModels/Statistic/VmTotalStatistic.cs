namespace Workflow.VM.ViewModels.Statistic
{
    /// <summary>
    /// Полная статистика по проекту
    /// </summary>
    public class VmTotalStatistic
    {
        /// <summary>
        /// Статистика выполнения задач
        /// </summary>
        public VmGoalCompletionStatistic GoalCompletion { get; set; }

        /// <summary>
        /// Статистика нагруженности по проектам
        /// </summary>
        public VmWorkloadByProjectsStatistic WorkloadByProjects { get; set; }

        /// <summary>
        /// Статистика нагруженности по дням
        /// </summary>
        public VmWorkloadByDaysStatistic WorkloadByDays { get; set; }
    }
}