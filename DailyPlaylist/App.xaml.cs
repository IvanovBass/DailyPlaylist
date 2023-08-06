using MediaManager;

namespace DailyPlaylist;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        CrossMediaManager.Current.Init();

        MainPage = new AppShell();
	}
}
