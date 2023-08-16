using DailyPlaylist.Services;

namespace DailyPlaylist.View;

public partial class LoadingPage : ContentPage
{
    private readonly AuthService  _authService;

    public LoadingPage(AuthService authService)
	{
		InitializeComponent();
		_authService = authService;
	}

	protected async override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		if (await _authService.IsAuthenticatedAsync())  // attente de deux seecondes pour authentifier
		{
			await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
		}
		else
		{
			await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
		}
	}
}
