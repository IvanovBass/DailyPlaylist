using DailyPlaylist.Services;

namespace DailyPlaylist.ViewModel
{
    public class SearchViewModel : BaseViewModel
    {
        private ObservableCollection<Track> _searchResults;
        private int _preStoredIndex; 
        private MediaPlayerService _mediaPlayerService;
        private string _searchQuery;
        private Track _selectedTrack;
        private string _trackName = "Song";
        private string _artistName = "Artist";
        private string _albumCover = "music_notes2.png";
        private Lazy<HttpClient> _httpClient = new Lazy<HttpClient>();
        private bool _isLoading;
        // private PlaylistViewModel _playlistViewModel;  // I'm adding a ref to the PlaylistVM for the SetFavoriteCommand


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

        public MediaPlayerService MediaPlayerService
        {
            get => _mediaPlayerService;
            set
            {
                SetProperty(ref _mediaPlayerService, value);
                if (value != null)
                {
                    _mediaPlayerService.TrackFinished += HandleTrackFinished;
                }
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
                    _preStoredIndex = selectedIndex;

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

        public ICommand PlayPauseCommand { get; }
        public ICommand PlayFromCollectionViewCommand { get; }
        public ICommand SetFavoriteCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }  
        public ICommand ItemSelectedCommand { get; }

        public ICommand SearchCommand { get; }


        public SearchViewModel()  // will probably have to put the Active Playlist or the PlaylisVM in DI
        {

            SearchCommand = new Command(PerformSearch);
            SearchResults = new ObservableCollection<Track>();

            PlayPauseCommand = new Command(async track =>
            {
                if (SearchResults == null || !SearchResults.Any() || _mediaPlayerService == null)
                {
                    await ShowSnackBarAsync("No track to play", "Dismiss", () => { });
                    return;
                }
                else
                {
                    await _mediaPlayerService.PlayPauseTaskAsync(_preStoredIndex);
                }
            });

            PlayFromCollectionViewCommand = new Command<Track>(async track =>
            {
                SelectedTrack = track;
                await _mediaPlayerService.PlayPauseTaskAsync(_preStoredIndex);
            });

            SetFavoriteCommand = new Command<Track>(track =>
            {
                //SelectedTrack = track;

                //var activePlaylist = _playlistViewModel.ActivePlaylist;

                //if (activePlaylist == null)
                //{
                //    activePlaylist = new Playlist
                //    {
                //        User = new User(),
                //        Name = "Name",
                //        Description = "Description",
                //        Tracks = new List<Track>()
                //    };

                //    _playlistViewModel.ActivePlaylist = activePlaylist;
                //}

                //if (!activePlaylist.Tracks.Contains(track))
                //{
                //    activePlaylist.Tracks.Add(track);
                //}

            });


            NextCommand = new Command<Track>(async track =>
            {
                if (SearchResults == null || !SearchResults.Any() || _mediaPlayerService == null)
                {
                    await ShowSnackBarAsync("No tracklist to be forwarded", "Dismiss", () => { });
                    return;
                }
                else
                {
                    SelectedTrack = await _mediaPlayerService.PlayNextAsync();
                }
            });

            PreviousCommand = new Command<Track>(async track =>
            {
                if (SearchResults == null || !SearchResults.Any() || _mediaPlayerService == null)
                {
                    await ShowSnackBarAsync("No tracklist to be backwarded", "Dismiss", () => { });
                    return;
                } 
                else
                {
                    SelectedTrack = await _mediaPlayerService.PlayPreviousAsync();
                }
            });

            ItemSelectedCommand = new Command<Track>(track =>
            {
                SelectedTrack = track;
            });

            LogoutViewModel.OnLogout += Reset;

        }

        private void HandleTrackFinished()
        {
            var currentIndex = SearchResults.IndexOf(SelectedTrack);
            currentIndex++;
            if (currentIndex >= SearchResults.Count)
            {
                currentIndex = 0;
            }
            _selectedTrack = SearchResults[currentIndex];
        }


        private async void PerformSearch()
        {
            if (string.IsNullOrEmpty(SearchQuery))
                return;

            IsLoading = true;

            var client = _httpClient.Value;

            try
            {
                var jsonResponse = await client.GetStringAsync($"https://api.deezer.com/search/track?q={SearchQuery}&limit=30");
                var searchData = JsonConvert.DeserializeObject<SearchData>(jsonResponse);
                SearchResults = new ObservableCollection<Track>(searchData.Data);

                if (SearchResults.Any())
                {
                    _mediaPlayerService = new MediaPlayerService(searchData.Data);
                    SelectedTrack = SearchResults[0];
                }
                else
                {
                    await ShowSnackBarAsync("No tracks found for your search query", "Dismiss", () => { });
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while searching: {ex.Message}");
                IsLoading = false;
                await ShowSnackBarAsync("Connexion can't be made, please retry", "Dismiss", () => { });
            }
            finally
            {
                await Task.Delay(2000);
                IsLoading = false;
            }
        }

        public class SearchData
        {
            public List<Track> Data { get; set; }
            //... other properties or objects to catch eventually
        }

        public async Task ShowSnackBarAsync(string message, string actionText, Action action, int durationInSeconds = 2)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.DarkSlateBlue,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Orange,
                CornerRadius = new CornerRadius(10),

                Font = Microsoft.Maui.Font.SystemFontOfSize(13),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(13),
                CharacterSpacing = 0.1
            };

            var snackbar = Snackbar.Make(message, action, actionText, TimeSpan.FromSeconds(durationInSeconds), snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
        }

        public void Reset()
        {
            SearchResults = new ObservableCollection<Track>();
            _mediaPlayerService = null;
            _mediaPlayerService = new MediaPlayerService(new List<Track>());
            _preStoredIndex = 0;
            SearchQuery = string.Empty;
            SelectedTrack = null;
            TrackName = "Song";
            ArtistName = "Artist";
            AlbumCover = "music_notes2.png";
            IsLoading = false;
        }

    }
}

// https://api.deezer.com/search?q={your_query}
// https://api.deezer.com/search/artist?q={your_query}
//  https://api.deezer.com/search/track?q={your_query}
// ...

