using Realms.Sync;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.ViewModel
{
    public partial class PlaylistViewModel : BaseViewModel
    {
        private Realm realm;
        private PartitionSyncConfiguration config;

        public PlaylistViewModel() 
        {
            userPlaylists = new ObservableCollection<Playlist>();
            Name = "My nice playlist ... ";
            Description = "It is a playlist about love and joy... ";
        }

        [ObservableProperty]
        ObservableCollection<Playlist> userPlaylists;

        [ObservableProperty]
        string name;

        [ObservableProperty]
        string description;

        [ObservableProperty]
        Playlist playlist;

        // ...
    }
}
