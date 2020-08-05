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
                CreationDate = viewModel.CreationDate,
                Title = viewModel.Title,
                Description = viewModel.Description,
                GoalNumber = viewModel.GoalNumber,
                PerformerId = viewModel.PerformerId,
                //Observers = viewModel.Observers?.Select(oId => new GoalObserver(viewModel.Id, oId)).ToList(),
                ProjectId = viewModel.ProjectId,
                State = viewModel.State,
                Priority = viewModel.Priority,
                ExpectedCompletedDate = viewModel.ExpectedCompletedDate,
                EstimatedPerformingTime = viewModel.EstimatedPerformingTime,
                IsRemoved = viewModel.IsRemoved
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
                OwnerId = model.OwnerId,
                Title = model.Title,
                Description = model.Description,
                GoalNumber = model.GoalNumber,
                PerformerId = model.PerformerId,
                PerformerFio = model.Performer?.Fio,
                ProjectId = model.ProjectId,
                ProjectName = model.Project?.Name,
                State = model.State,
                Priority = model.Priority,
                ExpectedCompletedDate = model.ExpectedCompletedDate,
                EstimatedPerformingTime = model.EstimatedPerformingTime,
                IsRemoved = model.IsRemoved
            };
        }
    }
}