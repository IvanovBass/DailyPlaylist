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

        }

        [ObservableProperty]
        ObservableCollection<Playlist> userPlaylist;

        [ObservableProperty]
        string emptyText;
    }
}
