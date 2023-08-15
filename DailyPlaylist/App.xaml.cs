namespace DailyPlaylist;

public partial class App : Application
{

    public App()
	{
		InitializeComponent();

        MainPage = new AppShell();

        CrossMediaManager.Current.Init();

    }
}
