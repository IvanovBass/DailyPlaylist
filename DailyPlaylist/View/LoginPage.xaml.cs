using DailyPlaylist.Services;
using DailyPlaylist.ViewModel;

namespace DailyPlaylist.View;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService; 
    
    public LoginPage(AuthService authService)
	{
		InitializeComponent();
        _authService = authService;
        BindingContext = new LoginViewModel(authService);

    }
    private async Task AnimatePressedButton(Button button)
    {
        await button.ScaleTo(0.8, 130, Easing.Linear);
        await button.ScaleTo(1, 70, Easing.Linear);
    }

    private async void ButtonLoginClicked(object sender, EventArgs e)
    {
        await AnimatePressedButton(ButtonLogin);
    }
    private async void ButtonCreateAccountClicked(object sender, EventArgs e)
    {
        await AnimatePressedButton(ButtonCreateAccount);
    }
}