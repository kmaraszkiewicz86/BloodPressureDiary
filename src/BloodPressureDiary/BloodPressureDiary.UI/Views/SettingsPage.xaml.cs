using BloodPressureDiary.UI.ViewModels;

namespace BloodPressureDiary.UI.Views;

/// <summary>Displays reminder settings.</summary>
public partial class SettingsPage : ContentPage
{
    /// <summary>Creates the settings page.</summary>
    public SettingsPage(SettingsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
