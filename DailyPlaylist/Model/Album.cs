using Newtonsoft.Json;


namespace DailyPlaylist.Model
{
    public class Album
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("cover_small")]
        public string CoverSmall { get; set; }

        [JsonProperty("cover_medium")]
        public string CoverMedium { get; set; }

        [JsonProperty("cover_big")]
        public string CoverBig { get; set; }

        [JsonProperty("cover_xl")]
        public string CoverXl { get; set; }

        [JsonProperty("md5_image")]
        public string Md5Image { get; set; }

        [JsonProperty("genre_id")]  // id int of the most characteristic genre
        public int GenreId { get; set; }

        [JsonProperty("genres")]  // list of the genres objects
        public List<Genre> Genres { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("fans")]
        public int Fans { get; set; }

        [JsonProperty("tracklist")]  // url to the tracklist
        public string Tracklist { get; set; }

        [JsonProperty("tracks")]
        public List<Track> Tracks { get; set; }

        [JsonProperty("contributors")]
        public List<Contributor> Contributors { get; set; }

        [JsonProperty("artist")]
        public Artist Artist { get; set; }
    }
}
