namespace DailyPlaylist.Model
{
    public class Playlist
    {
        public Guid Id { get; private set; }

        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }  // iamgine a method in the get that would simply retrieve the Count of List<Track> Tracks dynamically
        public List<Track> Tracks { get; set; }
        public DateTime DateCreation { get; set; }

        public DateTime DateUpdated { get; set; } = DateTime.Now;

        public Playlist()
        {
            Id = Guid.NewGuid();
        }
    }

}

//public class Playlist    --> this is a format I should go for if I was to import the Deezer's user's created playlist, which is not my goal
//{
//    [JsonProperty("id")]
//    public long Id { get; set; }

//    [JsonProperty("title")]
//    public string Title { get; set; }

//    [JsonProperty("description")]
//    public string Description { get; set; }

//    [JsonProperty("duration")]
//    public int Duration { get; set; }

//    [JsonProperty("public")]
//    public bool Public { get; set; }

//    [JsonProperty("is_loved_track")]
//    public bool IsLovedTrack { get; set; }

//    [JsonProperty("collaborative")]
//    public bool Collaborative { get; set; }

//    [JsonProperty("rating")]
//    public int Rating { get; set; }

//    [JsonProperty("fans")]
//    public int Fans { get; set; }

//    [JsonProperty("link")]
//    public string Link { get; set; }

//    [JsonProperty("share")]
//    public string Share { get; set; }

//    [JsonProperty("picture")]
//    public string Picture { get; set; }

//    [JsonProperty("picture_small")]
//    public string PictureSmall { get; set; }

//    [JsonProperty("picture_medium")]
//    public string PictureMedium { get; set; }

//    [JsonProperty("picture_big")]
//    public string PictureBig { get; set; }

//    [JsonProperty("picture_xl")]
//    public string PictureXl { get; set; }

//    [JsonProperty("checksum")]
//    public string Checksum { get; set; }

//    [JsonProperty("tracks")]
//    public List<Track> Tracks { get; set; }


