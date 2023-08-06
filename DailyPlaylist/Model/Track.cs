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
        public int Id { get; }

        public string Title { get; }

        public string TitleShort { get; }

        public string Url { get; }

        public string Preview { get; }

        public int Duration { get; }

        public int Rank { get; }

        public DateOnly ReleaseDate { get; }

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
