namespace Workflow.VM.ViewModelConverters.Absract
{
    public interface IViewModelConverter<TModel, TViewModel>
    {
        TModel ToModel(TViewModel viewModel);
        TViewModel ToViewModel(TModel model);

        void SetModel(TViewModel viewModel, TModel model);
        void SetViewModel(TModel model, TViewModel viewModel);
    }
}
