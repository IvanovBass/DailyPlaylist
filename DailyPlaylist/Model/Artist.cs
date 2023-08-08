using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.Model
{
    public class Artist
    {
        [JsonProperty("id")]
        public long Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        public string Url { get; }

        public string PictureURL { get; }
        // Again, you may use 'size' = 'small', 'medium', 'big', 'xl'

        [JsonProperty("picture_small")]
        public string PictureSmall { get; }

        public string PictureMedium { get; }

        public string PictureBig { get; }

        public int NbAlbum { get; }

        public int NbFan { get; }

        public string Tracklist { get; }
    }
}
