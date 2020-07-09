using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
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
            IFormFilesService formFilesService,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _service = goalsService;
            _attachmentsService = attachmentsService;
            _formFilesService = formFilesService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Получить задачу по идентификатору, если она доступна пользователю
        /// </summary>
        /// <param name="id">Идентификатор задачи</param>
        /// <returns>Задача</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VmGoal>> Get(int id)
        {
            var currentUser = await _currentUserService.Get(User);
            return Ok(await _service.Get(currentUser, id));
        }

        /// <summary>
        /// Постраничная загрузка задач с фильтрацией и сортировкой
        /// </summary>
        /// <returns>Коллеция задач</returns>
        [HttpPost]
        public async Task<IEnumerable<VmGoal>> GetPage([FromQuery]int? projectId, 
            [FromBody] PageOptions pageOptions)
        {
            var currentUser = await _currentUserService.Get(User);
            var page = await _service.GetPage(currentUser, projectId, pageOptions);
            return page;
        }

        /// <summary>
        /// Получение задач по идентификаторам. Возвращаются только задачи доступные пользователю
        /// </summary>
        /// <param name="ids">Идентификаторы задач</param>
        /// <returns>Коллекция задач</returns>
        [HttpGet]
        public async Task<IEnumerable<VmGoal>> GetRange([FromQuery]int[] ids)
        {
            var currentUser = await _currentUserService.Get(User);
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
            var currentUser = await _currentUserService.Get(User);
            return await _service.Create(currentUser, goal);
        }

        /// <summary>
        /// Создание задачи по форме
        /// </summary>
        /// <param name="goalForm">Форма создаваемой задачи</param>
        /// <returns>Созданная задача</returns>
        [HttpPost]
        public async Task<ActionResult<VmGoal>> CreateByForm([FromBody] VmGoalForm goalForm)
        {
            var currentUser = await _currentUserService.Get(User);
            return await _service.CreateByForm(currentUser, goalForm);
        }

        /// <summary>
        /// Обновление задачи
        /// </summary>
        /// <param name="goal">Обновляемая задача</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]VmGoal goal)
        {
            var currentUser = await _currentUserService.Get(User);
            await _service.Update(currentUser, goal);
            return NoContent();
        }

        /// <summary>
        /// Обновление задач
        /// </summary>
        /// <param name="goals">Обновляемые задачи</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateRange([FromBody] IEnumerable<VmGoal> goals)
        {
            var currentUser = await _currentUserService.Get(User);
            await _service.UpdateRange(currentUser, goals);
            return NoContent();
        }

        /// <summary>
        /// Обновление задачи по форме
        /// </summary>
        /// <param name="goalForm">Форма обновляемой задачи</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateByForm([FromBody] VmGoalForm goalForm)
        {
            var currentUser = await _currentUserService.Get(User);
            await _service.UpdateByForm(currentUser, goalForm);
            return NoContent();
        }

        /// <summary>
        /// Обновление задач по формам
        /// </summary>
        /// <param name="goalForms">Формы обновляемых задач</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateByFormRange([FromBody] IEnumerable<VmGoalForm> goalForms)
        {
            var currentUser = await _currentUserService.Get(User);
            await _service.UpdateByFormRange(currentUser, goalForms);
            return NoContent();
        }

        /// <summary>
        /// Обновление задач по формам
        /// </summary>
        /// <param name="goalForm">Форма обновляемой задачи</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateByFormRange([FromBody] VmGoalForm goalForm)
        {
            var currentUser = await _currentUserService.Get(User);
            await _service.UpdateByForm(currentUser, goalForm);
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
            var currentUser = await _currentUserService.Get(User);
            var deletedGoal = await _service.Delete(currentUser, id);
            if (deletedGoal == null)
                return NotFound();

            return Ok(deletedGoal);
        }

        /// <summary>
        /// Удаление задач
        /// </summary>
        /// <param name="ids">Идентификаторы задач</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<VmGoal>> DeleteRange([FromBody]IEnumerable<int> ids)
        {
            var currentUser = await _currentUserService.Get(User);
            var deletedGoal = await _service.DeleteRange(currentUser, ids);
            if (deletedGoal == null)
                return NotFound();

            return Ok(deletedGoal);
        }

        /// <summary>
        /// Восстановление задачи
        /// </summary>
        /// <param name="id">Идентификатор восстанавливаемой задачи</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<VmGoal>> Restore(int id)
        {
            var currentUser = await _currentUserService.Get(User);
            var deletedGoal = await _service.Restore(currentUser, id);
            if (deletedGoal == null)
                return NotFound();

            return Ok(deletedGoal);
        }

        /// <summary>
        /// Восстановление задач
        /// </summary>
        /// <param name="ids">Идентификаторы задач</param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<VmGoal>> RestoreRange([FromBody]IEnumerable<int> ids)
        {
            var currentUser = await _currentUserService.Get(User);
            var deletedGoal = await _service.RestoreRange(currentUser, ids);
            if (deletedGoal == null)
                return NotFound();

            return Ok(deletedGoal);
        }


        /// <summary>
        /// Получение вложений задачи
        /// </summary>
        /// <param name="goalId">Идентификатор задачи</param>
        /// <returns>Коллекция вложений</returns>
        [HttpGet("{goalId}")]
        public async Task<ActionResult<IEnumerable<VmAttachment>>> GetAttachments(int goalId)
        {
            var currentUser = await _currentUserService.Get(User);
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
            var currentUser = await _currentUserService.Get(User);
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
            var attachment = await _attachmentsService.DownloadAttachmentFile(currentUser, memoryStream, attachmentId);
            memoryStream.Position = 0;
            return File(memoryStream, attachment.FileType ?? "*/*");
        }



        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGoalsService _service;
        private readonly IGoalAttachmentsService _attachmentsService;
        private readonly IFormFilesService _formFilesService;
        private readonly ICurrentUserService _currentUserService;
    }
}