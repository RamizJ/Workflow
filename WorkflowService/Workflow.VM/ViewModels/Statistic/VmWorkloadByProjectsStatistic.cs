using System.Collections.Generic;

namespace Workflow.VM.ViewModels.Statistic
{
    /// <summary>
    /// Статистика нагрузки по проектам
    /// </summary>
    public class VmWorkloadByProjectsStatistic
    {
        /// <summary>
        /// Всего часов по всем проектам
        /// </summary>
        public double TotalHours { get; set; }

        /// <summary>
        /// Всего часов на каждый проект (идентификатор проекта - кол-во часов)
        /// </summary>
        public IDictionary<int, double> ProjectHours { get; set; }
    }
}