using Newtonsoft.Json;
using System.Windows.Input;
using DailyPlaylist.Model;


namespace DailyPlaylist.ViewModel
{
    public class PlaylistConfigViewModel : BaseViewModel
    {

        private ObservableCollection<Genre> _genres;

        HttpClient httpClient = new HttpClient();
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

        public async Task<List<Album>> SearchAlbumsByGenreAndDecadeAsync(int genreId, string decade)
        {
            var years = GenerateYearsForDecade(decade);
            var yearsQuery = string.Join(",", years);

            var jsonResponse = await httpClient.GetStringAsync($"https://api.deezer.com/search/album?q=genre_id:\"{genreId}\" release_date:{yearsQuery} &order=FANS_DESC");
            var searchResults = JsonConvert.DeserializeObject<SearchAlbumsResponse>(jsonResponse);

            return searchResults.Albums;
        }


        private List<string> GenerateYearsForDecade(string decade)
        {
            int startYear = int.Parse(decade);  
            // je ne fais pas de check car les valeurs qui viendront de mon Front, du Picker,
           // seront systématiquement parsables en années chiffrées
            return Enumerable.Range(startYear, 10).Select(year => year.ToString()).ToList();
        }

        public List<Album> FilterAlbumsByGenreAndDecade(List<Album> albums, int genreId, string decadeStart, string decadeEnd)
        {
            return albums.Where(album => album.GenreId == genreId
                                     && DateTime.TryParse(album.ReleaseDate, out DateTime releaseDate)
                                     && releaseDate >= DateTime.Parse(decadeStart)
                                     && releaseDate < DateTime.Parse(decadeEnd)).ToList();
        }

        public async Task<List<Track>> ExtractTracksFromAlbumsAsync(List<Album> albums)
        {
            Random rng = new Random();
            var selectedAlbums = albums.OrderBy(x => rng.Next()).Take(5).ToList();

            List<Track> tracks = new List<Track>();

            foreach (var album in selectedAlbums)
            {
                var trackResponse = await httpClient.GetStringAsync(album.Tracklist);
                var trackData = JsonConvert.DeserializeObject<TracksResponse>(trackResponse);

                tracks.AddRange(trackData.Tracks.OrderBy(x => rng.Next()).Take(3));
            }

            return tracks;
        }

        public async Task<List<Track>> GetTracksForSelection(int genreId, string decade)
        {
            string decadeStart = $"{decade}-01-01";
            string decadeEnd = $"{int.Parse(decade) + 10}-01-01";

            var allAlbums = await SearchAlbumsByGenreAndDecadeAsync(genreId, decadeStart);
            var filteredAlbums = FilterAlbumsByGenreAndDecade(allAlbums, genreId, decadeStart, decadeEnd);

            return await ExtractTracksFromAlbumsAsync(filteredAlbums);
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

        public class TracksResponse
        {
            public List<Track> Tracks { get; set;}
        }
    }
}

