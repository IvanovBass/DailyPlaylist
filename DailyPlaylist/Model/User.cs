namespace DailyPlaylist.Model
{
    public class User
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

    }
}