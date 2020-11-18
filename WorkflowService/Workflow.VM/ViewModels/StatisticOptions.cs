using System;

namespace Workflow.VM.ViewModels
{
    public class StatisticOptions
    {
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
    }

    public class UserStatisticOptions : StatisticOptions
    { }

    public class ProjectStatisticOptions : StatisticOptions
    { }
}