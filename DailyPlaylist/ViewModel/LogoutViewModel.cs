using DailyPlaylist.Services;
using DailyPlaylist.View;


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
                    CrossMediaManager.Current.MediaPlayer = null;
                    //CrossMediaManager.Current.Dispose();

                    OnLogout?.Invoke();

                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            });
        }
    }

}
