using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;
using WorkflowService.Services.Abstract;

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
        public GoalsController(ICurrentUserService currentUserService, 
            IGoalsService goalsService,
            IGoalAttachmentsService attachmentsService,
            IFormFilesService formFilesService)
        {
            _currentUserService = currentUserService;
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _service.GetRange(currentUser, ids);
        }

        /// <summary>
        /// Получение кол-ва задач проекта
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <returns></returns>
        [HttpGet("{projectId}")]
        public async Task<int> GetTotalProjectGoalsCount(int projectId)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _service.GetTotalProjectGoalsCount(currentUser, projectId);
        }

        /// <summary>
        /// Получение кол-ва задач проекта по состоянию задачи
        /// </summary>
        /// <param name="projectId">Идентификатор проекта</param>
        /// <param name="goalState">Состояние задачи</param>
        /// <returns></returns>
        [HttpGet("{projectId}")]
        public async Task<int> GetProjectGoalsByStateCount(int projectId, [FromQuery]GoalState goalState)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            return await _service.GetProjectGoalsByStateCount(currentUser, projectId, goalState);
        }

        /// <summary>
        /// Создание задачи
        /// </summary>
        /// <param name="goal">Создаваемая задача</param>
        /// <returns>Созданная задача</returns>
        [HttpPost]
        public async Task<ActionResult<VmGoal>> Create([FromBody]VmGoal goal)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
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
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _attachmentsService.Remove(currentUser, attachmentIds);
            return NoContent();
        }

        /// <summary>
        /// Загрузка файла вложения
        /// </summary>
        /// <param name="attachmentId">Идентификатор вложения</param>
        /// <returns></returns>
        [HttpGet("{attachmentId}")]
        public async Task<FileResult> DownloadAttachmentFile(int attachmentId)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var memoryStream = new MemoryStream();
            var attachment = await _attachmentsService.DownloadAttachmentFile(currentUser, memoryStream, attachmentId);
            memoryStream.Position = 0;
            return File(memoryStream, attachment.FileType ?? "*/*");
        }

        /// <summary>
        /// Получение дочерних задач
        /// </summary>
        /// <param name="parentGoalId">Идентификатор родительской задачи</param>
        /// <returns>Коллекция дочерних задач</returns>
        [HttpGet("{parentGoalId}")]
        public async Task<ActionResult<IEnumerable<VmGoal>>> GetChildGoals(int parentGoalId)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var childGoals = await _service.GetChildGoals(currentUser, parentGoalId, true);
            return Ok(childGoals);
        }

        /// <summary>
        /// Получение родительской задачи
        /// </summary>
        /// <param name="goalId">Идентификатор задачи</param>
        /// <returns>Родительская задача</returns>
        [HttpGet("{goalId}")]
        public async Task<ActionResult<VmGoal>> GetParentGoal(int goalId)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            var parentGoal = await _service.GetParentGoal(currentUser, goalId);
            return Ok(parentGoal);
        }

        /// <summary>
        /// Добавление дочерних задач
        /// </summary>
        /// <param name="parentGoalId">Родительская задача</param>
        /// <param name="childGoalIds">Идентификаторы дочерних задач</param>
        /// <returns></returns>
        [HttpPatch("{parentGoalId}")]
        public async Task<IActionResult> AddChildGoals(int parentGoalId, 
            [FromBody] IEnumerable<int> childGoalIds)
        {
            var currentUser = await _currentUserService.GetCurrentUser(User);
            await _service.AddChildGoals(currentUser, parentGoalId, childGoalIds);
            return NoContent();
        }


        private readonly ICurrentUserService _currentUserService;
        private readonly IGoalsService _service;
        private readonly IGoalAttachmentsService _attachmentsService;
        private readonly IFormFilesService _formFilesService;
    }
}