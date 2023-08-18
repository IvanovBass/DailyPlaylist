using DailyPlaylist.Services;
using MauiAppDI.Helpers;

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
            //var mediaPlayerService = ServiceHelper.GetService<MediaPlayerService>();

            //if (mediaPlayerService != null) 
            //{ 
            //    _mediaPlayerService = mediaPlayerService;
            //}
            //else
            //{
            //    _mediaPlayerService = new MediaPlayerService(new List<Track>());
            //}

            SearchCommand = new Command(PerformSearch);
            SearchResults = new ObservableCollection<Track>();

            PlayPauseCommand = new Command(async track =>
            {
                if (SearchResults == null || !SearchResults.Any() || _mediaPlayerService == null)
                {
                    await SnackBarVM.ShowSnackBarAsync("No track to play", "Dismiss", () => { });
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
                // we'll need the PLaylistVM as a singleton to call it ?

            });


            NextCommand = new Command<Track>(async track =>
            {
                if (SearchResults == null || !SearchResults.Any() || _mediaPlayerService == null)
                {
                    await SnackBarVM.ShowSnackBarAsync("No tracklist to be forwarded", "Dismiss", () => { });
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
                    await SnackBarVM.ShowSnackBarAsync("No tracklist to be backwarded", "Dismiss", () => { });
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


            CrossMediaManager.Current.PositionChanged += (sender, args) =>
            {
                if (args.Position.TotalSeconds >= 28)
                {
                    HandleTrackFinished();
                }
            };

            LogoutViewModel.OnLogout += Reset;

        }

        public void HandleTrackFinished()
        {
            var currentMediaUri = CrossMediaManager.Current.Queue.Current.MediaUri.ToString();

            var currentTrack = SearchResults.FirstOrDefault(t => t.Preview == currentMediaUri);
            if (currentTrack == null)
            {
                _preStoredIndex = 0;
                SelectedTrack = SearchResults[0];
                return;
            }

            var currentIndex = SearchResults.IndexOf(currentTrack);
            currentIndex++;
            if (currentIndex >= SearchResults.Count)
            {
                currentIndex = 0;
            }
            _preStoredIndex = currentIndex;
            SelectedTrack = SearchResults[currentIndex];
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
                IsLoading = false;
            }
        }

        public class SearchData
        {
            public List<Track> Data { get; set; }
            //... other properties or objects to catch eventually
        }

        public void Reset()
        {
            SearchResults = new ObservableCollection<Track>();
            _preStoredIndex = 0;
            _mediaPlayerService = null;
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

