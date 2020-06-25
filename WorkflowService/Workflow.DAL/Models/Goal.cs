using System;
using System.Collections.Generic;

namespace Workflow.DAL.Models
{
    public class Goal
    {
        public int Id { get; set; }

        public int GoalNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? ParentGoalId { get; set; }
        public Goal ParentGoal { get; set; }
        public List<Goal> ChildGoals { get; set; } = new List<Goal>();

        public DateTime CreationDate { get; set; }
        public DateTime ExpectedCompletedDate { get; set; }
        public TimeSpan EstimatedPerformingTime { get; set; }

        public GoalState State { get; set; }
        public GoalPriority Priority { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public string PerformerId { get; set; }
        public ApplicationUser Performer { get; set; }

        public List<GoalObserver> Observers { get; set; } = new List<GoalObserver>();

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        public bool IsRemoved { get; set; }
    }

    /// <summary>
    /// Состояние выполнения задачи
    /// </summary>
    public enum GoalState
    {
        /// <summary>
        /// Новая
        /// </summary>
        New,

        /// <summary>
        /// Выполняется
        /// </summary>
        Perform,

        /// <summary>
        /// Приостановлена
        /// </summary>
        Delay,

        /// <summary>
        /// Тестируется
        /// </summary>
        Testing,
        
        /// <summary>
        /// Прошла тестирование и успешно завершена
        /// </summary>
        Succeed,

        /// <summary>
        /// Отклонена по результатам тестирования
        /// </summary>
        Rejected
    }

    /// <summary>
    /// Приоритет задачи
    /// </summary>
    public enum GoalPriority
    {   /// <summary>
        /// Нормальный 
        /// </summary>
        Normal,

        /// <summary>
        /// Низкий
        /// </summary>
        Low,

        /// <summary>
        /// Высокий
        /// </summary>
        High
    }
}