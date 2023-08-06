using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.Model
{
    public class Album
    {
        public int Id { get; }
        public string Title { get; }
        public string Url { get; }
        public string CoverUrl { get; }
        // The url of the album's cover. Add 'size' parameter to the url to change size. Can be 'small', 'medium', 'big', 'xl'
        // Get here to see how to use the Route parameters : https://developers.deezer.com/api
        // You will have to concatenate the parameters to the CoverUrl

        public string CoverSmallUrl { get; }

        public string CoverMediumUrl { get; }

        public string CoverBigUrl { get; }

        public Genre Genre { get; }

        public List<Genre> Genres { get; }

        public int NbTrack { get; }

        public int Duration { get; }  // in seconds

        public DateOnly ReleaseDate { get; }



    }
}
