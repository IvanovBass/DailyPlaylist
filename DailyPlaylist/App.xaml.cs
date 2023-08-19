namespace DailyPlaylist;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();

    }

    protected override void OnStart()
    {
        CrossMediaManager.Current.Init();
    }

    protected override void OnSleep()
    {
        CrossMediaManager.Current.Dispose();
    }

    protected override void OnResume()
    {
        CrossMediaManager.Current.Init();
    }
}
