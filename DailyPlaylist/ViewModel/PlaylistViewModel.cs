using DailyPlaylist.Services;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace DailyPlaylist.ViewModel
{
    public class PlaylistViewModel : BaseViewModel
    {
        private readonly AuthService _authService ;
        private User _activeUser;
        private ObservableCollection<Playlist> _userPlaylists;
        private Playlist _selectedPlaylist;
        private ObservableCollection<Track> _playlistTracks;
        private string _selectedTrackTitle = "Title";
        private string _selectedTrackArtist = "Artist";
        private string _selectedTrackCover = "music_notes.png";
        private string _description = "Describe what makes this playlist special ...";
        private string _name = "Name ...";
        private readonly string _apiKey = "tviGbZrm0b4nfxTgVGvKB0skS4VIkV8xpjJ0qB5hcXZ9VwqAYDnXHPg6ZgAyXKh5";
        private Lazy<HttpClient> _httpClient = new Lazy<HttpClient>();


        // CONSTRUCTOR //

        public PlaylistViewModel(AuthService authservice)
        {
            _authService = authservice;

            _activeUser = _authService.ActiveUser;

            //LoadUserPlaylists();

            //if (_userPlaylists != null)
            //{
            //    _userPlaylists = new ObservableCollection<Playlist>(
            //    _userPlaylists.OrderByDescending(p =>
            //        p.DateCreation > p.DateUpdated ? p.DateCreation : p.DateUpdated));
            //}

            // LoadDefaultPlaylistTracks();

        }

        // PROPERTIES //

        public User ActiveUser { get { return _activeUser; } }

        public ObservableCollection<Playlist> UserPlaylists
        {
            get => _userPlaylists;
            set => SetProperty(ref _userPlaylists, value);
        }

        public Playlist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                SetProperty(ref _selectedPlaylist, value);
                SetProperty(ref _description, value.Description);
                SetProperty(ref _name, value.Name);
                LoadTracksForPlaylist(value);
            }
        }

        public ObservableCollection<Track> PlaylistTracks
        {
            get => _playlistTracks;
            set => SetProperty(ref _playlistTracks, value);
        }

        public string SelectedTrackTitle
        {
            get => _selectedTrackTitle;
            set => SetProperty(ref _selectedTrackTitle, string.IsNullOrEmpty(value) ? "Title" : value);
        }
        public string SelectedTrackArtist
        {
            get => _selectedTrackArtist;
            set => SetProperty(ref _selectedTrackArtist, string.IsNullOrEmpty(value) ? "Artist" : value);
        }
        public string SelectedTrackCover
        {
            get => _selectedTrackCover;
            set => SetProperty(ref _selectedTrackCover, string.IsNullOrEmpty(value) ? "music_notes.png" : value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        // METHODS //

        public ICommand PlayPauseCommand { get; }
        public ICommand PlayFromCollectionViewCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand ItemSelectedCommand { get; }

        private async void LoadUserPlaylists()
        {
            _userPlaylists = new ObservableCollection<Playlist>(await RetrievePlaylistsAsync(_activeUser.Id));
        }

        private void LoadDefaultPlaylistTracks()
        {
            var defaultPlaylist = _userPlaylists.OrderByDescending(p => p.DateUpdated).FirstOrDefault();
            if (defaultPlaylist != null)
            {
                SelectedPlaylist = defaultPlaylist;
            }
        }

        private async void LoadTracksForPlaylist(Playlist playlist)
        {
            _playlistTracks = new ObservableCollection<Track>();

            foreach (var trackId in playlist.DeezerTrackIds)
            {
                var track = await FetchTrackFromDeezer(trackId);
                if (track != null)
                {
                    _playlistTracks.Add(track);
                }
            }
        }

        private async Task<Track> FetchTrackFromDeezer(long trackId)
        {
            var client = _httpClient.Value;
            var apiUrl = $"https://api.deezer.com/track/{trackId}";
            var response = await client.GetStringAsync(apiUrl);
            return JsonConvert.DeserializeObject<Track>(response);
        }


        public async Task<List<Playlist>> RetrievePlaylistsAsync(string playlistUserId)
        {
            var requestUri = "https://eu-central-1.aws.data.mongodb-api.com/app/data-httpe/endpoint/data/v1/action/find";
            var payload = new
            {
                collection = "Playlist",
                database = "DailyPlaylistDB",
                dataSource = "DailyPlaylistMongoDB",
                filter = new { userId = playlistUserId }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var client = _httpClient.Value;

            client.DefaultRequestHeaders.Clear();
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "*");
            client.DefaultRequestHeaders.Add("api-key", _apiKey);

            var response = await client.PostAsync(requestUri, content);

            var playlists = new List<Playlist>();


            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(responseData);
                var playlistsJson = jsonObject["documents"]?.ToString();
                Debug.WriteLine($"Retrieved Playlists from server: {playlistsJson}");

                if (playlistsJson.Contains(playlistUserId))
                {
                    playlists = JsonConvert.DeserializeObject<List<Playlist>>(playlistsJson);

                    if (playlists != null && playlists.Any())
                    {
                        return playlists;
                    }
                }
            }

            return null;
        }


    }
}
