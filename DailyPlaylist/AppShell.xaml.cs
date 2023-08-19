using DailyPlaylist.Services;
using DailyPlaylist.View;
using DailyPlaylist.ViewModel;

namespace DailyPlaylist;

public partial class AppShell : Shell
{
    public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(PlaylistPage), typeof(PlaylistPage));
        Routing.RegisterRoute(nameof(PlayerPage), typeof(PlayerPage));
        Routing.RegisterRoute(nameof(PlaylistConfigPage), typeof(PlaylistConfigPage));

        BindingContext = new LogoutViewModel();
    }
}
