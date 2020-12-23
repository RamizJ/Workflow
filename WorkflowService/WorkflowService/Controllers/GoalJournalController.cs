using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace WorkflowService.Controllers
{
    /// <summary>
    /// API-методы работы с журналом сообщений задачи
    /// </summary>
    [ApiController, Route("api/[controller]/[action]")]
    public class GoalJournalController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMessage(ApplicationUser currentUser, int messageId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadMessagesCount(ApplicationUser currentUser)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<VmGoalMessage>> GetUnreadMessages(ApplicationUser currentUser)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> AddMessage(VmGoalMessage message)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> UpdateMessage(VmGoalMessage message)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            throw new NotImplementedException();
        }
    }
}