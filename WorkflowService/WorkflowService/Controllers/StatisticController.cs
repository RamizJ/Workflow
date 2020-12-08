using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;
using Workflow.VM.ViewModels.Statistic;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API работы со статистикой
    /// </summary>
    //[Authorize]
    [ApiController, Route("api/[controller]/[action]")]
    public class StatisticController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="goalCompletionStatisticService"></param>
        /// <param name="projectWorkloadStatisticService"></param>
        /// <param name="daysWorkloadStatisticService"></param>
        /// <param name="totalStatisticService"></param>
        /// <param name="currentUserService"></param>
        public StatisticController(
            IGoalCompletionStatisticService goalCompletionStatisticService,
            IWorkloadForProjectStatisticService projectWorkloadStatisticService,
            IWorkloadByDaysStatisticService daysWorkloadStatisticService,
            ITotalStatisticService totalStatisticService,
            ICurrentUserService currentUserService)
        {
            _goalCompletionStatisticService = goalCompletionStatisticService;
            _projectWorkloadStatisticService = projectWorkloadStatisticService;
            _daysWorkloadStatisticService = daysWorkloadStatisticService;
            _totalStatisticService = totalStatisticService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Получение статистики выполненеия задач по пользователям
        /// </summary>
        /// <param name="options">Параметры статистики</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task<IDictionary<string, VmGoalCompletionStatistic>> GetGoalCompletion([FromBody]StatisticOptions options)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _goalCompletionStatisticService.GetGoalCompletion(currentUser, options);
        }

        /// <summary>
        /// Получение статистики загрузки пользователей по проектам 
        /// </summary>
        /// <param name="options">Параметры статистики</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IDictionary<string, VmWorkloadByProjectsStatistic>> GetWorkloadByProject([FromBody] StatisticOptions options)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _projectWorkloadStatisticService.GetWorkloadByProject(currentUser, options);
        }

        /// <summary>
        /// Получение статистики загрузки пользователей по дням
        /// </summary>
        /// <param name="options">Параметры статистики</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IDictionary<string, VmWorkloadByDaysStatistic>> GetWorkloadByDays([FromBody] StatisticOptions options)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _daysWorkloadStatisticService.GetWorkloadByDays(currentUser, options);
        }

        /// <summary>
        /// Получение полной статистики
        /// </summary>
        /// <param name="options">Параметры статистики</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IDictionary<string, VmTotalStatistic>> GetTotal([FromBody] StatisticOptions options)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _totalStatisticService.GetTotal(currentUser, options);
        }


        private readonly IGoalCompletionStatisticService _goalCompletionStatisticService;
        private readonly IWorkloadForProjectStatisticService _projectWorkloadStatisticService;
        private readonly IWorkloadByDaysStatisticService _daysWorkloadStatisticService;
        private readonly ITotalStatisticService _totalStatisticService;
        private readonly ICurrentUserService _currentUserService;
    }
}