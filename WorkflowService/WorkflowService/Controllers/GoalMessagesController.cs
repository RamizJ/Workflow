using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PageLoading;
using Workflow.Services.Exceptions;
using Workflow.VM.ViewModels;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class GoalMessagesController : ControllerBase
    {
        /// <summary>
        /// Получить сообщение по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сообщения</param>
        /// <returns>Сообщение</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmGoalMessage>> Get(int id)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented, "Method not implemented");
        }

        /// <summary>
        /// Постраничная загрузка сообщений с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageOptions">Параметры загружаемой страницы</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<VmGoalMessage>> GetPage(
            [FromBody] PageOptions pageOptions)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented, "Method not implemented");
        }

        /// <summary>
        /// Постраничная загрузка непрочитанных сообщений с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageOptions">Параметры загружаемой страницы</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<VmGoalMessage>> GetUnreadPage(
            [FromBody] PageOptions pageOptions)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented, "Method not implemented");
        }

        /// <summary>
        /// Получение сообщений по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы сообщений</param>
        /// <returns>Коллекция сообщений</returns>
        [HttpGet]
        public async Task<IEnumerable<VmGoalMessage>> GetRange([FromQuery] int[] ids)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented, "Method not implemented");
        }

        /// <summary>
        /// Создание сообщения
        /// </summary>
        /// <param name="message">Создаваемое сообщение</param>
        /// <returns>Созданная сообщение</returns>
        [HttpPost]
        public async Task<ActionResult<VmGoalMessage>> Create([FromBody] VmGoalMessage message)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented, "Method not implemented");
        }

        /// <summary>
        /// Обновление сообщения
        /// </summary>
        /// <param name="message">Обновляемое сообщение</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VmGoalMessage message)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented, "Method not implemented");
        }

        /// <summary>
        /// Обновление сообщений
        /// </summary>
        /// <param name="messages">Обновляемые сообщения</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<VmGoalMessage> messages)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented, "Method not implemented");
        }

        /// <summary>
        /// Удаление сообщения
        /// </summary>
        /// <param name="id">Идентификатор удаляемого сообщения</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<VmGoalMessage>> Delete(int id)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented, "Method not implemented");
        }

        /// <summary>
        /// Удаление сообщений
        /// </summary>
        /// <param name="ids">Идентификаторы сообщений</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<VmGoalMessage>> DeleteRange([FromBody] IEnumerable<int> ids)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented, "Method not implemented");
        }
    }
}