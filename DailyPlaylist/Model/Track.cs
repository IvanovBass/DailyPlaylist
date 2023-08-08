using MediaManager.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.Model
{
    public class Track
    {
        [JsonProperty("id")]
        public long Id { get; }

        // By using an integer Id , I got the foolowing error : Newtonsoft.Json.JsonReaderException: 'JSON integer 2284985957 is too large
        // or small for an Int32. Path 'data[9].id', line 1, position 17174.'

        [JsonProperty("title")]
        public string Title { get; }

        public string TitleShort { get; }

        public string Url { get; }

        public string Preview { get; }

        [JsonProperty("duration")]
        public int Duration { get; }

        public int Rank { get; }

        public DateOnly ReleaseDate { get; }

        [JsonProperty("artist")]
        public Artist Artist { get; }

        public Album Album { get; }

        public Genre Genre
        {
            get
            {
                if (Album != null)
                    return Album.Genre;
                else
                    return null; 
            }
        }
    }
}
