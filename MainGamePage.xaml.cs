using BreakMaster.ViewModels;

namespace BreakMaster;

public partial class MainGamePage : ContentPage
{
    public MainGamePage()
    {
        InitializeComponent();
        BindingContext = new MainGameViewModel();
    }
}
