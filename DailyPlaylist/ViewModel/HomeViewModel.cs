using DailyPlaylist.View;
using Realms;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
