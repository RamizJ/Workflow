using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PageLoading;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API-методы работы с группами проектов
    /// </summary>
    [Authorize]
    [ApiController, Route("api/[controller]/[action]")]
    public class GroupsController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupsService"></param>
        /// <param name="currentUserService"></param>
        public GroupsController(IGroupsService groupsService,
            ICurrentUserService currentUserService)
        {
            _groupsService = groupsService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Получить группу проектов по идентификатору, если она доступна пользователю
        /// </summary>
        /// <param name="id">Идентификатор задачи</param>
        /// <returns>Группа</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmGroup>> Get(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return Ok(await _groupsService.Get(currentUser, id));
        }
        
        /// <summary>
        /// Получить все группы проектов
        /// </summary>
        /// <returns>Группа</returns>
        [HttpGet]
        public async Task<IEnumerable<VmGroup>> GetAll()
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _groupsService.GetAll(currentUser);
        }

        /// <summary>
        /// Постраничная загрузка группы проектов с фильтрацией и сортировкой
        /// </summary>
        /// <param name="parentGroupId">Идентификатор родительской группы</param>
        /// <param name="pageOptions">Параметры загружаемой страницы</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<VmGroup>> GetPage(
            [FromQuery] int? parentGroupId,
            [FromBody] PageOptions pageOptions)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var page = await _groupsService.GetPage(currentUser, parentGroupId, pageOptions);
            return page;
        }


        /// <summary>
        /// Создание группы проектов
        /// </summary>
        /// <param name="group">Создаваемая группа проектов</param>
        /// <returns>Созданная группа проектов</returns>
        [HttpPost]
        public async Task<ActionResult<VmGroup>> Create([FromBody] VmGroup group)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return Ok(await _groupsService.Create(currentUser, group));
        }

        /// <summary>
        /// Обновление группы проектов
        /// </summary>
        /// <param name="group">Обновляемая группа проектов</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VmGroup group)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _groupsService.Update(currentUser, group);
            return NoContent();
        }

        /// <summary>
        /// Обновление группы проектов
        /// </summary>
        /// <param name="groups">Обновляемые группы проектов</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<VmGroup> groups)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _groupsService.UpdateRange(currentUser, groups);
            return NoContent();
        }

        /// <summary>
        /// Удаление группы проектов
        /// </summary>
        /// <param name="id">Идентификатор удаляемой группы проектов</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<VmGroup>> Delete(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var deletedgroup = await _groupsService.Delete(currentUser, id);
            if (deletedgroup == null)
                return NotFound();

            return Ok(deletedgroup);
        }

        /// <summary>
        /// Удаление групп проектов
        /// </summary>
        /// <param name="ids">Идентификаторы групп проектов</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<VmGroup>> DeleteRange([FromBody] IEnumerable<int> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var deletedgroup = await _groupsService.DeleteRange(currentUser, ids);
            if (deletedgroup == null)
                return NotFound();

            return Ok(deletedgroup);
        }

        /// <summary>
        /// Восстановление группы проектов
        /// </summary>
        /// <param name="id">Идентификатор восстанавливаемой группы проектов</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<VmGroup>> Restore(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var deletedgroup = await _groupsService.Restore(currentUser, id);
            if (deletedgroup == null)
                return NotFound();

            return Ok(deletedgroup);
        }

        /// <summary>
        /// Восстановление групп проектов
        /// </summary>
        /// <param name="ids">Идентификаторы групп проектов</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<VmGroup>> RestoreRange([FromBody] IEnumerable<int> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var deletedgroup = await _groupsService.RestoreRange(currentUser, ids);
            if (deletedgroup == null)
                return NotFound();

            return Ok(deletedgroup);
        }

        /// <summary>
        /// Добавление проектов в группу
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <param name="projectIds">Идентификаторы проектов</param>
        /// <returns></returns>
        [HttpPost("{groupId}")]
        public async Task AddProjects(int groupId, [FromBody]ICollection<int> projectIds)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _groupsService.AddProjects(currentUser, groupId, projectIds);
        }

        /// <summary>
        /// Удаление проектов из группы
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <param name="projectIds">Идентификаторы проектов</param>
        /// <returns></returns>
        [HttpPost("{groupId}")]
        public async Task DeleteProjects(int groupId, [FromBody] ICollection<int> projectIds)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _groupsService.RemoveProjects(currentUser, groupId, projectIds);
        }


        private readonly IGroupsService _groupsService;
        private readonly ICurrentUserService _currentUserService;
    }
}