// using MediaManager;

using DailyPlaylist.View;

namespace DailyPlaylist;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		new NavigationPage(new HomePage());
        // CrossMediaManager.Current.Init();
    }
}
