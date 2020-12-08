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

            var model = new Goal();
            SetModel(viewModel, model);

            return model;
        }

        public VmGoal ToViewModel(Goal model)
        {
            if (model == null)
                return null;

            var viewModel = new VmGoal();
            SetViewModel(model, viewModel);

            return viewModel;
        }

        public void SetModel(VmGoal viewModel, Goal model)
        {
            model.Id = viewModel.Id;
            model.ParentGoalId = viewModel.ParentGoalId == 0 ? null : viewModel.ParentGoalId;
            model.OwnerId = viewModel.OwnerId;
            model.CreationDate = viewModel.CreationDate;
            model.Title = viewModel.Title;
            model.Description = viewModel.Description;
            model.GoalNumber = viewModel.GoalNumber;
            model.PerformerId = viewModel.PerformerId;
            //Observers = viewModel.Observers?.Select(oId => new GoalObserver(viewModel.Id, oId)).ToList(),
            model.ProjectId = viewModel.ProjectId;
            model.State = viewModel.State;
            model.Priority = viewModel.Priority;
            model.ExpectedCompletedDate = viewModel.ExpectedCompletedDate;
            model.EstimatedPerformingTime = viewModel.EstimatedPerformingTime;
            model.IsRemoved = viewModel.IsRemoved;
        }

        public void SetViewModel(Goal model, VmGoal viewModel)
        {
            if (model == null || viewModel == null)
                return;

            viewModel.Id = model.Id;
            viewModel.ParentGoalId = model.ParentGoalId;
            viewModel.OwnerId = model.OwnerId;
            viewModel.CreationDate = model.CreationDate;
            viewModel.Title = model.Title;
            viewModel.Description = model.Description;
            viewModel.GoalNumber = model.GoalNumber;
            viewModel.PerformerId = model.PerformerId;
            //PerformerFio = model.Performer.LastName + " " + model.Performer.FirstName + " " + model.Performer.MiddleName,
            viewModel.PerformerFio = model.Performer?.Fio;
            viewModel.ProjectId = model.ProjectId;
            viewModel.ProjectName = model.Project?.Name;
            viewModel.State = model.State;
            viewModel.Priority = model.Priority;
            viewModel.ExpectedCompletedDate = model.ExpectedCompletedDate;
            viewModel.EstimatedPerformingTime = model.EstimatedPerformingTime;
            viewModel.ActualPerformingTime = model.ActualPerformingTime;
            viewModel.IsRemoved = model.IsRemoved;
            viewModel.HasChildren = model.ChildGoals.Any();
            viewModel.IsAttachmentsExist = model.Attachments.Any();
            viewModel.ObserverIds = model.Observers?.Select(x => x.ObserverId).ToList();
            viewModel.MetadataList = model.MetadataList?.Select(x => new VmMetadata
            {
                Key = x.Key,
                Value = x.Value
            }).ToList();
        }
    }
}