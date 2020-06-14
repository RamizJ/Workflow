﻿namespace Workflow.DAL.Models
{
    public class ProjectTeam
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }


        public ProjectTeam()
        { }

        public ProjectTeam(int projectId, int teamId)
        {
            ProjectId = projectId;
            TeamId = teamId;
        }
    }
}