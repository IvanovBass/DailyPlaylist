using MediaManager;
using Newtonsoft.Json;
using System.Windows.Input;
using DailyPlaylist.View;

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
                PerformSearch();
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


        public ICommand PlayPauseCommand { get; }
        public ICommand PlayFromCollectionViewCommand { get; }
        public ICommand ItemSelectedCommand { get; }


        public SearchViewModel()
        {
            _httpClient = new HttpClient();
            SearchResults = new ObservableCollection<Track>();
            PlayPauseCommand = new Command(async () =>
            {
                if (CrossMediaManager.Current.IsPlaying())
                {
                    await CrossMediaManager.Current.Pause();
                }
                else
                {
                    if (!string.IsNullOrEmpty(TrackPreview))
                    {
                        await CrossMediaManager.Current.Play(TrackPreview);
                    }
                }
            });

            PlayFromCollectionViewCommand = new Command<Track>(track =>
            {
                if (track != null)
                {
                    SelectedTrack = track; // Setting it as the selected track

                    if (!string.IsNullOrEmpty(track.Preview))
                    {
                        CrossMediaManager.Current.Play(track.Preview);
                    }
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

