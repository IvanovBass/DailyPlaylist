using DailyPlaylist.Services;
using DailyPlaylist.View;
using MauiAppDI.Helpers;
using MediaManager.Library;
using MediaManager.Media;

namespace DailyPlaylist.ViewModel
{
    public class SearchViewModel : BaseViewModel
    {
        private PlaylistViewModel _playlistViewModel;
        private ObservableCollection<Track> _searchResults;
        public int preStoredIndex = 0; 
        private string _searchQuery;
        private Track _selectedTrack;
        private string _trackName = "Song";
        private string _artistName = "Artist";
        private string _albumCover = "music_notes2.png";
        private HttpClient _client;
        private bool _isLoading;

        // PROPERTIES //
        public ObservableCollection<Track> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SetProperty(ref _searchQuery, value);
            }
        }

        public string TrackName
        {
            get => _trackName;
            set => SetProperty(ref _trackName, string.IsNullOrEmpty(value) ? "Song" : value);
        }

        public string ArtistName
        {
            get => _artistName;
            set => SetProperty(ref _artistName, string.IsNullOrEmpty(value) ? "Artist" : value);
            // By doing so we no longer need :  OnPropertyChanged(nameof(ArtistName));
        }

        public string AlbumCover
        {
            get => _albumCover;
            set => SetProperty(ref _albumCover, string.IsNullOrEmpty(value) ? "music_notes2.png" : value);
        }

        public Track SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                SetProperty(ref _selectedTrack, value);
                if (value != null)
                {
                    int selectedIndex = SearchResults.IndexOf(value);
                    preStoredIndex = selectedIndex;

                    AlbumCover = value.Album?.CoverMedium;
                    TrackName = value.Title;
                    ArtistName = value.Artist?.Name;
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public PlaylistViewModel PlaylistViewModel
        {
            get
            {
                return _playlistViewModel;
            }
            set
            {
                _playlistViewModel = value;
            }
        }

        // COMMANDS //

        public ICommand PlayPauseCommand { get; }
        public ICommand PlayFromCollectionViewCommand { get; }
        public ICommand SetFavoriteCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }  
        public ICommand ItemSelectedCommand { get; }

        public ICommand SearchCommand { get; }


        // CONSTRUCTOR //

        public SearchViewModel(PlaylistViewModel playlistViewModel)
        {

            _client = ServiceHelper.GetService<HttpClient>();

            _playlistViewModel =  playlistViewModel;

            SearchResults = new ObservableCollection<Track>();

            SearchCommand = new Command(PerformSearch);
            PlayPauseCommand = new Command(async (track) => await HandlePlayPause());
            PlayFromCollectionViewCommand = new Command<Track>(async (track) => await HandlePlayFromCollectionView(track));
            SetFavoriteCommand = new Command<Track>(async (track) => await HandleSetFavorite(track));
            NextCommand = new Command<Track>(async (track) => await HandleNext());
            PreviousCommand = new Command<Track>(async (track) => await HandlePrevious());

            ItemSelectedCommand = new Command<Track>(track =>
            {
                SelectedTrack = track;
            });

            _playlistViewModel.SelectedPlaylistChanged += LoadSelectedFavoriteTrackUris;
            MediaPlayerService.OnItemChanged += SynchronizedSelectedItemSVM;

        }

        // COMMANDS METHODS //

        private async void PerformSearch()
        {
            if (string.IsNullOrEmpty(SearchQuery))
                return;

            IsLoading = true;

            try
            {
                var jsonResponse = await _client.GetStringAsync($"https://api.deezer.com/search/track?q={SearchQuery}&limit=30");
                var searchData = JsonConvert.DeserializeObject<SearchData>(jsonResponse);
                if (searchData.Data is List<Track> tracks && tracks.Any())
                {
                    SearchResults = new ObservableCollection<Track>(searchData.Data);
                    if (SearchResults.Any())
                    {
                        SelectedTrack = SearchResults[0];
                    }
                    NavigationState.LastPlayerUsed = "SVMprevious";
                }
                else
                {
                    await SnackBarVM.ShowSnackBarAsync("No tracks found for your search query", "Dismiss", () => { });
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while searching: {ex.Message}");
                IsLoading = false;
                await SnackBarVM.ShowSnackBarAsync("Connexion can't be made, please retry", "Dismiss", () => { });
            }
            finally
            {
                await Task.Delay(2000);
                LoadSelectedFavoriteTrackUris();
                IsLoading = false;
            }
        }

        private async Task HandlePlayPause()
        {
            if (!CrossMediaManager.Current.Queue.HasCurrent)
            {
                if (SearchResults.Any())
                {
                    CrossMediaManager.Current.Queue.Clear();
                    MediaPlayerService.Initialize(SearchResults.ToList(), false);
                    await MediaPlayerService.PlayPauseTaskAsync(0, false);
                }
                await SnackBarVM.ShowSnackBarAsync("No track to play", "Dismiss", () => { });
                return;
            }
            if (NavigationState.LastPlayerUsed != "SVM")
            {
                if (SelectedTrack != null)
                {
                    CrossMediaManager.Current.Queue.Clear();
                    MediaPlayerService.Initialize(SearchResults.ToList(), false);
                    NavigationState.LastPlayerUsed = "SVM";
                    await MediaPlayerService.PlayPauseTaskAsync(SearchResults.IndexOf(SelectedTrack), true);
                } else
                {
                    await MediaPlayerService.PlayPauseTaskAsync(0, false);
                }
            }
            else
            {
                if (SelectedTrack != null)
                {
                    await MediaPlayerService.PlayPauseTaskAsync(SearchResults.IndexOf(SelectedTrack), true);
                    NavigationState.LastPlayerUsed = "SVM";
                } else
                {
                    await MediaPlayerService.PlayPauseTaskAsync(0, false);
                }
            }
            
        }

        private async Task HandlePlayFromCollectionView(Track track)
        {
            SelectedTrack = track;

            if (NavigationState.LastPlayerUsed != "SVM")
            {
                CrossMediaManager.Current.Queue.Clear();
                MediaPlayerService.Initialize(SearchResults.ToList(), false);
            }
            await MediaPlayerService.PlayPauseTaskAsync(preStoredIndex, true);
            NavigationState.LastPlayerUsed = "SVM";
        }

        private async Task HandleSetFavorite(Track track)
        {
            if (_playlistViewModel.SelectedPlaylist == null)
            {
                await SnackBarVM.ShowSnackBarShortAsync("No playlist selected. Please select a playlist first.", "OK", () => { });
                return;
            }

            track.Favorite = !track.Favorite;

            var id = track.Id;

            NavigationState.refreshFavoritesNeeded = true;

            if (track.Favorite)
            {
                _playlistViewModel.SelectedPlaylist.DeezerTracks.Add(track);
                await SnackBarVM.ShowSnackBarShortAsync("'" + track.Title + "' succesfully added to playlist '" + _playlistViewModel.SelectedPlaylist.Name + "' !", "OK", () => { });
            }
            else
            {
                var tracks = _playlistViewModel.SelectedPlaylist.DeezerTracks;

                var trackToRemove = tracks.FirstOrDefault(t => t.Id == id);
                if (trackToRemove != null)
                {
                    tracks.Remove(trackToRemove);
                }
                await SnackBarVM.ShowSnackBarShortAsync("'" + track.Title + "' removed from playlist '" + _playlistViewModel.SelectedPlaylist.Name + "' !", "OK", () => { });

                _playlistViewModel.SelectedPlaylist.DeezerTracks = tracks;
            }
            await CrossMediaManager.Current.Queue.UpdateMediaItems();

        }

        private async Task HandleNext()
        {
            await MediaPlayerService.PlayNextAsync();
        }

        private async Task HandlePrevious( )
        {
            await MediaPlayerService.PlayPreviousAsync();
        }

        // BACK-END METHODS  //

        public class SearchData
        {
            public List<Track> Data { get; set; }
            //... other properties or objects to catch eventually
        }

        public void LoadSelectedFavoriteTrackUris()
        {
            if (_playlistViewModel.SelectedPlaylist is null) { return; }

            var favoriteTrackIds = new HashSet<long>(_playlistViewModel.SelectedPlaylist.DeezerTracks.Select(t => t.Id));

            foreach (var track in SearchResults.ToList())
            {
                track.Favorite = favoriteTrackIds.Contains(track.Id);
            }
        }
        // By using a HashSet, the lookup operation Contains is O(1) on average, making this approach much faster than searching through the list of tracks for each track in SearchResults.
        // The advantage of a HashSet is its constant-time average complexity for lookups, which makes it efficient regardless of the type of data it contains.

        public void SynchronizedSelectedItemSVM()
        {
            IMediaItem media = CrossMediaManager.Current.Queue.Current;
            if (media == null) return;
            _selectedTrack = null;
            TrackName = media.Title;
            ArtistName = media.Artist;
            AlbumCover = media.AlbumImageUri;
            
        }
    }
}

// https://api.deezer.com/search?q={your_query}
// https://api.deezer.com/search/artist?q={your_query}
//  https://api.deezer.com/search/track?q={your_query}
// ...

