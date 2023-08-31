using DailyPlaylist.Services;
using DailyPlaylist.View;
using MauiAppDI.Helpers;
using MediaManager.Library;
using MediaManager.Media;
using Newtonsoft.Json.Linq;

namespace DailyPlaylist.ViewModel
{
    public class PlaylistViewModel : BaseViewModel
    {
        private User _activeUser;
        private ObservableCollection<Tracklist> _userPlaylists;
        private Tracklist _selectedPlaylist;
        private ObservableCollection<Track> _playlistTracks;
        private Track _selectedTrackPVM;
        public int preStoredIndexPVM = 0;
        private string _selectedTrackTitle = "Title";
        private string _selectedTrackArtist = "Artist";
        private string _selectedTrackCover = "music_notes.png";
        private string _description = "Describe what makes this playlist special ...";
        private string _name = "Name ...";
        private HttpService _httpService;
        public event Action SelectedPlaylistChanged;
        public event Action PromptEditEvent;
        public event Action PromptCreateEvent;
        public static int attemtps = 0;
        


        // CONSTRUCTOR //

        public PlaylistViewModel(User authUser)
        {

            ActiveUser = authUser;

            _httpService = ServiceHelper.GetService<HttpService>();

            SelectedPlaylistChanged?.Invoke();

            PlayPauseCommand = new Command(async track => await PlayPauseAsync());
            PlayFromPlaylistCollectionCommand = new Command<Track>(async track => await PlayFromPlaylistCollectionAsync(track));
            NextCommand = new Command<Track>(async track => await PlayNextAsync());
            PreviousCommand = new Command<Track>(async track => await PlayPreviousAsync());
            DeleteFromPlaylistCollectionCommand = new Command<Track>(async track => await DeleteFromPlaylistCollectionAsync(track));
            DeleteCommand = new Command(async () => await DeleteAsync());
            SaveCommand = new Command(async () => await SaveAsync());

            ItemSelectedCommand = new Command<Track>(track =>
            {
                SelectedTrackPVM = track;
            });

            EditCommand = new Command(void () =>
            {
                PromptEditEvent?.Invoke();
            });

            CreateCommand = new Command(void () =>
            {
                PromptCreateEvent?.Invoke();
            });

            MediaPlayerService.OnItemChanged += SynchronizedSelectedItemPVM;

            _ = InitializationPlaylistsAsync();

        }

        // PROPERTIES //

        public User ActiveUser
        {
            get => _activeUser;
            set
            {
                _activeUser = value;
            }
        }

        public ObservableCollection<Tracklist> UserPlaylists
        {
            get => _userPlaylists;
            set
            {
                if (value != null && value != _userPlaylists)
                {
                    _userPlaylists = value;
                    OnPropertyChanged(nameof(UserPlaylists));
                }
            }
        }

        public Tracklist SelectedPlaylist
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

        public Track SelectedTrackPVM
        {
            get => _selectedTrackPVM;
            set
            {
                _selectedTrackPVM = value;
                OnPropertyChanged(nameof(SelectedTrackPVM));
                if (value != null)
                {
                    int selectedIndex = PlaylistTracks.IndexOf(value);
                    preStoredIndexPVM = selectedIndex;

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
            get => _selectedPlaylist?.Description;
            set
            {
                if (_selectedPlaylist.Description != value)
                {
                    _selectedPlaylist.Description = value;
                    OnPropertyChanged(nameof(Description));

                }
            }
        }

        public string Name
        {
            get => _selectedPlaylist?.Name;
            set
            {
                if (_selectedPlaylist.Name != value)
                {
                    _selectedPlaylist.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }


        // COMMANDS //

        public ICommand PlayPauseCommand { get; }
        public ICommand PlayFromPlaylistCollectionCommand { get; }
        public ICommand DeleteFromPlaylistCollectionCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand ItemSelectedCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }



        // METHODS //

        private async Task InitializationPlaylistsAsync()
        {
            await LoadUserPlaylists();

            if (UserPlaylists != null && UserPlaylists.Any())
            {
                UserPlaylists = OrderPlaylistByDate(UserPlaylists); // Order the Playlists by DateUpdated
                SelectedPlaylist = _selectedPlaylist = UserPlaylists.First();
                LoadTracksForPlaylist(_selectedPlaylist);
            }
            else
            {
                UserPlaylists = new ObservableCollection<Tracklist>();
            }
        }

        private async Task LoadUserPlaylists()
        {
            if (_activeUser == null) { return; }
            var playlists = await RetrievePlaylistsAsync(_activeUser.Id);
            if (playlists != null)
            {
                UserPlaylists = new ObservableCollection<Tracklist>(playlists);
            }           
        }

        private ObservableCollection<Tracklist> OrderPlaylistByDate(ObservableCollection<Tracklist> playlistsToOrder)
        {
            if (playlistsToOrder != null)
            {
                playlistsToOrder = new ObservableCollection<Tracklist>(
                playlistsToOrder.OrderByDescending(p => p.DateUpdated));
                // because DateUpdated is initialized with the CreateDate = DateTimeNow and will evovle at each change
            }
            return playlistsToOrder;

        }

        public void LoadTracksForPlaylist(Tracklist playlist)
        {
            if (playlist == null) { return; }

            var cachedPlaylist = new ObservableCollection<Track>(playlist.DeezerTracks);

            PlaylistTracks = cachedPlaylist;

            if (PlaylistTracks.Any())
            {
                SelectedTrackPVM = PlaylistTracks[0];
                preStoredIndexPVM = 0;
            }
            CrossMediaManager.Current.Queue.Clear();
            MediaPlayerService.Initialize(PlaylistTracks.ToList(), false);
            SelectedPlaylistChanged?.Invoke();
            
        }

        public async Task<List<Tracklist>> RetrievePlaylistsAsync(string playlistUserId)
        {
            var action = "find";
            var payload = new
            {
                collection = "Playlist",
                database = "DailyPlaylist",
                dataSource = "DailyPlaylistMongoDB",
                filter = new { userId = playlistUserId }
            };

            var response = await _httpService.MakeHttpRequestAsync(action, payload);

            var playlists = new List<Tracklist>();

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(responseData);
                var playlistsJson = jsonObject["documents"]?.ToString();
                Debug.WriteLine($"Retrieved Playlists from server: {playlistsJson}");

                if (playlistsJson.Contains(playlistUserId))
                {
                    playlists = JsonConvert.DeserializeObject<List<Tracklist>>(playlistsJson);
                }
            }
            else
            {
                Debug.WriteLine("Error while fetching the Playlists from the server");
            }

            return playlists;

        }  

        public async Task<bool> DeletePlaylistAsync(string playlistId)
        {
            var action = "deleteOne";
            var payload = new
            {
                dataSource = "DailyPlaylistMongoDB",
                database = "DailyPlaylist",
                collection = "Playlist",
                filter = new
                {
                    _id = playlistId
                }
            };

            var response = await _httpService.MakeHttpRequestAsync(action, payload);

            if (response.IsSuccessStatusCode)
            {
                await SnackBarVM.ShowSnackBarAsync("Playlist succesfully deleted", "Dismiss", () => { });
                return true;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error deleting playlist: {errorResponse}");
                await SnackBarVM.ShowSnackBarAsync("Error while deleting your Playlist at server-side, please retry", "Dismiss", () => { });
                return false;
            }
        }

        public async Task<Tracklist> InsertNewPlaylistAsync(Tracklist newPlaylist)
        {
            if (newPlaylist == null) return null;

            var action = "insertOne";
            var payload = new
            {
                collection = "Playlist",
                database = "DailyPlaylist",
                dataSource = "DailyPlaylistMongoDB",
                document = new
                {
                    _id = newPlaylist.Id,
                    userId = newPlaylist.UserId,
                    name = newPlaylist.Name,
                    description = newPlaylist.Description,
                    deezerTracks = newPlaylist.DeezerTracks,
                    dateCreation = newPlaylist.DateCreation.ToString("s") + "Z",
                    dateUpdated = newPlaylist.DateUpdated.ToString("s") + "Z",
                }
            };

            var response = await _httpService.MakeHttpRequestAsync(action, payload);

            if (response.IsSuccessStatusCode)
            {
                return newPlaylist;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error while inserting new playlist: {error}");
                return null;
            }
        }

        public async Task<bool> UpdatePlaylistAsync(Tracklist updatedPlaylist)
        {
            if (updatedPlaylist == null) return false;

            if (PlaylistTracks is  null) return false;  

            var tracks = PlaylistTracks.ToList();

            var action = "updateOne";
            var payload = new
            {
                collection = "Playlist",
                database = "DailyPlaylist",
                dataSource = "DailyPlaylistMongoDB",
                filter = new
                {
                    _id = updatedPlaylist.Id
                },
                update = new
                {
                    userId = updatedPlaylist.UserId,
                    name = updatedPlaylist.Name,
                    description = updatedPlaylist.Description,
                    deezerTracks = tracks,
                    dateCreation = updatedPlaylist.DateCreation.ToString("s") + "Z",
                    dateUpdated = DateTime.Now.ToString("s") + "Z",
                }
            };

            var response = await _httpService.MakeHttpRequestAsync(action, payload);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error while updating playlist: {error}");
                await SnackBarVM.ShowSnackBarAsync("Error while reaching the server to save your playlist, please retry;)", "Dismiss", () => { });
                return false;
            }
        }

        // COMMAND METHODS //

        private async Task PlayPauseAsync()
        {
            if (!CrossMediaManager.Current.Queue.HasCurrent)
            {
                if (PlaylistTracks.Any())
                {
                    await MediaPlayerService.PlayPauseTaskAsync(0, false);
                }
                await SnackBarVM.ShowSnackBarAsync("No track to play", "Dismiss", () => { });
                return;
            }
            else
            {
                if (SelectedTrackPVM != null)
                {
                    if (NavigationState.LastPlayerUsed != "PVM")
                    {
                        CrossMediaManager.Current.Queue.Clear();
                        MediaPlayerService.Initialize(PlaylistTracks.ToList(), false);
                    }
                    await MediaPlayerService.PlayPauseTaskAsync(PlaylistTracks.IndexOf(SelectedTrackPVM), true);
                    NavigationState.LastPlayerUsed = "PVM";
                }
                await MediaPlayerService.PlayPauseTaskAsync(0, false);

            }
        }

        private async Task PlayFromPlaylistCollectionAsync(Track track)
        {
            SelectedTrackPVM = track;
            // a ce moment là preStoredIndex est recalculé sur PlaylistTrack dans le setter de selectedTrack

            if (NavigationState.LastPlayerUsed != "PVM")
            {
                CrossMediaManager.Current.Queue.Clear();
                MediaPlayerService.Initialize(PlaylistTracks.ToList(), false);
            }
            await MediaPlayerService.PlayPauseTaskAsync(preStoredIndexPVM, true);
            NavigationState.LastPlayerUsed = "PVM";
        }

        private async Task PlayNextAsync()
        {
            await MediaPlayerService.PlayNextAsync();
        }

        private async Task PlayPreviousAsync()
        {
            await MediaPlayerService.PlayPreviousAsync();
        }

        private async Task DeleteFromPlaylistCollectionAsync(Track track)
        {
            int index = PlaylistTracks.IndexOf(track);
            
            if (index >= 0)
            {

                SelectedPlaylist.DeezerTracks.Remove(track);
                PlaylistTracks.Remove(track);
                if (index < MediaPlayerService._mediaItems.Count)
                {
                    var mediaItem = MediaPlayerService._mediaItems[index];
                    MediaPlayerService._mediaItems.Remove(mediaItem);
                    var timeSpan = CrossMediaManager.Current.Position;
                    CrossMediaManager.Current.Queue.Remove(mediaItem);
                    await CrossMediaManager.Current.Stop();
                    await CrossMediaManager.Current.Queue.UpdateMediaItems();
                    await CrossMediaManager.Current.Play();
                    await CrossMediaManager.Current.SeekTo(timeSpan);
                }
                await SnackBarVM.ShowSnackBarShortAsync($"Song '{track.Title}' removed from playlist '{SelectedPlaylist.Name}'!", "OK", () => { });
            }
            else
            {
                await SnackBarVM.ShowSnackBarShortAsync($"Song '{track.Title}' not found in playlist '{SelectedPlaylist.Name}'", "OK", () => { });
            }
            
            NavigationState.refreshFavoritesNeeded = true;
        }

        private async Task DeleteAsync()
        {
            bool confirmDelete = await Application.Current.MainPage.DisplayAlert("Delete Playlist", "Are you sure you want to delete the current playlist?", "Yes", "No");
            if (!confirmDelete) return;

            bool deletionSuccess = await DeletePlaylistAsync(SelectedPlaylist.Id);
            if (deletionSuccess)
            {
                UserPlaylists.Remove(SelectedPlaylist);
                SelectedPlaylist = UserPlaylists.OrderByDescending(p => p.DateUpdated).FirstOrDefault();
            }
            else
            {
                await SnackBarVM.ShowSnackBarAsync($"Error while deleting the Playlist at server side, please retry", "OK", () => { });
            }
            NavigationState.refreshFavoritesNeeded = true;
        }

        private async Task SaveAsync()
        {
            bool isUpdateSuccessful = await UpdatePlaylistAsync(SelectedPlaylist);

            if (isUpdateSuccessful)
            {
                await SnackBarVM.ShowSnackBarAsync($"Playlist '{SelectedPlaylist.Name}' successfully saved! Good job!", "OK", () => { });
            }
            else
            {
                await SnackBarVM.ShowSnackBarAsync("Failed to save the playlist. Please try again.", "Dismiss", () => { });
            }
        }

        public void SynchronizedSelectedItemPVM ()
        {
            IMediaItem media = CrossMediaManager.Current.Queue.Current;
            if (media == null) return;
            _selectedTrackPVM = null;
            SelectedTrackTitle = media.Title; 
            SelectedTrackArtist = media.Artist;
            SelectedTrackCover = media.AlbumImageUri;
        }
    }
}

