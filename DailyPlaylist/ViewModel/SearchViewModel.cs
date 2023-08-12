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
        private string _trackPreview;

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

        public Track SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;

                if (_selectedTrack != null)
                {
                    TrackName = _selectedTrack.Title;
                    ArtistName = _selectedTrack.Artist.Name;
                    AlbumCover = _selectedTrack.Album.CoverMedium;
                    TrackPreview = _selectedTrack.Preview;
                }
                else
                {
                    AlbumCover = "music_notes2.png";
                    TrackName = "Song";
                    ArtistName = "Artist";
                    TrackPreview = string.Empty;
                }

                OnPropertyChanged(nameof(TrackName));
                OnPropertyChanged(nameof(ArtistName));
                OnPropertyChanged(nameof(AlbumCover));
                OnPropertyChanged(nameof(TrackPreview));
            }
        }



        public string TrackName { get; set; }
        public string ArtistName { get; set; }
        public string AlbumCover { get; set; }
        public string TrackPreview
        {
            get => _trackPreview;
            set => SetProperty(ref _trackPreview, value);
        }

        public ICommand PlayPauseCommand { get; }
        public ICommand PlayFromCollectionViewCommand { get; }

        public SearchViewModel()
        {
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

        }

        private async void PerformSearch()
        {
            if (string.IsNullOrEmpty(SearchQuery))
                return;

            try
            {
                var httpClient = new HttpClient();
                var jsonResponse = await httpClient.GetStringAsync($"https://api.deezer.com/search/track?q={SearchQuery}&limit=25");
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

