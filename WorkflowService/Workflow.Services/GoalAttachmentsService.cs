using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Workflow.DAL;
using Workflow.DAL.Models;
using Workflow.DAL.Repositories.Abstract;
using Workflow.Services.Abstract;
using Workflow.Services.Exceptions;
using Workflow.Services.Extensions;
using Workflow.VM.ViewModelConverters;
using Workflow.VM.ViewModels;
using static System.Net.HttpStatusCode;

namespace Workflow.Services
{
    /// <inheritdoc />
    public class GoalAttachmentsService : IGoalAttachmentsService
    {
        public GoalAttachmentsService(DataContext dataContext, 
            IUsersRepository usersRepository,
            IBackgroundTaskQueue<VmEntityStateMessage> entityStateQueue,
            IFileService fileService)
        {
            _dataContext = dataContext;
            _usersRepository = usersRepository;
            _entityStateQueue = entityStateQueue;
            _fileService = fileService;
            _vmConverter = new VmAttachmentConverter();
        }


        /// <inheritdoc />
        public async Task<IEnumerable<VmAttachment>> GetAll(ApplicationUser currentUser, int goalId)
        {
            if(currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = GetQuery(currentUser, true);
            var attachments = await query
                .Where(g => g.Id == goalId)
                .SelectMany(g => g.Attachments)
                .ToArrayAsync();

            return attachments.Select(a => _vmConverter.ToViewModel(a));
        }


        /// <inheritdoc />
        public async Task Add(ApplicationUser currentUser,
            int goalId, ICollection<Attachment> attachments)
        {
            if(currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            var query = GetQuery(currentUser, true);
            var goal = await query.FirstOrDefaultAsync(g => g.Id == goalId);

            if (goal == null)
                throw new HttpResponseException(BadRequest, 
                    "Cannot add attachments to goal. Goal for current user not found");

            foreach (var attachment in attachments) 
                attachment.CreationDate = DateTime.Now.ToUniversalTime();

            goal.Attachments.AddRange(attachments);
            await _dataContext.SaveChangesAsync();

            _entityStateQueue.EnqueueIds(currentUser.Id, 
                attachments.Select(x => x.Id), 
                nameof(Attachment), EntityOperation.Create);
        }

        /// <inheritdoc />
        public async Task Remove(ApplicationUser currentUser, IEnumerable<int> attachmentIds)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));
            
            var query = GetQuery(currentUser, true);
            var attachments = await query
                .SelectMany(g => g.Attachments)
                .Where(a => attachmentIds.Any(aId => aId == a.Id))
                .ToArrayAsync();

            if (attachments.Any())
            {
                _dataContext.Attachments.RemoveRange(attachments);
                await _dataContext.SaveChangesAsync();

                _entityStateQueue.EnqueueIds(currentUser.Id,
                    attachments.Select(x => x.Id),
                    nameof(Attachment), EntityOperation.Delete);
            }
        }

        public async Task<Attachment> DownloadAttachmentFile(ApplicationUser currentUser, Stream stream, int attachmentId)
        {
            var query = GetQuery(currentUser, true);
            var attachment = await query
                .SelectMany(g => g.Attachments)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);

            if(attachment == null)
                throw new HttpResponseException(BadRequest, 
                    $"Attachment with id='{attachmentId}' not found");

            await _fileService.Download(stream, attachment.FileDataId);
            return attachment;
        }

        private IQueryable<Goal> GetQuery(ApplicationUser currentUser, bool withRemoved)
        {
            var query = _dataContext.Goals
                .Include(g => g.Attachments)
                .Where(x => _usersRepository.IsAdmin(currentUser.Id)
                            || x.Project.OwnerId == currentUser.Id
                            || x.Project.ProjectTeams
                                .SelectMany(pt => pt.Team.TeamUsers)
                                .Any(tu => tu.UserId == currentUser.Id));

            if (!withRemoved)
                query = query.Where(x => x.IsRemoved == false);

            return query;
        }


        private readonly DataContext _dataContext;
        private readonly IUsersRepository _usersRepository;
        private readonly IBackgroundTaskQueue<VmEntityStateMessage> _entityStateQueue;
        private readonly IFileService _fileService;
        private readonly VmAttachmentConverter _vmConverter;
    }
}