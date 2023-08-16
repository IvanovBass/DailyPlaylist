using DailyPlaylist.Services;
using DailyPlaylist.ViewModel;

namespace DailyPlaylist.View;

public partial class LoginPage : ContentPage
{
    private AuthService _authService;
    public LoginPage()
	{
		InitializeComponent();

        _authService = new AuthService();
        
        BindingContext = new LoginViewModel(_authService);

    }

    private async Task AnimatePressedButton(Button button)
    {
        await button.ScaleTo(0.9, 100, Easing.Linear);
        await button.ScaleTo(1, 50, Easing.Linear);
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