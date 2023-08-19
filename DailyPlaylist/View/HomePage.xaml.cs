using DailyPlaylist.Services;
using DailyPlaylist.ViewModel;
using MauiAppDI.Helpers;


namespace DailyPlaylist.View
{
    public partial class HomePage : ContentPage
    {
        private AuthService _authService;
        private PlaylistViewModel _playlistViewModel;
        public HomePage()
        {
            InitializeComponent();
            StartAnimations();

            _ = InitializeAsync();

            _playlistViewModel = new PlaylistViewModel(_authService);
        }

        private async Task InitializeAsync()
        {
            try
            {
                var authService = ServiceHelper.GetService<AuthService>();
                if (authService != null)
                {
                    _authService = authService;
                    string emailUser = _authService.WhoIsAuthenticatedAsync();
                    User authUser = await authService.RetrieveUserAsync(emailUser);

                    if (authUser != null && authUser is User)
                    {
                        _authService.ActiveUser = authUser;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private async void StartAnimations()
        {
            while (true)
            {
                await AnimateLogoImage();
                await AnimateButton();
                await Task.Delay(3000); 
            }
        }

        private async Task AnimateLogoImage()
        {
            await logoImage.ScaleTo(1.12, 800, Easing.SinIn);
            await logoImage.ScaleTo(1, 600, Easing.SinOut);
            for (int i = 0; i < 2; i++)
            {
                await logoImage.ScaleTo(1.10, 700, Easing.SinIn);
                await logoImage.ScaleTo(1, 500, Easing.SinOut);
            }
        }

        private async Task AnimateButton()
        {
            await PlaylistGenBtn.ScaleTo(1.25, 800, Easing.SinIn);
            await PlaylistGenBtn.ScaleTo(1, 600, Easing.SinOut);
            for (int i = 0; i < 2; i++)
            {
                await PlaylistGenBtn.ScaleTo(1.15, 700, Easing.SinIn);
                await PlaylistGenBtn.ScaleTo(1, 600, Easing.SinOut);
            }
        }
    }
}
