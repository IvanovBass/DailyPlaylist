using DailyPlaylist.Services;
using DailyPlaylist.ViewModel;
using MauiAppDI.Helpers;
using System.ComponentModel;


namespace DailyPlaylist.View
{
    public partial class HomePage : ContentPage
    {
        private AuthService _authService;
        private PlaylistViewModel _playlistViewModel;
        private SearchViewModel _searchViewModel;
        public HomePage()
        {
            InitializeComponent();

            StartAnimations();
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
                    else 
                    {
                        await SnackBarVM.ShowSnackBarAsync("Problem to retrieve your playlists and details from server, please re-log in and try again", "Dismiss", () => { });
                    }
                    //var playVM = ServiceHelper.GetService<PlaylistViewModel>();
                    //var searchVM = ServiceHelper.GetService<SearchViewModel>();
                    //_searchViewModel = searchVM;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (NavigationState.LastVisitedPage == nameof(LoadingPage))
            {
                NavigationState.LastVisitedPage = "";
                await InitializeAsync();
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
