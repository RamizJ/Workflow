using System;
using System.Collections.Generic;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels
{
    public class VmGoal
    {
        public int Id { get; set; }

        public int GoalNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        public int? ParentGoalId { get; set; }
        //public List<int> ChildGoals { get; set; }
        public List<VmGoal> Children { get; set; }
        public List<string> ObserverIds { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreationDate { get; set; }
        
        /// <summary>
        /// Ожидаемые дата и время завершения
        /// </summary>
        public DateTime? ExpectedCompletedDate { get; set; }
        
        /// <summary>
        /// Фаактическое время выполнения в часах 
        /// </summary>
        public double? ActualPerformingHours { get; set; }
        
        /// <summary>
        /// Примерное время выполнения в часах
        /// </summary>
        public double? EstimatedPerformingHours { get; set; }

        /// <summary>
        /// Статус задачи
        /// </summary>
        public GoalState State { get; set; }
        
        /// <summary>
        /// Приоритет задачи
        /// </summary>
        public GoalPriority Priority { get; set; }

        public string OwnerId { get; set; }
        public string OwnerFio { get; set; }
        public string PerformerId { get; set; }
        public string PerformerFio { get; set; }

        public bool IsRemoved { get; set; }

        public bool HasChildren { get; set; }
        public bool IsAttachmentsExist { get; set; }

        public List<VmMetadata> MetadataList { get; set; }
    }
}
