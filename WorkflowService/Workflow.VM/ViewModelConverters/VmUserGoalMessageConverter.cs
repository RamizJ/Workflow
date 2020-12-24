using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmUserGoalMessageConverter : ViewModelConverter<UserGoalMessage, VmUserGoalMessage>
    {
        public override void SetModel(VmUserGoalMessage viewModel, UserGoalMessage model)
        {
            model.UserId = viewModel.UserId;
            model.LastReadingDate = viewModel.LastReadingDate;
        }

        public override void SetViewModel(UserGoalMessage model, VmUserGoalMessage viewModel)
        {
            viewModel.UserId = model.UserId;
            viewModel.UserFullName = model.User?.FullName;
            viewModel.LastReadingDate = model.LastReadingDate;
        }
    }
}