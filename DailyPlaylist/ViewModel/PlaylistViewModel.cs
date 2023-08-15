// using Realms.Sync;
// using Realms;

namespace DailyPlaylist.ViewModel
{
    public class PlaylistViewModel : BaseViewModel
    {
        private User _activeUser;
        private Playlist _activePlaylist;
        private ObservableCollection<Playlist> _userPlaylists;

        public PlaylistViewModel()
        {
            _activeUser = new User();
            LoadUserPlaylists();
            SetActivePlaylist();
        }

        public ObservableCollection<Playlist> UserPlaylists
        {
            get => _userPlaylists;
            set
            {
                _userPlaylists = value;
                OnPropertyChanged();
            }
        }

        public Playlist ActivePlaylist
        {
            get => _activePlaylist;
            set
            {
                _activePlaylist = value;
                OnPropertyChanged();
            }
        }

        private void LoadUserPlaylists()
        {
            // Fetch the user's playlists, possibly from a database or service.
            // Set the UserPlaylists property.
        }

        private void SetActivePlaylist()
        {
            // Sort the user's playlists by DateUpdated (or DateCreation if DateUpdated is not available).
            // Set the first playlist from the sorted list as the ActivePlaylist.
        }
    }
}
