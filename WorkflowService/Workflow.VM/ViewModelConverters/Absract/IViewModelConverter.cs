using System.Collections.Generic;
using System.Text;

namespace Workflow.VM.ViewModelConverters.Absract
{
    public interface IViewModelConverter<TModel, TViewModel>
    {
        TModel ToModel(TViewModel viewModel);
        TViewModel ToViewModel(TModel model);
    }
}
