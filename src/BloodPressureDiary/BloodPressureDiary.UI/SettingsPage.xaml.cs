using BloodPressureDiary.UI.ViewModels;
namespace BloodPressureDiary.UI;
/// <summary>Displays reminder preferences.</summary>
public partial class SettingsPage : ContentPage
{
    /// <summary>Initializes the settings view.</summary>
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
