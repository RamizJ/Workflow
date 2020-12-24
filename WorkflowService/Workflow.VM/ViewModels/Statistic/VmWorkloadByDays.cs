using System;
using System.Collections.Generic;

namespace Workflow.VM.ViewModels.Statistic
{
    /// <summary>
    /// Рабочая нагрузка по дням
    /// </summary>
    public class VmWorkloadByDaysStatistic
    {
        /// <summary>
        /// Список рабочих нагрузок по дням
        /// </summary>
        public IDictionary<DateTime, VmDayWorkload> DayWorkloads { get; set; }
    }
}