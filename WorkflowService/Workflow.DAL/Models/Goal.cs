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
        public List<Goal> ChildGoals { get; set; }

        public DateTime CreationDate { get; set; }
        private DateTime ExpectedCompletedDate { get; set; }
        private TimeSpan EstimatedPerformingTime { get; set; }

        public GoalState State { get; set; }
        public GoalProirity Priority { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public string PerformerId { get; set; }
        public ApplicationUser Performer { get; set; }

        public List<GoalObserver> Observers { get; set; }

        public int? AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

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
    public enum GoalProirity
    {
        /// <summary>
        /// Высокий
        /// </summary>
        Hight,

        /// <summary>
        /// Нормальный 
        /// </summary>
        Normal,

        /// <summary>
        /// Низкий
        /// </summary>
        Low
    }
}