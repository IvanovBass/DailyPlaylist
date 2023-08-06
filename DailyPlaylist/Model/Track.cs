using MediaManager.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.Model
{
    public class Track
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleShort { get; set; }

        public string Url { get; set; }

        public string Preview { get; set; }

        public int Duration { get; set; }

        public int Rank { get; set; }

        public DateOnly ReleaseDate { get; set; }

        public Artist Artist { get; set; }

        public Album Album { get; set; }

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
