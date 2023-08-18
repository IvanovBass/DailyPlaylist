namespace DailyPlaylist.Model
{
    public class User
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; private set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "playlistIds")]
        public List<string> playlistIds { get; set; }

        public User()
        {
            Id = Guid.NewGuid().ToString();
            // automatically creates a GUID Id so that we don't have to code it each time
        }
    }
}