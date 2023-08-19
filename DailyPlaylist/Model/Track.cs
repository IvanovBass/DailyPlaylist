using System.ComponentModel;

namespace DailyPlaylist.Model
{
    public class Track : INotifyPropertyChanged
    {
        private string _favoriteImageSource;
        private bool _favorite;


        public bool Favorite
        {
            get => _favorite;
            set
            {
                if (_favorite != value)
                {
                    _favorite = value;
                    OnPropertyChanged(nameof(Favorite));
                    FavoriteImageSource = _favorite ? "heart.png" : "non_favorite.png";
                }
            }
        }
        public string FavoriteImageSource
        {
            get => _favoriteImageSource;
            set
            {
                if (_favoriteImageSource != value)
                {
                    _favoriteImageSource = value;
                    OnPropertyChanged(nameof(FavoriteImageSource));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Track()
        {
            FavoriteImageSource = "non_favorite.png";
        }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("readable")]
        public bool Readable { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_short")]
        public string TitleShort { get; set; }

        [JsonProperty("title_version")]
        public string TitleVersion { get; set; }

        [JsonProperty("isrc")]
        public string Isrc { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("share")]
        public string Share { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("track_position")]
        public int TrackPosition { get; set; }

        [JsonProperty("disk_number")]
        public int DiskNumber { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("explicit_lyrics")]
        public bool ExplicitLyrics { get; set; }

        [JsonProperty("explicit_content_lyrics")]
        public int ExplicitContentLyrics { get; set; }

        [JsonProperty("explicit_content_cover")]
        public int ExplicitContentCover { get; set; }

        [JsonProperty("preview")]
        public string Preview { get; set; }

        [JsonProperty("bpm")]
        public double Bpm { get; set; }

        [JsonProperty("gain")]
        public double Gain { get; set; }

        [JsonProperty("available_countries")]
        public List<string> AvailableCountries { get; set; }

        [JsonProperty("contributors")]
        public List<Contributor> Contributors { get; set; }

        [JsonProperty("md5_image")]
        public string Md5Image { get; set; }

        [JsonProperty("artist")]
        public Artist Artist { get; set; }

        [JsonProperty("album")]
        public Album Album { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
