using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.Model
{
    public class Playlist
    {
        public Guid Id { get; private set; }

        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public List<Track> Tracks { get; set; }
        public DateTime DateCreation { get; set; }

        public DateTime DateUpdated { get; set; } = DateTime.Now;

        public Playlist()
        {
            Id = Guid.NewGuid();
        }
    }

}

