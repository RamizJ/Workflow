namespace Workflow.VM.ViewModels.Statistic
{
    /// <summary>
    /// Рабочая нагрузка за день
    /// </summary>
    public class VmDayWorkload
    {
        /// <summary>
        /// Рабочая нагрузка по выбранным проектам
        /// </summary>
        public double SelectedProjectsWorkload { get; set; }

        /// <summary>
        /// Рабочая нагрузка по остальным проектам
        /// </summary>
        public double OtherProjectsWorkload { get; set; }

        /// <summary>
        /// Рабочая нагрузка по всем проектам
        /// </summary>
        public double TotalWorkload { get; set; }
    }
}