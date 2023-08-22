using DailyPlaylist.Services;
using DailyPlaylist.View;
using MauiAppDI.Helpers;

namespace DailyPlaylist.ViewModel
{
    public class LogoutViewModel
    {
        private readonly AuthService _authService;

        private readonly IAppSessionManager _appSessionManager;

        // public static event Action OnLogout;

        public ICommand LogoutCommand { get; }

        public LogoutViewModel(IAppSessionManager appSessionManager)
        {

            _authService = ServiceHelper.GetService<AuthService>();

            _appSessionManager = appSessionManager ?? throw new ArgumentNullException(nameof(appSessionManager));

            LogoutCommand = new Command(async () =>
            {
                var result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to log out?", "Yes", "No");
                if (result)
                {
                    _authService.Logout();  // we unlog the authService and the active user in it

                    Logout();  // we dispose the current session

                    await CrossMediaManager.Current.Stop();
                    CrossMediaManager.Current.Queue.Clear();
                    MediaPlayerService.ResetProperties();

                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            });
        }
        public void Logout()
        {
            _appSessionManager.EndSession();
        }
    }

}
