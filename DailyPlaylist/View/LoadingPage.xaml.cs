using DailyPlaylist.Services;

namespace DailyPlaylist.View;

public partial class LoadingPage : ContentPage
{
    private readonly AuthService  _authService;
    // private readonly AppSessionManager _appSessionManager;

    public LoadingPage(AuthService authService)
	{
		InitializeComponent();
        _authService = authService;
        // _appSessionManager = appSessionManager;
    }

	protected async override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

        await Task.Delay(3500);

		if (_authService.IsAuthenticatedAsync())
		{
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            NavigationState.LastVisitedPage = nameof(LoadingPage);
        }
		else
		{
			await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
			await Task.Delay(3000);
            if(_authService.IsAuthenticatedAsync())
			{ 
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                NavigationState.LastVisitedPage = nameof(LoadingPage);
            }
        }
    }
}
