﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;
using Workflow.VM.ViewModels.Statistic;

namespace Workflow.Services.Abstract
{
    public interface ITotalStatisticService
    {
        Task<VmTotalStatistic> GetTotal(
            ApplicationUser currentUser, 
            StatisticOptions options);
    }
}