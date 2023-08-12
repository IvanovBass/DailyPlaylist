using MediaManager;
using Newtonsoft.Json;
using System.Windows.Input;
using DailyPlaylist.View;

namespace DailyPlaylist.ViewModel
{
    public class SearchViewModel : BaseViewModel
    {

        private string _albumCover = "music_notes2.png";  
        public string AlbumCover
        {
            get { return _albumCover; }
            set
            {
                _albumCover = value;
                OnPropertyChanged(nameof(AlbumCover));
            }
        }

        private string _artistName = "Artist";
        public string ArtistName
        {
            get { return _artistName; }
            set
            {
                _artistName = value;
                OnPropertyChanged(nameof(ArtistName));
            }
        }

        private string _trackName = "Song";
        public string TrackName
        {
            get { return _trackName; }
            set
            {
                _trackName = value;
                OnPropertyChanged(nameof(TrackName));
            }
        }


        private string _searchQuery;

        private ObservableCollection<Track> _searchResults;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();

                // Trigger the search when the query length is 5 or more.
                if (_searchQuery.Length >= 5)
                    PerformSearch();
            }
        }



        public ObservableCollection<Track> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }

        private async void PerformSearch()
        {
            try
            {
                using (var httpClient = new HttpClient()) // Create a new instance of HttpClient
                {
                    var jsonResponse = await httpClient.GetStringAsync($"https://api.deezer.com/search/track?q={_searchQuery}&limit=25");

                    //Debug.WriteLine("JSON Response:");
                    //Debug.WriteLine(jsonResponse);

                    var searchData = JsonConvert.DeserializeObject<SearchData>(jsonResponse);

                    //Debug.WriteLine("Deserialized SearchData Object:");
                    //Debug.WriteLine(JsonConvert.SerializeObject(searchData, Formatting.Indented));

                    SearchResults = new ObservableCollection<Track>(searchData.Data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while searching: {ex.Message}");
                // To notify the user with a message ...
            }
        }
        public class SearchData
        {
            public List<Track> Data { get; set; }
            //... other properties you may want to capture.
            // add a progress bar , a loading widget or a text to inform the user of the search
        }

        // ...

        public ICommand GetTrackCommand => new Command<Track>(async (track) => await GetTrackDetailsAsync(track));

        private async Task GetTrackDetailsAsync(Track selectedTrack)
        {
            try
            {
                string apiUrl = $"https://api.deezer.com/track/{selectedTrack.Id}";
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.GetStringAsync(apiUrl);
                    Track detailedTrack = JsonConvert.DeserializeObject<Track>(response);

                    // Here, you can update any UI bindings to display the detailed track information.
                    // For example, if you have properties for AlbumCover, TrackName, ArtistName, etc. in your ViewModel:
                    AlbumCover = detailedTrack.Album.Cover;
                    TrackName = detailedTrack.Title;
                    ArtistName = detailedTrack.Artist.Name;

                    if (!string.IsNullOrEmpty(detailedTrack.Preview))
                    {
                        await PlayTrackAsync(detailedTrack.Preview);
                    }
                    else
                    {
                        // Handle case where preview URL is null or empty.
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching track details: {ex.Message}");
            }
        }

        private async Task PlayTrackAsync(string previewUrl)
        {
            await CrossMediaManager.Current.Play(previewUrl);
            Debug.WriteLine("OK je suis rentré dedans");
            Debug.WriteLine(previewUrl);
            // Here you can add any additional logic if required after starting the playback.
        }

    }
}

// https://api.deezer.com/search?q={your_query}
// https://api.deezer.com/search/artist?q={your_query}
//  https://api.deezer.com/search/track?q={your_query}

