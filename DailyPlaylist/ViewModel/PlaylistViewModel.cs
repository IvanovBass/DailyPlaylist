using DailyPlaylist.Services;
using DailyPlaylist.View;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace DailyPlaylist.ViewModel
{
    public class PlaylistViewModel : BaseViewModel
    {
        private readonly AuthService _authService ;
        public MediaPlayerService mediaPlayerService;
        private User _activeUser;
        private ObservableCollection<Playlist> _userPlaylists;
        private Playlist _selectedPlaylist;
        private ObservableCollection<Track> _playlistTracks;
        private Track _selectedTrack;
        public int preStoredIndex = 0;
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

            ItemSelectedCommand = new Command<Track>(track =>
            {
                SelectedTrack = track;
            });

            PlayPauseCommand = new Command(async track =>
            {
                if (PlaylistTracks == null || !PlaylistTracks.Any())
                {
                    await SnackBarVM.ShowSnackBarAsync("No track to play", "Dismiss", () => { });
                    return;
                }
                else
                {
                    if (NavigationState.LastVisitedPage != nameof(PlaylistPage))
                    {
                        CrossMediaManager.Current.Queue.Clear();
                        mediaPlayerService = new MediaPlayerService(PlaylistTracks.ToList(), true);
                        await mediaPlayerService.PlayPauseTaskAsync(PlaylistTracks.IndexOf(SelectedTrack));
                        NavigationState.LastVisitedPage = nameof(PlaylistPage);
                    }
                    else
                    {
                        await mediaPlayerService.PlayPauseTaskAsync(PlaylistTracks.IndexOf(SelectedTrack));
                    } 
                }
            });

            PlayFromPlaylistCollectionCommand = new Command<Track>(async track =>
            {
                SelectedTrack = track;
                var currentIndex = PlaylistTracks.IndexOf(track);
                preStoredIndex = currentIndex;

                if (NavigationState.LastVisitedPage != nameof(PlaylistPage))
                {
                    CrossMediaManager.Current.Queue.Clear();
                    mediaPlayerService = new MediaPlayerService(PlaylistTracks.ToList(), true);
                    await mediaPlayerService.PlayPauseTaskAsync(preStoredIndex);
                    NavigationState.LastVisitedPage = nameof(PlaylistPage);
                }
                else
                {
                    await mediaPlayerService.PlayPauseTaskAsync(preStoredIndex);
                }
            });

            NextCommand = new Command<Track>(async track =>
            {
                if (PlaylistTracks == null || !PlaylistTracks.Any())
                {
                    await SnackBarVM.ShowSnackBarAsync("No tracklist to be forwarded", "Dismiss", () => { });
                    return;
                }
                else
                {
                    if (NavigationState.LastVisitedPage != nameof(PlaylistPage))
                    {
                        preStoredIndex++;
                        if (preStoredIndex >= PlaylistTracks.Count)
                        {
                            preStoredIndex = 0;
                        }
                        SelectedTrack = PlaylistTracks[preStoredIndex];
                    }
                    else
                    {
                        SelectedTrack = await mediaPlayerService.PlayNextAsync();
                    }                   
                }
            });

            PreviousCommand = new Command<Track>(async track =>
            {
                if (PlaylistTracks == null || !PlaylistTracks.Any())
                {
                    await SnackBarVM.ShowSnackBarAsync("No tracklist to be backwarded", "Dismiss", () => { });
                    return;
                }
                else
                {
                    if (NavigationState.LastVisitedPage != nameof(PlaylistPage))
                    {
                        preStoredIndex--;
                        if (preStoredIndex < 0)
                        {
                            preStoredIndex = PlaylistTracks.Count - 1;
                        }
                        SelectedTrack = PlaylistTracks[preStoredIndex];
                    }
                    else
                    {
                        SelectedTrack = await mediaPlayerService.PlayPreviousAsync();
                    }  
                }
            });

            CrossMediaManager.Current.PositionChanged += (sender, args) =>
            {
                if (args.Position.TotalSeconds >= 28)
                {
                    HandleTrackFinishedPVM();
                }
            };

            LogoutViewModel.OnLogout += Reset;

        }

        // PROPERTIES //

        public ObservableCollection<Playlist> UserPlaylists
        {
            get => _userPlaylists;
            set
            {
                _userPlaylists = value;
                OnPropertyChanged(nameof(UserPlaylists));
            }
        }

        public Playlist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                if (value != _selectedPlaylist && value != null)
                {
                    _name = value.Name;
                    _description = value.Description;
                    _selectedPlaylist = value;
                    OnPropertyChanged(nameof(SelectedPlaylist));
                    LoadTracksForPlaylist(value);
                }
            }
        }

        public ObservableCollection<Track> PlaylistTracks
        {
            get => _playlistTracks;
            set
            {
                _playlistTracks = value;
                OnPropertyChanged(nameof(PlaylistTracks));
            }
        }

        public Track SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged(nameof(SelectedTrack));
                if (value != null)
                {
                    int selectedIndex = PlaylistTracks.IndexOf(value);
                    preStoredIndex = selectedIndex;

                    SelectedTrackCover = value.Album?.CoverMedium;
                    SelectedTrackTitle = value.Title;
                    SelectedTrackArtist = value.Artist?.Name;
                }
            }
        }

        public string SelectedTrackTitle
        {
            get => _selectedTrackTitle;
            set
            {
                _selectedTrackTitle = string.IsNullOrEmpty(value) ? "Title" : value;
                OnPropertyChanged(nameof(SelectedTrackTitle));
            }
        }

        public string SelectedTrackArtist
        {
            get => _selectedTrackArtist;
            set
            {
                _selectedTrackArtist = string.IsNullOrEmpty(value) ? "Artist" : value;
                OnPropertyChanged(nameof(SelectedTrackArtist));
            }
        }

        public string SelectedTrackCover
        {
            get => _selectedTrackCover;
            set
            {
                _selectedTrackCover = string.IsNullOrEmpty(value) ? "music_notes.png" : value;
                OnPropertyChanged(nameof(SelectedTrackCover));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }


        // COMMANDS //

        public ICommand PlayPauseCommand { get; }
        public ICommand PlayFromPlaylistCollectionCommand { get; }
        public ICommand DeleteFromCollectionViewCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand ItemSelectedCommand { get; }

        // METHODS //

        private async Task InitializationAsync()
        {
            await LoadUserPlaylists();

            OrderPlaylistByDate(UserPlaylists);

            if (_userPlaylists != null && _userPlaylists.Any())
            {
                SelectedPlaylist = _selectedPlaylist = _userPlaylists.First();
            }

            LoadTracksForPlaylist(_selectedPlaylist);
        }

        private async Task LoadUserPlaylists()
        {
            UserPlaylists = new ObservableCollection<Playlist>(await RetrievePlaylistsAsync(_activeUser.Id));
        }

        private void OrderPlaylistByDate(ObservableCollection<Playlist> playlistsToOrder)
        {
            if (playlistsToOrder != null)
            {
                playlistsToOrder = new ObservableCollection<Playlist>(
                playlistsToOrder.OrderByDescending(p => p.DateUpdated));  // because DateUpdated is initialized with the CreateDate = DateTimeNow and will evovle at each change
            }
        }

        private async void LoadTracksForPlaylist(Playlist playlist)
        {
            if (playlist == null) { return; }

            var cachedPlaylist = new ObservableCollection<Track>();

            foreach (var trackId in playlist.DeezerTrackIds)
            {
                var track = await FetchTrackFromDeezer(trackId);
                if (track != null)
                {
                    cachedPlaylist.Add(track);
                }
            }
            PlaylistTracks = cachedPlaylist;

            if (PlaylistTracks.Any())
            {
                SelectedTrack = PlaylistTracks[0];
                preStoredIndex = 0;
            }
            CrossMediaManager.Current.Queue.Clear();
            mediaPlayerService = new MediaPlayerService(cachedPlaylist.ToList(), false);
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

        public void HandleTrackFinishedPVM()
        {
            var mediaPlayer = mediaPlayerService;
            if (mediaPlayer != null)
            {
                var currentIndex = CrossMediaManager.Current.Queue.CurrentIndex;
                currentIndex++;
                if (currentIndex >= PlaylistTracks.Count)
                {
                    currentIndex = 0;
                }
                preStoredIndex = currentIndex;
                SelectedTrack = PlaylistTracks[currentIndex];
            }
        }

        private void Reset()
        {

        }
    }
}
