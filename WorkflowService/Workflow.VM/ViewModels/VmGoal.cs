﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels
{
    public class VmGoal
    {
        public int Id { get; set; }

        public int GoalNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int ScopeId { get; set; }
        public int? ParentGoalId { get; set; }

        public DateTime CreationDate { get; set; }
        private DateTime ExpectedCompletedDate { get; set; }
        private TimeSpan EstimatedPerformingTime { get; set; }

        [EnumDataType(typeof(GoalState))]
        [JsonConverter(typeof(StringEnumConverter))]
        public GoalState GoalState { get; set; }

        public string OwnerId { get; set; }
        public string PerformerId { get; set; }
        public List<string> Observers { get; set; }

        public int? AttachmentId { get; set; }
        public bool IsRemoved { get; set; }
    }
}
