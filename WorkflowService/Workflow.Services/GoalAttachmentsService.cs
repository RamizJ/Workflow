using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class GoalAttachmentsService : IGoalAttachmentsService
    {
        public GoalAttachmentsService(DataContext dataContext, 
            UserManager<ApplicationUser> userManager,
            IFileService fileService)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _fileService = fileService;
            _vmConverter = new VmAttachmentConverter();
        }


        /// <inheritdoc />
        public async Task<IEnumerable<VmAttachment>> GetAll(ApplicationUser currentUser, int goalId)
        {
            if(currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = await GetQuery(currentUser, true);
            var attachments = query
                .Where(g => g.Id == goalId)
                .SelectMany(g => g.Attachments);

            return attachments.Select(a => _vmConverter.ToViewModel(a));
        }


        /// <inheritdoc />
        public async Task Add(ApplicationUser currentUser,
            int goalId, ICollection<Attachment> attachments)
        {
            if(currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = await GetQuery(currentUser, true);
            var goal = await query.FirstOrDefaultAsync(g => g.Id == goalId);

            if (goal == null)
                throw new InvalidOperationException("Cannot add attachments to goal. Goal for current user not found");

            foreach (var attachment in attachments) 
                attachment.CreationDate = DateTime.Now;

            goal.Attachments.AddRange(attachments);
            await _dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task Remove(ApplicationUser currentUser, IEnumerable<int> attachmentIds)
        {
            var query = await GetQuery(currentUser, true);
            var attachments = query
                .SelectMany(g => g.Attachments)
                .Where(a => attachmentIds.Any(aId => aId == a.Id));

            if (attachments.Any())
            {
                _dataContext.Attachments.RemoveRange(attachments);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task<Attachment> DownloadAttachmentFile(ApplicationUser currentUser, Stream stream, int attachmentId)
        {
            var query = await GetQuery(currentUser, true);
            var attachment = await query
                .SelectMany(g => g.Attachments)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);

            if(attachment == null)
                throw new InvalidOperationException($"Attachment with id='{attachmentId}' not found");

            await _fileService.Download(stream, attachment.FileDataId);
            return attachment;
        }

        private async Task<IQueryable<Goal>> GetQuery(ApplicationUser currentUser, bool withRemoved)
        {
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, RoleNames.ADMINISTRATOR_ROLE);
            var query = _dataContext.Goals
                .Include(g => g.Attachments)
                .Where(x => isAdmin
                            || x.Project.OwnerId == currentUser.Id
                            || x.Project.ProjectTeams
                                .SelectMany(pt => pt.Team.TeamUsers)
                                .Any(tu => tu.UserId == currentUser.Id));

            if (!withRemoved)
                query = query.Where(x => x.IsRemoved == false);

            return query;
        }


        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;
        private readonly VmAttachmentConverter _vmConverter;
    }
}