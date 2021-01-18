using Workflow.DAL.Migrations;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels
{
    public class VmGoalState
    {
        public int GoalId { get; set; }
        
        /// <summary>
        /// Статус выполнения задачи
        /// </summary>
        public GoalState GoalState { get; set; }
        
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Примерное время выполнения в часах
        /// </summary>
        public double? EstimatedPerformingHours { get; set; }
        
        /// <summary>
        /// Фаактическое время выполнения в часах 
        /// </summary>
        public double? ActualPerformingHours { get; set; }
    }
}