using BloodPressureDiary.UI.ViewModels;

namespace BloodPressureDiary.UI;

/// <summary>Displays the measurements sheet.</summary>
public partial class MainPage : ContentPage
{
    /// <summary>Creates the measurements page.</summary>
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
