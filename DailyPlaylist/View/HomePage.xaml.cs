using DailyPlaylist.Services;

namespace DailyPlaylist.View
{
    public partial class HomePage : ContentPage
    {
        private readonly AppSessionManager _appSessionManager;
        private readonly AuthService _authService;


        public HomePage(AuthService authService)
        {
            InitializeComponent();

            StartAnimations();
            
            _authService = authService;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (NavigationState.LastVisitedPage == nameof(LoadingPage))
            {
                NavigationState.LastVisitedPage = "";
                _appSessionManager = new AppSessionManager(IServiceScopeFactory, _authService);
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
