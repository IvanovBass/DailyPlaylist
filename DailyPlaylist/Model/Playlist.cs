using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.Model
{
    public class Playlist
    {
        private static int CurrentId { get; set; } = 0;
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public List<Track> Tracks { get; set; }
        public DateOnly DateCreation { get; set; }

        public Playlist()
        {
            Id = CurrentId++;
        }
    }
}
