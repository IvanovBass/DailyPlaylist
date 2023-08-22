using DailyPlaylist.Services;
using DailyPlaylist.View;
using MauiAppDI.Helpers;

namespace DailyPlaylist.ViewModel
{
    public class LogoutViewModel
    {
        private readonly AuthService _authService;

        public static event Action OnLogout;

        public ICommand LogoutCommand { get; }

        public LogoutViewModel( )
        {
            _authService = new AuthService();

            LogoutCommand = new Command(async () =>
            {
                var result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to log out?", "Yes", "No");
                if (result)
                {
                    _authService.Logout();

                    await CrossMediaManager.Current.Stop();
                    CrossMediaManager.Current.Queue.Clear();
                    CrossMediaManager.Current.MediaPlayer = null;
                    //CrossMediaManager.Current.Dispose();

                    OnLogout?.Invoke();

                    MediaPlayerService.ResetProperties();

                    // NavigationState.IsRelogged = true;   // on va mettre la lgoqieu sur le disappearing

                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            });
        }
    }

}
