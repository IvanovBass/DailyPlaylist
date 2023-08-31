using DailyPlaylist.Services;
using DailyPlaylist.View;
using MauiAppDI.Helpers;

namespace DailyPlaylist.ViewModel
{
    public class LogoutViewModel
    {
        public ICommand LogoutCommand { get; }


        // public LogoutViewModel(IAppSessionManager appSessionManager)
        public LogoutViewModel()
        {

            LogoutCommand = new Command(async () =>
            {
                var result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to log out?", "Yes", "No");
                if (result)
                {

                    var authService = ServiceHelper.GetService<AuthService>();
                    var appSessionManager = ServiceHelper.GetService<AppSessionManager>();


                    authService.Logout();  // we unlog the authService and the active user in it

                    appSessionManager.DisposeCurrentScope();  // we dispose the current session and nullify the VM's

                    await CrossMediaManager.Current.Stop();
                    CrossMediaManager.Current.Queue.Clear();
                    MediaPlayerService.ResetProperties();


                    Application.Current.MainPage = new AppShell();
                    // ça et les 3 /// en dessous , un des 2 a réussi à killer tes instances de pages...

                    await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
                }
            });
        }
    }

}
 