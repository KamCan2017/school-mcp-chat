namespace School.Web.Components.Pages;

public partial class Home : IDisposable
{
    protected override void OnInitialized()
    {
        ViewModel.StateChanged += OnViewModelStateChanged;
    }

    private void OnViewModelStateChanged() => _ = InvokeAsync(StateHasChanged);

    public void Dispose() => ViewModel.StateChanged -= OnViewModelStateChanged;
}
