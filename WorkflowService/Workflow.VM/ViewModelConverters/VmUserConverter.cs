using System;
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
            return new ApplicationUser
            {
                Id = id,
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                NormalizedEmail = viewModel.Email.ToUpper(),
                NormalizedUserName = viewModel.Email.ToUpper(),
                PhoneNumber = viewModel.Phone,
                PositionId = viewModel.PositionId,
                PositionCustom = viewModel.PositionId == null ? viewModel.Position : null,
                FirstName = viewModel.FirstName,
                MiddleName = viewModel.MiddleName,
                LastName = viewModel.LastName
            };
        }

        public VmUser ToViewModel(ApplicationUser model)
        {
            if (model == null)
                return null;

            return new VmUser
            {
                Id = model.Id,
                UserName = model.UserName,
                Email = model.Email,
                Phone = model.PhoneNumber,
                PositionId = model.PositionId,
                Position = model.Position?.Name ?? model.PositionCustom,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                IsRemoved = model.IsRemoved
            };
        }
    }
}