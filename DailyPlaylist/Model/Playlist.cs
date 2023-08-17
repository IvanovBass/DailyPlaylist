namespace DailyPlaylist.Model
{
    public class Playlist
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = "Name";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "Describe what makes your playlist special ...";

        [JsonProperty(PropertyName = "deezerTrackIds")]
        public List<long> DeezerTrackIds { get; set; }

        [JsonProperty(PropertyName = "dateCreation")]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "dateUpdated")]
        public DateTime DateUpdated { get; set; } = DateTime.Now;

        public Playlist()
        {
        }
    }

}

