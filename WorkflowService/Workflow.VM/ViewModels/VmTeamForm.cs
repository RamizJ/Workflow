using System.Collections.Generic;

namespace Workflow.VM.ViewModels
{
    /// <summary>
    /// Форма создания / удаления команды
    /// </summary>
    public class VmTeamForm
    {
        /// <summary>
        /// Команда
        /// </summary>
        public VmTeam Team { get; set; }
        
        /// <summary>
        /// Идентификаторы участников команды
        /// </summary>
        public List<string> UserIds { get; set; }

        /// <summary>
        /// Идентификаторы проектов
        /// </summary>
        public List<int> ProjectIds { get; set; }

        public VmTeamForm()
        { }

        public VmTeamForm(VmTeam vmTeam, List<string> teamUserIds = null, List<int> projectIds = null)
        {
            Team = vmTeam;
            UserIds = teamUserIds;
            ProjectIds = projectIds;
        }
    }
}