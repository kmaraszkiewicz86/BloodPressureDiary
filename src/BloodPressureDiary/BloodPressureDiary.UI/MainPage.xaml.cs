using BloodPressureDiary.UI.ViewModels;
namespace BloodPressureDiary.UI;
/// <summary>Displays the measurement spreadsheet.</summary>
public partial class MainPage : ContentPage
{
    private readonly MainViewModel viewModel;
    /// <summary>Initializes the measurement view.</summary>
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }
    /// <summary>Loads measurements when the tab appears.</summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadCommand.ExecuteAsync(null);
    }
}
