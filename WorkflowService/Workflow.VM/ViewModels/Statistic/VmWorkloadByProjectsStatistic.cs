using System.Collections.Generic;

namespace Workflow.VM.ViewModels.Statistic
{
    /// <summary>
    /// Статистика нагрузки по проектам
    /// </summary>
    public class VmWorkloadByProjectsStatistic
    {
        /// <summary>
        /// Всего часов по всем проектам
        /// </summary>
        public double TotalHours { get; set; }

        /// <summary>
        /// Кол-во часов участия пользователей в проектах  
        /// </summary>
        public List<VmProjectHoursForUser> ProjectHoursForUsers { get; set; } = new List<VmProjectHoursForUser>();
    }

    public class VmProjectHoursForUser
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Кол-во часов участия пользователя в проекте
        /// </summary>
        public List<VmHoursForProject> HoursForProject { get; set; } = new List<VmHoursForProject>();

        public VmProjectHoursForUser()
        { }

        public VmProjectHoursForUser(string userId)
        {
            UserId = userId;
        }
    }

    public class VmHoursForProject
    {
        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Имя проекта
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Кол-во часов
        /// </summary>
        public double Hours { get; set; }


        public VmHoursForProject()
        { }

        public VmHoursForProject(int projectId, string projectName, double hours)
        {
            ProjectId = projectId;
            ProjectName = projectName;
            Hours = hours;
        }
    }
}