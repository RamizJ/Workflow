using Workflow.DAL.Models;
using Workflow.VM.ViewModelConverters.Absract;
using Workflow.VM.ViewModels;

namespace Workflow.VM.ViewModelConverters
{
    public class VmUserConverter : IViewModelConverter<ApplicationUser, VmUser>
    {
        public ApplicationUser ToModel(VmUser viewModel)
        {
            if (viewModel == null)
                return null;

            var id = string.IsNullOrWhiteSpace(viewModel.Id) ? null : viewModel.Id;
            var model = new ApplicationUser {Id = id};

            SetModel(viewModel, model);

            return model;
        }

        public VmUser ToViewModel(ApplicationUser model)
        {
            if (model == null)
                return null;

            var viewModel =  new VmUser();
            SetViewModel(model, viewModel);

            return viewModel;
        }

        public void SetModel(VmUser viewModel, ApplicationUser model)
        {
            if(viewModel == null || model == null)
                return;

            model.UserName = viewModel.UserName;
            model.Email = viewModel.Email;
            model.NormalizedEmail = viewModel.Email.ToUpper();
            model.NormalizedUserName = viewModel.Email.ToUpper();
            model.PhoneNumber = viewModel.Phone;
            model.PositionId = viewModel.PositionId;
            model.PositionCustom = viewModel.PositionId == null ? viewModel.Position : null;
            model.FirstName = viewModel.FirstName;
            model.MiddleName = viewModel.MiddleName;
            model.LastName = viewModel.LastName;
        }

        public void SetViewModel(ApplicationUser model, VmUser viewModel)
        {
            if (model == null || viewModel == null)
                return;

            viewModel.Id = model.Id;
            viewModel.UserName = model.UserName;
            viewModel.Email = model.Email;
            viewModel.Phone = model.PhoneNumber;
            viewModel.PositionId = model.PositionId;
            viewModel.Position = model.Position?.Name ?? model.PositionCustom;
            viewModel.FirstName = model.FirstName;
            viewModel.MiddleName = model.MiddleName;
            viewModel.LastName = model.LastName;
            viewModel.IsRemoved = model.IsRemoved;
        }
    }
}