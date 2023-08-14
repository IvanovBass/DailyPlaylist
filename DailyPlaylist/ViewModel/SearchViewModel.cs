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
        private readonly HttpClient _httpClient;
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
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }  
        public ICommand ItemSelectedCommand { get; }

        public ICommand SearchCommand { get; }


        public SearchViewModel()
        {
            _httpClient = new HttpClient();

            SearchCommand = new Command(PerformSearch);
            SearchResults = new ObservableCollection<Track>();

            PlayPauseCommand = new Command(async track =>
            {
                await _mediaPlayerService.PlayPauseTaskAsync(_preStoredIndex);
                Debug.WriteLine("le _preStoredIndex est : " + _preStoredIndex.ToString());
            });

            PlayFromCollectionViewCommand = new Command<Track>(async track =>
            {
                SelectedTrack = track;
                await _mediaPlayerService.PlayPauseTaskAsync(_preStoredIndex);
            });


            NextCommand = new Command<Track>(async track =>
            {
                SelectedTrack = await _mediaPlayerService.PlayNextAsync();
            });

            PreviousCommand = new Command<Track>(async track =>
            {
                SelectedTrack = await _mediaPlayerService.PlayPreviousAsync();
            });

            ItemSelectedCommand = new Command<Track>(track =>
            {
                SelectedTrack = track;
            });

        }

        private async void PerformSearch()
        {
            if (string.IsNullOrEmpty(SearchQuery))
                return;

            IsLoading = true;

            try
            {
                var jsonResponse = await _httpClient.GetStringAsync($"https://api.deezer.com/search/track?q={SearchQuery}&limit=30");
                var searchData = JsonConvert.DeserializeObject<SearchData>(jsonResponse);
                SearchResults = new ObservableCollection<Track>(searchData.Data);
                _mediaPlayerService = new MediaPlayerService(searchData.Data, -1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while searching: {ex.Message}");
                // Notify the user if needed...
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
            // add a progress bar , a loading widget or a text to inform the user of the search
        }

    }
}

// https://api.deezer.com/search?q={your_query}
// https://api.deezer.com/search/artist?q={your_query}
//  https://api.deezer.com/search/track?q={your_query}
// ...

