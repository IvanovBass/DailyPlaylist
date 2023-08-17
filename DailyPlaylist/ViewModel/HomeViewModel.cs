using DailyPlaylist.View;

namespace DailyPlaylist.ViewModel
{
    public partial class HomeViewModel : BaseViewModel
    {
        public HomeViewModel() 
        {
            
        }

        public ICommand GoToPlaylistConfigCommand => new Command(async () => await GoToPlaylistConfigAsync());

        private async Task GoToPlaylistConfigAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new PlaylistConfigPage());
        }


    }
}
