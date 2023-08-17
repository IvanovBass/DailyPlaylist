using DailyPlaylist.Services;
using DailyPlaylist.ViewModel;
using MauiAppDI.Helpers;

namespace DailyPlaylist.View;

public partial class PlaylistPage : ContentPage
{
	private readonly AuthService _authService;
	public PlaylistPage()
    {
        InitializeComponent();

        var authService = ServiceHelper.GetService<AuthService>();

        _authService = authService;

        BindingContext = new PlaylistViewModel(_authService);

    }
}
