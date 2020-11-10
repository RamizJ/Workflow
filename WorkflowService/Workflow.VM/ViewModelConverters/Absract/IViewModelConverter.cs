namespace Workflow.VM.ViewModelConverters.Absract
{
    public interface IViewModelConverter<TModel, TViewModel>
    {
        TModel ToModel(TViewModel viewModel);
        TViewModel ToViewModel(TModel model);

        void SetModel(TViewModel viewModel, TModel model);
        void SetViewModel(TModel model, TViewModel viewModel);
    }

    public abstract class ViewModelConverter<TModel, TViewModel> 
        : IViewModelConverter<TModel, TViewModel> 
        where TModel : new() 
        where TViewModel : new()
    {
        public virtual TViewModel ToViewModel(TModel model)
        {
            if (model == null)
                return default;

            var viewModel = new TViewModel();
            SetViewModel(model, viewModel);

            return viewModel;
        }

        public virtual TModel ToModel(TViewModel viewModel)
        {
            if (viewModel == null)
                return default;

            var model = new TModel();
            SetModel(viewModel, model);

            return model;
        }

        public abstract void SetModel(TViewModel viewModel, TModel model);
        public abstract void SetViewModel(TModel model, TViewModel viewModel);
    }
}
