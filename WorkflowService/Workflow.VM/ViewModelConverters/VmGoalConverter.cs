using System.Linq;
using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmGoalConverter : IViewModelConverter<Goal, VmGoal>
    {
        public Goal ToModel(VmGoal viewModel)
        {
            if (viewModel == null)
                return null;

            return new Goal
            {
                Id = viewModel.Id,
                ParentGoalId = viewModel.ParentGoalId,
                OwnerId = viewModel.OwnerId,
                AttachmentId = viewModel.AttachmentId,
                CreationDate = viewModel.CreationDate,
                Title = viewModel.Title,
                Description = viewModel.Description,
                GoalNumber = viewModel.GoalNumber,
                PerformerId = viewModel.PerformerId,
                ProjectId = viewModel.ProjectId,
                State = viewModel.State,
                Priority = viewModel.Priority,
                IsRemoved = viewModel.IsRemoved,
            };
        }

        public VmGoal ToViewModel(Goal model)
        {
            if (model == null)
                return null;

            return new VmGoal
            {
                Id = model.Id,
                ParentGoalId = model.ParentGoalId,
                ChildGoals = model.ChildGoals?.Select(x => x.Id).ToList(),
                OwnerId = model.OwnerId,
                AttachmentId = model.AttachmentId,
                CreationDate = model.CreationDate,
                Title = model.Title,
                Description = model.Description,
                GoalNumber = model.GoalNumber,
                PerformerId = model.PerformerId,
                Observers = model.Observers?.Select(x => x.ObserverId).ToList(),
                ProjectId = model.ProjectId,
                State = model.State,
                Priority = model.Priority,
                IsRemoved = model.IsRemoved
            };
        }
    }
}