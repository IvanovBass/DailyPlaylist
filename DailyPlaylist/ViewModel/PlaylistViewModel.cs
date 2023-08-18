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
        private readonly string _apiKey = "BhSU3Kx9cS7norilVbrWO6JicjdihtQUnYZOhrU7Js8GYSYIqQOka0uh1znKlf7H";
        private Lazy<HttpClient> _httpClient = new Lazy<HttpClient>();


        // CONSTRUCTOR //

        public PlaylistViewModel(AuthService authservice)
        {
            _authService = authservice;

            _activeUser = _authService.ActiveUser;

            _ = InitializationAsync();

            //string jsonPlaylists = @"[
            //    {
            //        ""_id"": ""550e8400-e29b-41d4-a716-446655440023"",
            //        ""userId"": ""3794bd26-9b0a-4ff3-b3bb-83a2d2dc8030"",
            //        ""name"": ""Late 90's"",
            //        ""description"": ""All the best from the late 90's in every genre"",
            //        ""deezerTrackIds"": [
            //        ""3135556""
            //        ],
            //        ""dateCreation"": ""2023-07-29T14:48:00Z"",
            //        ""dateUpdated"": ""2023-07-29T14:48:00Z""
            //    }
            //]";

            //List<Playlist> playlists = JsonConvert.DeserializeObject<List<Playlist>>(jsonPlaylists);
            //UserPlaylists = new ObservableCollection<Playlist>(playlists);

        }

        // PROPERTIES //

        // public User ActiveUser { get { return _activeUser; } }

        public ObservableCollection<Playlist> UserPlaylists
        {
            get => _userPlaylists;
            set
            {
                if (_userPlaylists != value)
                {
                    _userPlaylists = value;
                    OnPropertyChanged();
                }
            }
        }

        public Playlist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                if (value != _selectedPlaylist)
                {
                    _selectedPlaylist = value;
                    OnPropertyChanged();
                    Description = _selectedPlaylist.Description;
                    Name = _selectedPlaylist.Name;
                    LoadTracksForPlaylist(_selectedPlaylist);
                }
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


        private async Task InitializationAsync()
        {
            await LoadUserPlaylists();

            OrderPlaylistByDate(_userPlaylists);

            if (_userPlaylists != null && _userPlaylists.Any())
            {
                SelectedPlaylist = _userPlaylists.First();
            }
            UserPlaylists = _userPlaylists;
        }

        private async Task LoadUserPlaylists()
        {
            _userPlaylists = new ObservableCollection<Playlist>(await RetrievePlaylistsAsync(_activeUser.Id));
        }

        private void OrderPlaylistByDate(ObservableCollection<Playlist> playlistsToOrder)
        {
            if (playlistsToOrder != null)
            {
                playlistsToOrder = new ObservableCollection<Playlist>(
                playlistsToOrder.OrderByDescending(p =>
                    p.DateCreation > p.DateUpdated ? p.DateCreation : p.DateUpdated));
            }
        }

        private async void LoadTracksForPlaylist(Playlist playlist)
        {
            if (playlist == null) { return; }

            _playlistTracks = new ObservableCollection<Track>();

            foreach (var trackId in playlist.DeezerTrackIds)
            {
                var track = await FetchTrackFromDeezer(trackId);
                if (track != null)
                {
                    _playlistTracks.Add(track);
                    PlaylistTracks.Add(track);
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

            // client.DefaultRequestHeaders.Clear();
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "*");
            client.DefaultRequestHeaders.Add("api-key", _apiKey);

            var response = new HttpResponseMessage();
            try
            {
                response = await client.PostAsync(requestUri, content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while fetching the Playlists on the MongoDB server : " + ex.Message);
                await SnackBarVM.ShowSnackBarAsync("Problem while fetching the playlists", "Dismiss", () => { });
                return null;
            }

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
