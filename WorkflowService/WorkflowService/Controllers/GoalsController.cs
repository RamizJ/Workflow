using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.VM.ViewModels;
using WorkflowService.Common;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API работы с задачами
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class GoalsController : ControllerBase
    {
        /// <summary>
        /// Получить задачу по идентификатору, если она доступна пользователю
        /// </summary>
        /// <param name="id">Идентификатор задачи</param>
        /// <returns>Задача</returns>
        [HttpGet("{id}")]
        public Task<ActionResult<VmGoal>> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Получить все задачи доступные пользователю
        /// </summary>
        /// <returns>Коллеция задач</returns>
        [HttpGet]
        public Task<IEnumerable<VmGoal>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Постраничная загрузка задач с фильтрацией и сортировкой
        /// </summary>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="filter">Строка фильтрации</param>
        /// <param name="filterFields">Поля фильтрации. Если не указаны, то фильтрация производится по всем полям</param>
        /// <param name="sort">Тип сортировки</param>
        /// <param name="sortedFields">Поля сортировки</param>
        /// <returns>Коллеция задач</returns>
        [HttpGet]
        public Task<IEnumerable<VmGoal>> GetPage([FromQuery]int pageNumber, [FromQuery]int pageSize, 
            [FromQuery]string filter, [FromQuery]string[] filterFields, 
            [FromQuery]SortType sort, [FromQuery]string[] sortedFields)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Получение задач по идентификаторам. Возвращаются только задачи доступные пользователю
        /// </summary>
        /// <param name="ids">Идентификаторы задач</param>
        /// <returns>Коллекция задач</returns>
        [HttpGet]
        public Task<IEnumerable<VmGoal>> GetRange([FromQuery]int[] ids)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Создание задачи
        /// </summary>
        /// <param name="goal">Новая задача</param>
        /// <returns>Задача с обновленным идентификатором</returns>
        [HttpPost]
        public Task<ActionResult<VmGoal>> Create([FromBody]VmGoal goal)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Обновление задачи
        /// </summary>
        /// <param name="goal">Обновленная задача</param>
        /// <returns></returns>
        [HttpPut]
        public Task<IActionResult> Update([FromBody]VmGoal goal)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Удаление задачи
        /// </summary>
        /// <param name="id">Идентификатор задачи</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public Task<ActionResult<VmGoal>> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}