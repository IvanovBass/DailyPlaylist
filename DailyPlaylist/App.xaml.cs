using DailyPlaylist.View;
using MediaManager;

namespace DailyPlaylist;

public partial class App : Application
{
	public static Realms.Sync.App RealmApp;
	
	public App()
	{
		InitializeComponent();

		RealmApp = Realms.Sync.App.Create(AppConfig.RealmAppId);

        CrossMediaManager.Current.Init();

        MainPage = new AppShell();
	}
}
