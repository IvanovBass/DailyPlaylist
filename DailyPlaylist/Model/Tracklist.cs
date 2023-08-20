using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyPlaylist.Model
{
    public class Tracklist
    {
        private string _name;
        private string _description;

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get => string.IsNullOrEmpty(_name) ? "Name" : _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get => string.IsNullOrEmpty(_description) ? "Describe what makes your Playlist special..." : _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        [JsonProperty(PropertyName = "deezerTrackIds")]
        public List<long> DeezerTrackIds { get; set; }

        [JsonProperty(PropertyName = "dateCreation")]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "dateUpdated")]
        public DateTime DateUpdated { get; set; } = DateTime.Now;

        public Tracklist()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}

