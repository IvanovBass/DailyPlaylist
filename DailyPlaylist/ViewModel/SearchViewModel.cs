namespace DailyPlaylist.ViewModel
{
    public class SearchViewModel : BaseViewModel
    {
        private ObservableCollection<Track> _searchResults;
        private string _searchQuery;
        private Track _selectedTrack;
        private string _trackName = "Song";
        private string _artistName = "Artist";
        private string _albumCover = "music_notes2.png";
        private string _trackPreview;
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

        public string TrackPreview
        {
            get => _trackPreview;
            set => SetProperty(ref _trackPreview, value);
        }

        public Track SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                SetProperty(ref _selectedTrack, value);
                if (value != null)
                {
                    AlbumCover = value.Album?.CoverMedium;
                    TrackName = value.Title;
                    ArtistName = value.Artist?.Name;
                    TrackPreview = value.Preview;
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
        public ICommand ItemSelectedCommand { get; }

        public ICommand SearchCommand { get; }


        public SearchViewModel()
        {
            _httpClient = new HttpClient();

            SearchCommand = new Command(PerformSearch);
            SearchResults = new ObservableCollection<Track>();

            PlayPauseCommand = new Command(async () =>
            {
                var currentMediaItem = CrossMediaManager.Current.Queue.Current;

                // Will check if a track is currently playing and if it's the same as the selected track !
                if (CrossMediaManager.Current.IsPlaying() && currentMediaItem != null && currentMediaItem.MediaUri == SelectedTrack.Preview)
                {
                    await CrossMediaManager.Current.Pause();
                }
                else
                {
                    // Stops the current track (if a track is currently playing)
                    if (currentMediaItem != null)
                    {
                        await CrossMediaManager.Current.Stop();
                    }

                    // Play the selected track
                    if (!string.IsNullOrEmpty(TrackPreview))
                    {
                        try
                        {
                            await CrossMediaManager.Current.Play(TrackPreview);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error playing track: {ex.Message}");
                            await ShowSnackBarAsync("Unable to play track", "Dismiss", () => { });
                        }
                    }
                    else
                    {
                        await ShowSnackBarAsync("Preview not available", "Dismiss", () => { });
                    }
                }
            });


            PlayFromCollectionViewCommand = new Command<Track>(async track =>
            {
                SelectedTrack = track;
                if (string.IsNullOrEmpty(track.Preview))
                {
                    await ShowSnackBarAsync("Preview not available", "Dismiss", () => { });
                    return;
                }
                try
                {
                    await CrossMediaManager.Current.Play(track.Preview);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error playing track: {ex.Message}");
                    await ShowSnackBarAsync("Unable to play track", "Dismiss", () => { });
                }
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

        public async Task ShowSnackBarAsync(string message, string actionText, Action action, int durationInSeconds = 3)
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


    }
}

// https://api.deezer.com/search?q={your_query}
// https://api.deezer.com/search/artist?q={your_query}
//  https://api.deezer.com/search/track?q={your_query}
// ...

