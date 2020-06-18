using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.Services.Common;
using Workflow.VM.ViewModels;
using WorkflowService.Services;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API-методы работы с задачами
    /// </summary>
    //[Authorize]
    [ApiController, Route("api/[controller]/[action]")]
    public class GoalsController : ControllerBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public GoalsController(UserManager<ApplicationUser> userManager, 
            IGoalsService goalsService,
            IGoalAttachmentsService attachmentsService,
            IFormFilesService formFilesService)
        {
            _userManager = userManager;
            _service = goalsService;
            _attachmentsService = attachmentsService;
            _formFilesService = formFilesService;
        }

        /// <summary>
        /// Получить задачу по идентификатору, если она доступна пользователю
        /// </summary>
        /// <param name="id">Идентификатор задачи</param>
        /// <returns>Задача</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmGoal>> Get(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            return Ok(await _service.Get(user, id));
        }

        /// <summary>
        /// Постраничная загрузка задач с фильтрацией и сортировкой
        /// </summary>
        /// <param name="projectId">Идентификатор проекта. Если не указан, то загружаются задачи по всем проектам</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Фильтр по всем полям</param>
        /// <param name="filterFields">Конкретные поля фильтрации</param>
        /// <param name="sortFields">Поля сортировки</param>
        /// <param name="withRemoved">Вместе с удаленными</param>
        /// <returns>Коллеция задач</returns>
        [HttpGet]
        public async Task<IEnumerable<VmGoal>> GetPage([FromQuery]int? projectId, 
            [FromQuery]int pageNumber, [FromQuery]int pageSize, 
            [FromQuery]string filter = null, [FromQuery]FieldFilter[] filterFields = null, 
            [FromQuery]FieldSort[] sortFields = null, [FromQuery] bool withRemoved = false)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _service.GetPage(currentUser, projectId, 
                pageNumber, pageSize, filter, filterFields, sortFields, withRemoved);
        }

        /// <summary>
        /// Получение задач по идентификаторам. Возвращаются только задачи доступные пользователю
        /// </summary>
        /// <param name="ids">Идентификаторы задач</param>
        /// <returns>Коллекция задач</returns>
        [HttpGet]
        public async Task<IEnumerable<VmGoal>> GetRange([FromQuery]int[] ids)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _service.GetRange(currentUser, ids);
        }

        /// <summary>
        /// Создание задачи
        /// </summary>
        /// <param name="goal">Создаваемая задача</param>
        /// <returns>Созданная задача</returns>
        [HttpPost]
        public async Task<ActionResult<VmGoal>> Create([FromBody]VmGoal goal)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _service.Create(currentUser, goal);
        }

        /// <summary>
        /// Обновление задачи
        /// </summary>
        /// <param name="goal">Обновляемая задача</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]VmGoal goal)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            await _service.Update(currentUser, goal);
            return NoContent();
        }

        /// <summary>
        /// Удаление задачи
        /// </summary>
        /// <param name="id">Идентификатор удаляемой задачи</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<VmGoal>> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var deletedScope = await _service.Delete(currentUser, id);
            if (deletedScope == null)
                return NotFound();

            return Ok(deletedScope);
        }


        /// <summary>
        /// Получение вложений задачи
        /// </summary>
        /// <param name="goalId">Идентификатор задачи</param>
        /// <returns>Коллекция вложений</returns>
        [HttpGet("{goalId}")]
        public async Task<ActionResult<IEnumerable<VmGoal>>> GetAttachments(int goalId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var attachments = await _attachmentsService.GetAll(currentUser, goalId);
            return Ok(attachments);
        }


        /// <summary>
        /// Добавление вложения
        /// </summary>
        /// <param name="goalId">Идентификатор задачи</param>
        /// <param name="files">Коллекция файлов</param>
        /// <returns></returns>
        [HttpPatch("{goalId}")]
        public async Task<IActionResult> AddAttachments(int goalId, IFormFileCollection files)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var attachments = _formFilesService.GetAttachments(files);
            await _attachmentsService.Add(currentUser, goalId, attachments.ToArray());
            return NoContent();
        }

        /// <summary>
        /// Удаление вложений. Удаляются только те вложения тех задач, которые доступны пользователю
        /// </summary>
        /// <param name="attachmentIds">Идентификаторы вложений</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> RemoveAttachments([FromBody] IEnumerable<int> attachmentIds)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            await _attachmentsService.Remove(currentUser, attachmentIds);
            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        [HttpGet("{attachmentId}")]
        public async Task<FileResult> DownloadAttachmentFile(int attachmentId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var memoryStream = new MemoryStream();
            var attachment = await _attachmentsService.DowloadAttachmentFile(currentUser, memoryStream, attachmentId);
            memoryStream.Position = 0;
            return File(memoryStream, attachment.FileType ?? "*/*");
        }



        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGoalsService _service;
        private readonly IGoalAttachmentsService _attachmentsService;
        private readonly IFormFilesService _formFilesService;
    }
}