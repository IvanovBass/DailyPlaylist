using Newtonsoft.Json;

namespace DailyPlaylist.ViewModel
{
    public class SearchViewModel : BaseViewModel
    {

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
    }
    // add a progress bar , aloading handler  or text to inform the user of the search
    // You'll need a 'SearchData' class to help with deserialization, which may look something like this:
    public class SearchData
    {
        public List<Track> Data { get; set; }
        //... other properties you may want to capture.
    }

}

// https://api.deezer.com/search?q={your_query}
// https://api.deezer.com/search/artist?q={your_query}
//  https://api.deezer.com/search/track?q={your_query}

