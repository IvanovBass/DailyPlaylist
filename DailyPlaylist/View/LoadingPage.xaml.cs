﻿using DailyPlaylist.Services;

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

        await Task.Delay(4000);

		if (_authService.IsAuthenticatedAsync())
		{
            NavigationState.LastVisitedPage = nameof(LoadingPage);
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
		else
		{
			await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
			await Task.Delay(3500);
            if(_authService.IsAuthenticatedAsync())
			{
				NavigationState.LastVisitedPage = nameof(LoadingPage);
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
        }
	}
}
