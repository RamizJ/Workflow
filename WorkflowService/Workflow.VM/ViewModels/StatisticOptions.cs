using System;
using System.Collections.Generic;

namespace Workflow.VM.ViewModels
{
    public class StatisticOptions
    {
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }

        public List<int> ProjectIds { get; set; }
        public List<string> UserIds { get; set; }
    }

    public class UserStatisticOptions : StatisticOptions
    { }

    public class ProjectStatisticOptions : StatisticOptions
    { }
}