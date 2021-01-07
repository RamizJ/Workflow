using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL.Models;

namespace Workflow.DAL.Repositories.Abstract
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Получение всех пользователей проектов, которым принадлежат задачи
        /// </summary>
        /// <param name="goals">Исходная коллекция задач, по которой будет проводиться поиск</param>
        /// <param name="goalIds">Идентификаторы задач проектов для которых будут возвращаться пользователи</param>
        /// <returns></returns>
        IQueryable<string> GetUserIdsForGoalsProjects(
            IQueryable<Goal> goals,
            IEnumerable<int> goalIds);

        /// <summary>
        /// Получение всех пользователей учавствующих в проектах
        /// </summary>
        /// <param name="projects">Исходная коллекция проектов, по которой будет проводиться поиск</param>
        /// <param name="projectIds">Идентификаторы проектов для которых будут возвращаться пользователи</param>
        /// <returns></returns>
        IQueryable<string> GetUserIdsForProjects(
            IQueryable<Project> projects, 
            IEnumerable<int> projectIds);

        /// <summary>
        /// Получение всех пользователей состоящих в командах указанных пользователей
        /// </summary>
        /// <param name="users">Исходная коллекция пользователей, по которой будет проводиться поиск</param>
        /// <param name="userIds">Идентификаторы пользователей</param>
        /// <returns></returns>
        IQueryable<string> GetTeamMemberIdsForUsers(
            IQueryable<ApplicationUser> users, 
            IEnumerable<string> userIds);

        /// <summary>
        /// Получение всех пользователей состоящих в проектах указанных групп
        /// </summary>
        /// <param name="groups">Исходная коллекция групп, по которой будет проводиться поиск</param>
        /// <param name="groupIds">Идентификаторы групп</param>
        /// <returns></returns>
        IQueryable<string> GetProjectUserIdsForGroups(
            IQueryable<Group> groups, 
            IEnumerable<int> groupIds);

        /// <summary>
        /// Получение всех пользователей состоящих в командах
        /// </summary>
        /// <param name="teams">Исходная коллекция команд, по которой будет проводиться поиск</param>
        /// <param name="teamIds">Идентификаторы команд</param>
        /// <returns></returns>
        IQueryable<string> GetUserIdsForTeams(
            IQueryable<Team> teams, 
            IEnumerable<int> teamIds);
    }
}