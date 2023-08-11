using Newtonsoft.Json;
using System.Windows.Input;
using DailyPlaylist.Model;


namespace DailyPlaylist.ViewModel
{
    public class PlaylistConfigViewModel : BaseViewModel
    {

        private ObservableCollection<Genre> _genres;

        HttpClient httpClient = DependencyService.Get<HttpClient>();
        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set
            {
                _genres = value;
                OnPropertyChanged();
            }
        }
        public ICommand LoadGenresCommand { get; }

        public PlaylistConfigViewModel()
        {
            // we can invoke an async LoadGenres in the constructor ... so ....
            LoadGenresCommand = new Command(async () => await LoadGenres());
        }


        private async Task LoadGenres()
        {
            try
            {
        
                var jsonResponse = await httpClient.GetStringAsync($"https://api.deezer.com/genre");
                var genresData = JsonConvert.DeserializeObject<GenreData>(jsonResponse);
                Genres = new ObservableCollection<Genre>(genresData.Data);
            
            }
            catch (HttpRequestException httpEx)
            {
                // We handle here network-related errors, like connectivity issues, time-outs etc.....
                Debug.WriteLine($"Network error while fetching genres: {httpEx.Message}");
                MessagingCenter.Send(this, "Error", "There was a problem fetching the genres. Please check your internet connection and" +
                    " try again.");
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine($"Error parsing genre data: {jsonEx.Message}");
                MessagingCenter.Send(this, "Error", "There was a problem fetching the genres.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error while fetching genres: {ex.Message}");
                MessagingCenter.Send(this, "Error", "Unexpected error while fetching the genres. Please retry...");
            }
        }

        // We will basically fetch already existing Deezer playlists, otherwise it will be too complicated to search Tracks with
        // algorithmic complexity, due to how the models and proerpties are structured and accessible through our API requests

        public async Task<List<Album>> SearchAlbumsAsync(string query)
        {

            var jsonResponse = await httpClient.GetStringAsync($"https://api.deezer.com/search/album?q={query}");
            var searchResults = JsonConvert.DeserializeObject<SearchAlbumsResponse>(jsonResponse);
            return searchResults.Albums;
            
        }





        public class GenreData
        {
            public List<Genre> Data { get; set; }
      
        }

        public class SearchAlbumsResponse
        {
            public List<Album> Albums { get; set; }
            // and other properties to capture if needed ....
        }
    }

    
}

