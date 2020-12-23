using System.Linq;
using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmGoalMessageConverter : ViewModelConverter<GoalMessage, VmGoalMessage>
    {
        private readonly IViewModelConverter<UserGoalMessage, VmUserGoalMessage> _vmUserMessageConverter;

        public VmGoalMessageConverter(
          IViewModelConverter<UserGoalMessage, VmUserGoalMessage> vmUserMessageConverter)
        {
            this._vmUserMessageConverter = vmUserMessageConverter;
        }

        public override void SetModel(VmGoalMessage viewModel, GoalMessage model)
        {
            model.Id = viewModel.Id;
            model.Text = viewModel.Text;
            model.OwnerId = viewModel.OwnerId;
            model.GoalId = viewModel.GoalId;
            model.CreationDate = viewModel.CreationDate;
            model.LastEditDate = viewModel.LastEditDate;
            model.IsRemoved = viewModel.IsRemoved;

            model.MessageSubscribers = viewModel.MessageSubscribers?.Select(x =>
            {
                var um = _vmUserMessageConverter.ToModel(x);
                um.GoalMessageId = model.Id;
                return um;
            }).ToList();
        }

        public override void SetViewModel(GoalMessage model, VmGoalMessage viewModel)
        {
            viewModel.Id = model.Id;
            viewModel.Text = model.Text;
            viewModel.OwnerId = model.OwnerId;
            viewModel.GoalId = model.GoalId;
            viewModel.CreationDate = model.CreationDate;
            viewModel.LastEditDate = model.LastEditDate;
            viewModel.IsRemoved = model.IsRemoved;
            
            viewModel.MessageSubscribers = model.MessageSubscribers?
                .Select(x => _vmUserMessageConverter.ToViewModel(x))
                .ToList();
        }
    }
}