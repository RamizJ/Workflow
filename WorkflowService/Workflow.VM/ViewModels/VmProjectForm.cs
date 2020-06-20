using System.Collections.Generic;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels
{
    /// <summary>
    /// Модель создания / обновления проекта
    /// </summary>
    public class VmProjectForm
    {
        /// <summary>
        /// Проект
        /// </summary>
        public VmProject Project { get; set; }


        /// <summary>
        /// Идентификаторы команд
        /// </summary>
        public IEnumerable<int> TeamIds { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public VmProjectForm()
        { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="project">Проект</param>
        /// <param name="teamIds">Идентификаторы команд</param>
        public VmProjectForm(VmProject project, IEnumerable<int> teamIds)
        {
            Project = project;
            TeamIds = teamIds;
        }
    }
}