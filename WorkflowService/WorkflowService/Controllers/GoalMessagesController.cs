using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PageLoading;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class GoalMessagesController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUserService"></param>
        /// <param name="goalMessageService"></param>
        public GoalMessagesController(ICurrentUserService currentUserService, 
            IGoalMessageService goalMessageService)
        {
            _currentUserService = currentUserService;
            _goalMessageService = goalMessageService;
        }

        /// <summary>
        /// Получить сообщение по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сообщения</param>
        /// <returns>Сообщение</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmGoalMessage>> Get(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _goalMessageService.Get(currentUser, id);
        }

        /// <summary>
        /// Постраничная загрузка сообщений задачи с фильтрацией и сортировкой
        /// </summary>
        /// <param name="goalId">Идентификатор задачи. Null - получение сообщений всех задач</param>
        /// <param name="pageOptions">Параметры загружаемой страницы</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<VmGoalMessage>> GetPage(int? goalId,
            [FromBody] PageOptions pageOptions)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _goalMessageService.GetPage(currentUser, goalId, pageOptions);
        }

        /// <summary>
        /// Постраничная загрузка непрочитанных сообщений с фильтрацией и сортировкой
        /// </summary>
        /// <param name="goalId">Идентификатор задачи. Null - получение сообщений всех задач</param>
        /// <param name="pageOptions">Параметры загружаемой страницы</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<VmGoalMessage>> GetUnreadPage(int? goalId,
            [FromBody] PageOptions pageOptions)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _goalMessageService.GetUnreadPage(currentUser, goalId, pageOptions);
        }

        /// <summary>
        /// Получение сообщений по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы сообщений</param>
        /// <returns>Коллекция сообщений</returns>
        [HttpGet]
        public async Task<IEnumerable<VmGoalMessage>> GetRange([FromQuery] int[] ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _goalMessageService.GetRange(currentUser, ids);
        }

        /// <summary>
        /// Создание сообщения
        /// </summary>
        /// <param name="message">Создаваемое сообщение</param>
        /// <returns>Созданная сообщение</returns>
        [HttpPost]
        public async Task<VmGoalMessage> Create([FromBody] VmGoalMessage message)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _goalMessageService.Create(currentUser, message);
        }

        /// <summary>
        /// Обновление сообщения
        /// </summary>
        /// <param name="message">Обновляемое сообщение</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VmGoalMessage message)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _goalMessageService.Update(currentUser, message);
            return NoContent();
        }

        /// <summary>
        /// Обновление сообщения
        /// </summary>
        /// <param name="messageIds">Идентификаторы сообщений</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> MarkAsRead([FromBody] IEnumerable<int> messageIds)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _goalMessageService.MarkAsRead(currentUser, messageIds);
            return NoContent();
        }

        /// <summary>
        /// Удаление сообщения
        /// </summary>
        /// <param name="id">Идентификатор удаляемого сообщения</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<VmGoalMessage> Delete(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _goalMessageService.Delete(currentUser, id);
        }

        /// <summary>
        /// Удаление сообщений
        /// </summary>
        /// <param name="ids">Идентификаторы сообщений</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IEnumerable<VmGoalMessage>> DeleteRange([FromBody] IEnumerable<int> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _goalMessageService.DeleteRange(currentUser, ids);
        }

        /// <summary>
        /// Удаление сообщения
        /// </summary>
        /// <param name="id">Идентификатор удаляемого сообщения</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<VmGoalMessage> Restore(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _goalMessageService.Restore(currentUser, id);
        }

        /// <summary>
        /// Удаление сообщений
        /// </summary>
        /// <param name="ids">Идентификаторы сообщений</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IEnumerable<VmGoalMessage>> RestoreRange([FromBody] IEnumerable<int> ids)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _goalMessageService.RestoreRange(currentUser, ids);
        }


        private readonly ICurrentUserService _currentUserService;
        private readonly IGoalMessageService _goalMessageService;
    }
}